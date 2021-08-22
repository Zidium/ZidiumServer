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
using Microsoft.Extensions.Logging;
using Zidium.Api;
using Zidium.Api.Dto;
using Zidium.Common;
using Zidium.Core;
using Zidium.Core.AccountsDb;
using Zidium.Core.InternalLogger;
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

        public void ConfigureServices(IServiceCollection services)
        {
            var configuration = new Configuration(_appConfiguration);
            DependencyInjection.SetServicePersistent<IDebugConfiguration>(configuration);
            DependencyInjection.SetServicePersistent<IDatabaseConfiguration>(configuration);
            DependencyInjection.SetServicePersistent<IAccessConfiguration>(configuration);
            DependencyInjection.SetServicePersistent<ILogicConfiguration>(configuration);
            DependencyInjection.SetServicePersistent<IStorageFactory>(new StorageFactory());

            services.AddHttpContextAccessor();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
        {
            try
            {
                Client.Instance.Disable = true;
                var loggerFactory = app.ApplicationServices.GetRequiredService<ILoggerFactory>();
                DependencyInjection.SetLoggerFactory(loggerFactory);
                DependencyInjection.SetServicePersistent<InternalLoggerComponentMapping>(app.ApplicationServices.GetRequiredService<InternalLoggerComponentMapping>());

                // Для упрощения пусть диспетчер отвечает за актуальность базы
                var storageFactory = DependencyInjection.GetServicePersistent<IStorageFactory>();
                Mirgate(storageFactory);

                Client.Instance.Disable = false;

                DispatcherService.Version = DispatcherWebHandlerMiddleware.GetVersion();
                var componentControl = DispatcherService.Wrapper.Control;
                logger.LogInformation("Start, IsFake={0}", componentControl.IsFake());
                logger.LogInformation("Version {0}", VersionHelper.GetProductVersion());

                DispatcherWebHandlerMiddleware.Init(DispatcherService.Wrapper.Control);
                DebugInfoMiddleware.Init(DispatcherService.Wrapper.Control);
                TestInfoMiddleware.Init(DispatcherService.Wrapper.Control);
                InitLimitsSaving();

                var serverAddressesFeature = app.ServerFeatures.Get<IServerAddressesFeature>();
                logger.LogInformation("Listening on: " + (serverAddressesFeature.Addresses.Count > 0 ? string.Join("; ", serverAddressesFeature.Addresses) : "IIS reverse proxy"));

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
            catch (Exception exception)
            {
                logger.LogCritical(exception, exception.Message);
                throw;
            }
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

        protected void Mirgate(IStorageFactory storageFactory)
        {
            // Выполняем миграции
            var storage = storageFactory.GetStorage();
            storage.Migrate();

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
