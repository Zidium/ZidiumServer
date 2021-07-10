using System;
using System.Threading;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NLog;
using Zidium.Api;
using Zidium.Api.Dto;
using Zidium.Common;
using Zidium.Core;
using Zidium.Core.AccountsDb;
using Zidium.Core.Limits;
using Zidium.Storage;
using Zidium.Storage.Ef;

namespace Zidium.Dispatcher
{
    public class Startup
    {
        public Startup(IConfiguration appConfiguration)
        {
            _appConfiguration = appConfiguration;
        }

        private readonly IConfiguration _appConfiguration;
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public void ConfigureServices(IServiceCollection services)
        {
            var configuration = new Configuration(_appConfiguration);
            DependencyInjection.SetServicePersistent<IDebugConfiguration>(configuration);
            DependencyInjection.SetServicePersistent<IDatabaseConfiguration>(configuration);
            DependencyInjection.SetServicePersistent<IAccessConfiguration>(configuration);
            DependencyInjection.SetServicePersistent<ILogicConfiguration>(configuration);
            DependencyInjection.SetServicePersistent<IStorageFactory>(new StorageFactory());
            var defaultStorageFactory = new DefaultStorageFactory();
            DependencyInjection.SetServicePersistent<IDefaultStorageFactory>(defaultStorageFactory);

            services.AddHttpContextAccessor();

            // Для упрощения пусть диспетчер отвечает за актуальность базы
            Client.Instance.Disable = true;
            Mirgate(defaultStorageFactory);

            DispatcherService.Version = DispatcherWebHandlerMiddleware.GetVersion();
            var componentControl = DispatcherService.Wrapper.Control;
            Client.Instance.Disable = false;

            _logger.Info("Start, IsFake={0}", componentControl.IsFake());
            _logger.Info("Version {0}", VersionHelper.GetProductVersion());

            DispatcherWebHandlerMiddleware.Init(DispatcherService.Wrapper.Control);
            DebugInfoMiddleware.Init(DispatcherService.Wrapper.Control);
            TestInfoMiddleware.Init(DispatcherService.Wrapper.Control);
            InitLimitsSaving();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            var serverAddressesFeature = app.ServerFeatures.Get<IServerAddressesFeature>();
            _logger.Info("Listening on: " + (serverAddressesFeature.Addresses.Count > 0 ? string.Join("; ", serverAddressesFeature.Addresses) : "IIS reverse proxy"));

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            HttpServiceEventPreparer.Configure(app.ApplicationServices.GetRequiredService<IHttpContextAccessor>());
            Client.Instance.EventPreparer = new HttpServiceEventPreparer();

            app.Map("/debug", c =>
            {
                c.UseWhen(t => "GET".Equals(t.Request.Method, StringComparison.OrdinalIgnoreCase),
                    x => x.UseMiddleware<DebugInfoMiddleware>());
            });

            app.Map("/test", c =>
            {
                c.UseWhen(t => "GET".Equals(t.Request.Method, StringComparison.OrdinalIgnoreCase),
                    x => x.UseMiddleware<TestInfoMiddleware>());
            });

            app.Map("/error", c =>
            {
                c.UseWhen(t => "GET".Equals(t.Request.Method, StringComparison.OrdinalIgnoreCase),
                    x => x.UseMiddleware<TestErrorMiddleware>());
            });

            app.Map("", c =>
            {
                c.UseWhen(t => "POST".Equals(t.Request.Method, StringComparison.OrdinalIgnoreCase),
                    x => x.UseMiddleware<DispatcherWebHandlerMiddleware>());
                c.UseWhen(t => "GET".Equals(t.Request.Method, StringComparison.OrdinalIgnoreCase) &&
                               "/GetServerTime".Equals(t.Request.Path),
                    x => x.UseMiddleware<DispatcherWebHandlerMiddleware>());
                c.UseWhen(t => "/".Equals(t.Request.Path),
                    x => x.UseMiddleware<TestInfoMiddleware>());
            });
        }

        protected void InitLimitsSaving()
        {
            var control = DispatcherService.Wrapper.Control;
            if (control != null)
            {
                UnitTestControl = control.GetOrCreateUnitTestControl("Dispatcher.LimitsSaving");
            }
            StartLimitsSavingTimer();
        }

        protected Timer LimitsSavingTimer;

        protected IUnitTestControl UnitTestControl;

        protected void DoSaveLimits(object obj)
        {
            var startTime = DateTime.Now;
            var count = AccountLimitsCheckerManager.Save();
            var endTime = DateTime.Now;
            var duration = endTime - startTime;
            var control = DispatcherService.Wrapper.Control;
            if (AccountLimitsCheckerManager.LastSaveException != null)
            {
                if (control != null)
                {
                    control.SendApplicationError(AccountLimitsCheckerManager.LastSaveException);
                }
            }

            if (UnitTestControl != null)
            {
                if (AccountLimitsCheckerManager.LastSaveException == null)
                {
                    UnitTestControl.SendResult(UnitTestResult.Success, TimeSpan.FromMinutes(10), "Сохранено записей: " + count + " за " + duration);
                }
                else
                {
                    UnitTestControl.SendResult(UnitTestResult.Alarm, TimeSpan.FromMinutes(10), AccountLimitsCheckerManager.LastSaveException.Message);
                }
            }

            StartLimitsSavingTimer();
        }

        protected void StartLimitsSavingTimer()
        {
            if (LimitsSavingTimer != null)
            {
                LimitsSavingTimer.Dispose();
            }
            LimitsSavingTimer = new Timer(DoSaveLimits, null, AccountLimitsChecker.LimitDataTimeStep * 60 * 1000, Timeout.Infinite);
        }

        protected void Mirgate(DefaultStorageFactory defaultStorageFactory)
        {
            // Выполняем миграции
            var storage = defaultStorageFactory.GetStorage();
            storage.Migrate(_logger);

            // обновляем справочники
            var registrator = new AccountDbDataRegistator(storage);
            registrator.RegisterAll();

            // Проверим, создан ли админ
            var userService = new UserService(storage);
            var adminCreated = userService.GetAccountAdmins().Length > 0;

            if (!adminCreated)
            {
                // создаем root компонент
                var componentService = new ComponentService(storage);
                componentService.CreateRoot(Core.AccountsDb.SystemComponentType.Root.Id);

                // создаем админа
                var adminUserId = userService.CreateAccountAdmin(
                    "Admin",
                    null, null, null, null, null);

                // Установим пароль админа
                var passwordToken = userService.StartResetPassword(adminUserId, false);
                userService.EndResetPassword(passwordToken, "12345");
            }

        }
    }
}
