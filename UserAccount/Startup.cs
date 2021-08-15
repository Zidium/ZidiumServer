using System;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Routing.Constraints;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Web.ModelBindings;
using Zidium.Api;
using Zidium.Common;
using Zidium.Core;
using Zidium.Core.Common.Helpers;
using Zidium.Core.InternalLogger;
using Zidium.Storage;
using Zidium.Storage.Ef;
using Zidium.UserAccount.Helpers;

namespace Zidium.UserAccount
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
            DependencyInjection.SetServicePersistent<IDispatcherConfiguration>(configuration);
            DependencyInjection.SetServicePersistent<IAccessConfiguration>(configuration);
            DependencyInjection.SetServicePersistent<IStorageFactory>(new StorageFactory());
            DependencyInjection.SetServicePersistent<IDefaultStorageFactory>(new DefaultStorageFactory());

            services.AddHttpContextAccessor();
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.AddScoped<HtmlHelperGenerator>();

            EnumHelper.RegisterNaming(new ObjectColorNaming());
            EnumHelper.RegisterNaming(new EventImportanceNaming());
            EnumHelper.RegisterNaming(new MonitoringStatusNaming());
            EnumHelper.RegisterNaming(new EventCategoryNaming());

            services.AddControllersWithViews(options =>
            {
                options.ModelBinderProviders.Insert(0, new ZidiumModelBinderProvider());
            });

            services.AddSession();

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.Cookie.Name = "ZidiumAuth";
                    options.LoginPath = new PathString("/Account/Logon");
                    options.ExpireTimeSpan = TimeSpan.FromDays(7);
                });
            services.AddAuthorization();

            services.AddTransient<GlobalExceptionFilterAttribute>();

            InitMonitoring(services);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger, IComponentControl componentControl)
        {
            try
            {
                logger.LogInformation("Start, IsFake={0}", componentControl.IsFake());
                logger.LogInformation("Version {0}", VersionHelper.GetProductVersion());

                var loggerFactory = app.ApplicationServices.GetRequiredService<ILoggerFactory>();
                DependencyInjection.SetLoggerFactory(loggerFactory);
                DependencyInjection.SetServicePersistent<InternalLoggerComponentMapping>(app.ApplicationServices.GetRequiredService<InternalLoggerComponentMapping>());

                var serverAddressesFeature = app.ServerFeatures.Get<IServerAddressesFeature>();
                logger.LogInformation("Listening on: " + (serverAddressesFeature.Addresses.Count > 0 ? string.Join("; ", serverAddressesFeature.Addresses) : "IIS reverse proxy"));

                app.UseForwardedHeaders(new ForwardedHeadersOptions
                {
                    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
                });

                app.UseDeveloperExceptionPage();

                if (env.IsDevelopment())
                {
                    app.UseDeveloperExceptionPage();
                }

                var httpContextAccessor = app.ApplicationServices.GetRequiredService<IHttpContextAccessor>();
                WebApplicationEventPreparer.Configure(httpContextAccessor);
                Client.Instance.EventPreparer = new WebApplicationEventPreparer();

                app.Use(async (ctx, next) =>
                {
                    await next();

                    if (ctx.Response.StatusCode == 404 && !ctx.Response.HasStarted)
                    {
                        if (!ctx.Request.IsAjaxRequest() && !ctx.Request.IsSmartBlocksRequest())
                        {
                            ctx.Request.Path = "/Home/Error404";
                            await next();
                        }
                    }
                });

                app.UseHttpsRedirection();
                app.UseStaticFiles();
                app.UseRouting();
                app.UseSession();
                app.UseAuthentication();
                app.UseAuthorization();

                app.UseEndpoints(endpoints =>
                {
                    // ����������

                    endpoints.MapControllerRoute(
                            name: "ShowComponent",
                            pattern: "Components/{id}",
                            defaults: new { controller = "Components", action = "Show" },
                            constraints: new { id = new GuidRouteConstraint() });

                    // ���� �����������

                    endpoints.MapControllerRoute(
                            name: "ShowComponentType",
                            pattern: "ComponentTypes/{id}",
                            defaults: new { controller = "ComponentTypes", action = "Show" },
                            constraints: new { id = new GuidRouteConstraint() }
                        );

                    // �������

                    endpoints.MapControllerRoute(
                            name: "ShowEvent",
                            pattern: "Events/{id}",
                            defaults: new { controller = "Events", action = "Show" },
                            constraints: new { id = new GuidRouteConstraint() }
                        );

                    // ������������

                    endpoints.MapControllerRoute(
                            name: "ShowUser",
                            pattern: "Users/{id}",
                            defaults: new { controller = "Users", action = "Show" },
                            constraints: new { id = new GuidRouteConstraint() }
                        );

                    // ��������� ��������

                    endpoints.MapControllerRoute(
                            name: "StartPage",
                            pattern: "",
                            defaults: new { controller = "Home", action = "Start" }
                        );

                    // �����

                    endpoints.MapControllerRoute(
                            name: "default",
                            pattern: "{controller=Home}/{action=Index}/{id?}");
                });
            }
            catch (Exception exception)
            {
                logger.LogCritical(exception, exception.Message);
                throw;
            }
        }

        protected void InitMonitoring(IServiceCollection services)
        {
            var client = SystemAccountHelper.GetInternalSystemClient();
            client.WaitUntilAvailable(TimeSpan.FromSeconds(60));

            // �������� ���������
            // ���� ����������� � �������, �� ��������� ����� �� � �����, � � ����� DEBUG
            var debugConfiguration = DependencyInjection.GetServicePersistent<IDebugConfiguration>();
            var folder = !debugConfiguration.DebugMode ? client.GetRootComponentControl() : client.GetRootComponentControl().GetOrCreateChildFolderControl("DEBUG");
            var componentType = client.GetOrCreateComponentTypeControl(!debugConfiguration.DebugMode ? SystemComponentType.WebSite.SystemName : DebugHelper.DebugComponentType);
            var componentControl = folder
                .GetOrCreateChildComponentControl(new GetOrCreateComponentData("UserAccountWebSite", componentType)
                {
                    DisplayName = "������ ������� ������������",
                    Version = VersionHelper.GetProductVersion()
                });

            // �������� Id ���������� �� ���������, ����� ������� ����������� ��� ��� ������������
            Client.Instance = client;
            Client.Instance.Config.DefaultComponent.Id = componentControl.Info?.Id;
            services.AddSingleton(componentControl);
        }

    }
}
