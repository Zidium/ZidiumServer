using System;
using System.Data.Entity;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using NLog;
using Zidium.Api;
using Zidium.Core;
using Zidium.Core.AccountsDb;
using Zidium.Core.Common.Helpers;
using Zidium.Core.ConfigDb;
using Zidium.UserAccount.Binders;
using Zidium.UserAccount.Controllers;
using Zidium.UserAccount.Helpers;
using Zidium.UserAccount.Models;
using Zidium.UserAccount.Models.Controls;

namespace Zidium.UserAccount
{
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            // Приложение не должно накатывать миграции или создавать базы
            AccountDbContext.DisableMigrations();

            Initialization.SetServices();
            InitMonitoring();

            try
            {
                // Загрузим подключаемую сборку с управлением аккаунтом
                Assembly.LoadFile(Path.Combine(HostingEnvironment.ApplicationPhysicalPath, @"bin\Zidium.UserAccount.AddIn.dll"));

                AreaRegistration.RegisterAllAreas();
                FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
                RouteConfig.RegisterRoutes(RouteTable.Routes);
                BundleConfig.RegisterBundles(BundleTable.Bundles);
                RegisterBinders();

                EnumHelper.RegisterNaming(new ObjectColorNaming());
                EnumHelper.RegisterNaming(new BalanceOperationNaming());
                EnumHelper.RegisterNaming(new EventImportanceNaming());
                EnumHelper.RegisterNaming(new MonitoringStatusNaming());
                EnumHelper.RegisterNaming(new EventCategoryNaming());

                ClientDataTypeModelValidatorProvider.ResourceClassKey = "RussianResources";
                DefaultModelBinder.ResourceClassKey = "RussianResources";
                Thread.CurrentThread.CurrentCulture = CultureHelper.Russian;
            }
            catch (Exception exception)
            {
                LogManager.GetCurrentClassLogger().Fatal(exception);
            }
        }

        protected static void RegisterBinders()
        {
            ModelBinders.Binders.Add(typeof(DateTime?), new DateTimeModelBinder());
            ModelBinders.Binders.Add(typeof(DateTime), new DateTimeModelBinder());
            ModelBinders.Binders.Add(typeof(ColorStatusSelectorValue), new ColorStatusValueBinder());
            ModelBinders.Binders.Add(typeof(TimeSpan), new TimeSpanBinder());
            ModelBinders.Binders.Add(typeof(TimeSpan?), new TimeSpanBinder());
            ModelBinders.Binders.Add(typeof(Time), new TimeBinder());
            ModelBinders.Binders.Add(typeof(Time?), new TimeBinder());
        }

        public static IComponentControl ComponentControl { get; protected set; }

        protected void InitMonitoring()
        {
            var client = SystemAccountHelper.GetInternalSystemClient();
            client.EventPreparer = new WebApplicationEventPreparer();

            // Создадим компонент
            // Если запускаемся в отладке, то компонент будет не в корне, а в папке DEBUG
            var folder = !DebugHelper.IsDebugMode ? client.GetRootComponentControl() : client.GetRootComponentControl().GetOrCreateChildFolderControl("DEBUG");
            var componentType = client.GetOrCreateComponentTypeControl(!DebugHelper.IsDebugMode ? SystemComponentTypes.WebSite.SystemName : DebugHelper.DebugComponentType);
            ComponentControl = folder
                .GetOrCreateChildComponentControl(new GetOrCreateComponentData("UserAccountWebSite", componentType)
                {
                    DisplayName = "Личный кабинет пользователя",
                    Version = Version
                });

            // Присвоим Id компонента по умолчанию, чтобы адаптер NLog мог его использовать
            Client.Instance = client;
            Client.Instance.Config.DefaultComponent.Id = ComponentControl.Info?.Id;

            LogManager.GetCurrentClassLogger().Info("Запуск, IsFake={0}", ComponentControl.IsFake());
        }

        protected void Application_Error(object sender, EventArgs args)
        {
            try
            {
                var exception = Server.GetLastError();
                var request = new HttpRequestWrapper(HttpContext.Current.Request);
                var isAjax = request.IsAjaxRequest();

                var httpException = exception as HttpException;
                if (httpException != null)
                {
                    if (httpException.GetHttpCode() == 404)
                    {
                        if (!isAjax)
                        {
                            using (var controller = new HomeController())
                            {
                                var controllerContext = new ControllerContext(HttpContext.Current.Request.RequestContext, controller);
                                var result = new ViewResult
                                {
                                    ViewName = "~/Views/Errors/Error404.cshtml"
                                };
                                result.ExecuteResult(controllerContext);
                                HttpContext.Current.Response.End();
                            }

                            return;
                        }
                    }
                }

                if (isAjax && request.IsSmartBlocksRequest())
                {
                    HandleException(exception);

                    using (var controller = new HomeController())
                    {
                        var controllerContext = new ControllerContext(HttpContext.Current.Request.RequestContext, controller);
                        var result = controller.GetErrorJsonResponse(exception);
                        result.ExecuteResult(controllerContext);
                        HttpContext.Current.Response.End();
                    }

                    return;
                }

                HandleException(exception);

                using (var controller = new HomeController())
                {
                    var controllerContext = new ControllerContext(HttpContext.Current.Request.RequestContext, controller);
                    var result = new ViewResult
                    {
                        ViewName = "~/Views/Errors/Error.cshtml",
                        ViewData =
                        {
                            Model = new ErrorModel()
                            {
                                Exception = exception,
                                IsAjax = isAjax
                            }
                        }
                    };
                    result.ExecuteResult(controllerContext);
                    HttpContext.Current.Response.End();
                }
            }
            catch (Exception exception)
            {
                LogManager.GetCurrentClassLogger().Fatal(exception);
            }
        }

        public static string HandleException(Exception exception)
        {
            if (exception == null)
                return null;

            if (exception is UserFriendlyException)
                return null;

            Tools.HandleOutOfMemoryException(exception);
            LogManager.GetCurrentClassLogger().Error(exception);

            exception.HelpLink = new ExceptionRender().GetExceptionTypeCode(exception);
            return exception.HelpLink;
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {
            // Залогируем остановку
            LogManager.GetCurrentClassLogger().Info("Остановка");
            Client.Instance.Flush();
        }

        public static DateTime GetServerDateTime()
        {
            return Client.Instance.ToServerTime(DateTime.Now);
        }

        public static string Version
        {
            get { return typeof(MvcApplication).Assembly.GetName().Version.ToString(); }
        }

    }
}
