using System;
using System.IO;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using Zidium.Core;
using Zidium.Core.AccountsDb;
using Zidium.Core.AccountsDb.Classes;
using Zidium.Core.Api;
using Zidium.Core.Api.Dispatcher;
using Zidium.Core.Common;
using Zidium.Core.Common.Helpers;
using Zidium.Core.ConfigDb;
using Zidium.UserAccount.Helpers;
using Zidium.UserAccount.Models;

namespace Zidium.UserAccount.Controllers
{
    public class ContextController : Controller
    {
        /// <summary>
        /// Текущий пользователь
        /// </summary>
        /// <returns></returns>
        public UserInfo CurrentUser { get; private set; }

        public DatabasesContext DbContext { get; private set; }

        public AccountDbContext CurrentAccountDbContext
        {
            get
            {
                if (CurrentUser == null)
                {
                    return null;
                }
                return DbContext.GetAccountDbContext(CurrentUser.AccountId);
            }
        }

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
            return ConfigDbServicesHelper.GetAccountService().GetOneById(CurrentUser.AccountId);
        }

        protected Component GetComponentById(Guid id)
        {
            var repository = CurrentAccountDbContext.GetComponentRepository();
            return repository.GetById(id);
        }

        protected UnitTest GetUnitTestById(Guid id)
        {
            var repository = CurrentAccountDbContext.GetUnitTestRepository();
            return repository.GetById(id);
        }

        protected Metric GetMetricById(Guid id)
        {
            var repository = CurrentAccountDbContext.GetMetricRepository();
            return repository.GetById(id);
        }

        protected MetricType GetMetricTypeById(Guid id)
        {
            var repository = CurrentAccountDbContext.GetMetricTypeRepository();
            return repository.GetById(id);
        }

        protected EventType GetEventTypeById(Guid id)
        {
            var repository = CurrentAccountDbContext.GetEventTypeRepository();
            return repository.GetById(id);
        }

        protected Event GetEventById(Guid id)
        {
            var repository = CurrentAccountDbContext.GetEventRepository();
            return repository.GetById(id);
        }

        protected User GetUserById(Guid id)
        {
            var repository = CurrentAccountDbContext.GetUserRepository();
            return repository.GetById(id);
        }

        protected Defect GetDefectById(Guid id)
        {
            var repository = CurrentAccountDbContext.GetDefectRepository();
            return repository.GetById(id);
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
                    ConfigDbServicesHelper.GetLoginService().UpdateUserAgentTag(CurrentUser.Id, userAgentTagGuid);
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
            DbContext = FullRequestContext.Current.DbContext;

            if (Request.IsAuthenticated && !CurrentUser.IsSwitched)
            {
                // не будем логировать действия системных пользователей
                // а то когда смотришь лог, в самых актуальных записях видишь свои же запросы
                if (!string.Equals(CurrentUser.AccountName, SystemAccountHelper.SystemAccountName, StringComparison.OrdinalIgnoreCase))
                {
                    var uri = filterContext.HttpContext.Request.Url;
                    if (uri != null && !filterContext.IsChildAction && !filterContext.HttpContext.Request.IsAjaxRequest())
                    {
                        var actionName = filterContext.ActionDescriptor.ActionName;
                        var controllerName = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;
                        var baseUrl = controllerName + "/" + actionName;
                        ConfigDbServicesHelper.GetUserAccountActivityService().Add(CurrentUser.Id, baseUrl);
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

        /// <summary>
        /// Освобождение контекстов после отправки страницы
        /// </summary>
        protected override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            base.OnResultExecuted(filterContext);
            if (DbContext != null)
            {
                if (!filterContext.IsChildAction)
                    DbContext.Dispose();
                DbContext = null;
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
        public ContextController(Guid? accountId, Guid? userId)
        {
            ControllerContext = new ControllerContext();

            var request = new HttpRequest(
                string.Empty,
                @"//localhost/",
                string.Empty);

            RouteData.Values.Add("controller", GetType().Name.ToLower().Replace("controller", ""));

            var response = new HttpResponse(new StringWriter());

            ControllerContext.HttpContext = new HttpContextWrapper(new HttpContext(request, response));

            var routes = new RouteCollection();
            RouteConfig.RegisterRoutes(routes);
            Url = new System.Web.Mvc.UrlHelper(ControllerContext.RequestContext, routes);

            System.Web.HttpContext.Current = new HttpContext(request, response);

            var fullContext = new FullRequestContext(this);
            FullRequestContext.Current = fullContext;

            DbContext = fullContext.DbContext;

            if (accountId.HasValue)
            {
                var accountDbContext = DbContext.GetAccountDbContext(accountId.Value);
                if (userId.HasValue)
                {
                    var user = accountDbContext.GetUserRepository().GetById(userId.Value);
                    CurrentUser = UserHelper.UserInfoByUser(user, accountId.Value);
                    fullContext.SetUser(CurrentUser);
                }
            }
        }

        public ContextController() { }

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
    }
}