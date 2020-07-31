using System;
using System.IO;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using Zidium.Common;
using Zidium.Core;
using Zidium.Core.Api;
using Zidium.Core.Api.Dispatcher;
using Zidium.Core.Common;
using Zidium.Core.Common.Helpers;
using Zidium.Core.ConfigDb;
using Zidium.Storage;
using Zidium.UserAccount.Helpers;
using Zidium.UserAccount.Models;

namespace Zidium.UserAccount.Controllers
{
    public class BaseController : Controller
    {
        /// <summary>
        /// Текущий пользователь
        /// </summary>
        /// <returns></returns>
        public UserInfo CurrentUser { get; private set; }

        protected ActionResult ViewDialog(DeleteConfirmationAjaxModel model)
        {
            return View("Dialogs/DeleteConfirmationAjax", model);
        }

        protected ActionResult ViewDialog(DeleteConfirmationSmartModel model)
        {
            return PartialView("Dialogs/DeleteConfirmationSmart", model);
        }

        protected AccountInfo GetCurrentAccount()
        {
            return FullRequestContext.Current.CurrentAccount;
        }

        protected virtual void ProcessUserAgentTag(ActionExecutingContext filterContext)
        {
            if (filterContext == null)
            {
                return;
            }
            if (filterContext.RequestContext == null)
            {
                return;
            }

            var request = filterContext.RequestContext.HttpContext.Request;
            if (request.IsLocal)
            {
                // чтобы не мусорить из студии
                return;
            }
            var tag = request.Cookies["uat"];

            if (tag == null)
            {
                return;
            }

            Guid userAgentTagGuid;
            if (Guid.TryParse(tag.Value, out userAgentTagGuid) == false)
            {
                tag.Expires = DateTime.Now.AddDays(-10);
                return;
            }

            // если это бот
            if (UserAgentHelper.IsBot(request.UserAgent))
            {
                return;
            }

            // установим время жизни
            if (tag.Expires.Year < 2030)
            {
                tag.Expires = new DateTime(2030, 1, 1);
            }

            // обновим userAgentTagId логина
            if (CurrentUser != null)
            {
                var cookieUserAgentTagValue = tag.Value;
                var sessionKey = "userAgentTagId";
                var sessionValue = Session[sessionKey] as string;

                if (sessionValue != cookieUserAgentTagValue)
                {
                    Session[sessionKey] = cookieUserAgentTagValue;
                    GetConfigDbServicesFactory().GetLoginService().UpdateUserAgentTag(CurrentUser.Id, userAgentTagGuid);
                }
            }
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            Thread.CurrentThread.CurrentCulture = CultureHelper.Russian;

            base.OnActionExecuting(filterContext);

            CurrentUser = UserHelper.CurrentUser;

            ProcessUserAgentTag(filterContext);

            if (Request.IsAuthenticated)
            {
                if (CurrentUser == null)
                {
                    Session.Abandon();
                    FormsAuthentication.SignOut();
                    filterContext.Result = new RedirectResult("~/");
                    return;
                }
            }

            if (!filterContext.IsChildAction)
            {
                var fullContext = new FullRequestContext(this);
                FullRequestContext.Current = fullContext;
            }

            if (Request.IsAuthenticated && !CurrentUser.IsSwitched)
            {
                // не будем логировать действия системных пользователей
                // а то когда смотришь лог, в самых актуальных записях видишь свои же запросы
                if (!string.Equals(CurrentUser.AccountName, SystemAccountHelper.SystemAccountName,
                    StringComparison.OrdinalIgnoreCase))
                {
                    var uri = filterContext.HttpContext.Request.Url;
                    if (uri != null && !filterContext.IsChildAction &&
                        !filterContext.HttpContext.Request.IsAjaxRequest())
                    {
                        var actionName = filterContext.ActionDescriptor.ActionName;
                        var controllerName = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;
                        var baseUrl = controllerName + "/" + actionName;
                        GetConfigDbServicesFactory().GetUserAccountActivityService().Add(CurrentUser.Id, baseUrl);
                    }
                }

                var control = MvcApplication.ComponentControl;

                var message = string.Format(
                    "Пользователь {0} c IP {1}",
                    CurrentUser.Login,
                    FullRequestContext.Current.Ip);

                control
                    .CreateComponentEvent("Пользователь на сайте", message)
                    .SetImportance(Api.EventImportance.Success)
                    .SetJoinInterval(TimeSpan.FromMinutes(Session.Timeout))
                    .SetJoinKey(CurrentUser.Login, FullRequestContext.Current.Ip)
                    .SetProperty("UserAgent", filterContext.HttpContext.Request.UserAgent)
                    .SetProperty("IP", FullRequestContext.Current.Ip)
                    .Add();
            }

        }

        protected override void OnException(ExceptionContext filterContext)
        {
            if (filterContext.HttpContext.Request.IsAjaxRequest() && filterContext.HttpContext.Request.IsSmartBlocksRequest())
            {
                MvcApplication.HandleException(filterContext.Exception);
                filterContext.ExceptionHandled = true;
                filterContext.Result = GetErrorJsonResponse(filterContext.Exception);
                return;
            }

            string viewName = null;

            if (filterContext.Exception is ObjectNotFoundException)
            {
                viewName = "~/Views/Errors/ObjectNotFoundError.cshtml";
            }
            else if (filterContext.Exception is AccessDeniedException)
            {
                viewName = "~/Views/Errors/AccessDeniedError.cshtml";
            }
            else if (filterContext.Exception is UserFriendlyException)
            {
                viewName = "~/Views/Errors/UserFriendlyError.cshtml";
            }
            else
            {
                viewName = "~/Views/Errors/CommonError.cshtml";
                MvcApplication.HandleException(filterContext.Exception);
            }

            filterContext.ExceptionHandled = true;
            var controllerName = (string)filterContext.RouteData.Values["controller"];
            var actionName = (string)filterContext.RouteData.Values["action"];
            var model = new HandleErrorInfo(filterContext.Exception, controllerName, actionName);

            var result = new ViewResult
            {
                ViewName = viewName,
                ViewData = new ViewDataDictionary<HandleErrorInfo>(model),
                TempData = filterContext.Controller.TempData
            };
            result.ViewBag.IsChildAction = filterContext.IsChildAction;

            var fullContext = new FullRequestContext(this);
            FullRequestContext.Current = fullContext;

            filterContext.Result = result;
        }

        /// <summary>
        /// В таком формате дата и время будут отображаться в строке запроса
        /// </summary>
        public const string MomentDateTimeParamFormat = "YYYY-MM-DD_HH.mm.ss";

        /// <summary>
        /// В таком формате дата и время будут отображаться на экране
        /// </summary>
        public const string MomentDateTimeDisplayFormat = "DD.MM.YYYY HH:mm:ss";

        protected DateTime DecodeDateTimeParameter(string value)
        {
            return ParseHelper.ParseDateTime(value);
        }

        public DispatcherClient GetDispatcherClient()
        {
            if (_dispatcherClient == null)
                _dispatcherClient = new DispatcherClient("UserAccount." + GetType().Name);
            return _dispatcherClient;
        }

        private DispatcherClient _dispatcherClient;

        /// <summary>
        /// Для unit-тестов
        /// </summary>
        public BaseController(Guid? accountId, Guid? userId, bool isSmartBlocksRequest = false)
        {
            ControllerContext = new ControllerContext();

            var request = new HttpRequest(
                string.Empty,
                @"//localhost/",
                string.Empty);

            _isSmartBlocksRequestOverride = isSmartBlocksRequest;

            RouteData.Values.Add("controller", GetType().Name.ToLower().Replace("controller", ""));

            var response = new HttpResponse(new StringWriter());

            ControllerContext.HttpContext = new HttpContextWrapper(new HttpContext(request, response));

            var routes = new RouteCollection();
            RouteConfig.RegisterRoutes(routes);
            Url = new System.Web.Mvc.UrlHelper(ControllerContext.RequestContext, routes);

            System.Web.HttpContext.Current = new HttpContext(request, response);

            var fullContext = new FullRequestContext(this);
            FullRequestContext.Current = fullContext;

            if (accountId.HasValue)
            {
                if (userId.HasValue)
                {
                    var storage = _accountStorageFactory.GetStorageByAccountId(accountId.Value);
                    var user = storage.Users.GetOneById(userId.Value);
                    var roles = storage.Roles.GetByUserId(user.Id);

                    CurrentUser = UserHelper.UserInfoByUser(user, roles, accountId.Value);
                    fullContext.SetUser(CurrentUser);
                }
            }
        }

        public BaseController() { }

        public JsonResult GetSuccessJsonResponse(object data = null)
        {
            var response = MyJsonHelper.GetSuccessResponse(data);
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetErrorJsonResponse(Exception exception)
        {
            var response = MyJsonHelper.GetErrorResponse(exception);
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        public void SetCommonError(Exception exception)
        {
            ViewBag.CommonErrorException = exception;
        }

        public void SetCommonError(string errorMessage)
        {
            var exception = new UserFriendlyException(errorMessage);
            SetCommonError(exception);
        }

        public string Referrer
        {
            get
            {
                if (Request.UrlReferrer == null)
                    return null;
                return Request.UrlReferrer.ToString();
            }
        }

        public DateTime Now()
        {
            return GetDispatcherClient().GetServerTime().Data.Date;
        }

        public bool IsSmartBlocksRequest()
        {
            return _isSmartBlocksRequestOverride || Request.IsSmartBlocksRequest();
        }

        private readonly bool _isSmartBlocksRequestOverride;

        public IConfigDbServicesFactory GetConfigDbServicesFactory()
        {
            if (_configDbServicesFactory == null)
            {
                _configDbServicesFactory = DependencyInjection.GetServicePersistent<IConfigDbServicesFactory>();
            }

            return _configDbServicesFactory;
        }

        private IConfigDbServicesFactory _configDbServicesFactory;

        private readonly IAccountStorageFactory _accountStorageFactory = DependencyInjection.GetServicePersistent<IAccountStorageFactory>();

        private IStorage _storage;

        public IStorage GetStorage()
        {
            if (_storage == null)
                _storage = _accountStorageFactory.GetStorageByAccountId(CurrentUser.AccountId);
            return _storage;
        }

        protected IStorage GetStorage(Guid accountId)
        {
            return _accountStorageFactory.GetStorageByAccountId(accountId);
        }

    }
}