using System;
using System.Threading;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Zidium.Api;
using Zidium.Api.Dto;
using Zidium.Common;
using Zidium.Core;
using Zidium.Core.Api.Dispatcher;
using Zidium.Core.Common;
using Zidium.Storage;
using Zidium.UserAccount.Helpers;
using Zidium.UserAccount.Models;

namespace Zidium.UserAccount.Controllers
{
    [ServiceFilter(typeof(GlobalExceptionFilterAttribute))]
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

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            Thread.CurrentThread.CurrentCulture = CultureHelper.Russian;

            if (!filterContext.HttpContext.Items.ContainsKey("ControllerObject"))
                filterContext.HttpContext.Items.Add("ControllerObject", this);

            base.OnActionExecuting(filterContext);

            CurrentUser = Helpers.UserHelper.CurrentUser(HttpContext);

            if (!HttpContext.Items.ContainsKey("ChildAction"))
            {
                var fullContext = new FullRequestContext(this);
                FullRequestContext.SetCurrent(HttpContext, fullContext);
            }

            if (filterContext.HttpContext.User?.Identity?.IsAuthenticated == true)
            {
                if (CurrentUser == null)
                {
                    filterContext.HttpContext.Session.Clear();
                    filterContext.HttpContext.SignOutAsync().Wait();
                    filterContext.Result = new RedirectResult("~/");
                    return;
                }
            }

            if (filterContext.HttpContext.User?.Identity?.IsAuthenticated == true && !CurrentUser.IsSwitched)
            {
                var control = DependencyInjection.GetServicePersistent<IComponentControl>();

                var message = string.Format(
                    "Пользователь {0} c IP {1}",
                    CurrentUser.Login,
                    FullRequestContext.GetCurrent(HttpContext).Ip);

                control
                    .CreateComponentEvent("Пользователь на сайте", message)
                    .SetImportance(EventImportance.Success)
                    .SetJoinInterval(TimeSpan.FromDays(7))
                    .SetJoinKey(CurrentUser.Login, FullRequestContext.GetCurrent(HttpContext).Ip)
                    .SetProperty("UserAgent", filterContext.HttpContext.Request.Headers["User-Agent"].ToString())
                    .SetProperty("IP", FullRequestContext.GetCurrent(HttpContext).Ip)
                    .Add();
            }
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
        protected BaseController(Guid? userId, bool isSmartBlocksRequest = false)
        {
            ControllerContext = new ControllerContext();

            var fullContext = new FullRequestContext(this);
            FullRequestContext.SetCurrent(HttpContext, fullContext);

            if (userId.HasValue)
            {
                var storage = _storageFactory.GetStorage();
                var user = storage.Users.GetOneById(userId.Value);
                var roles = storage.Roles.GetByUserId(user.Id);

                CurrentUser = Helpers.UserHelper.UserInfoByUser(user, roles);
                fullContext.SetUser(CurrentUser);
            }
        }

        protected BaseController(ILogger logger)
        {
            Logger = logger;
        }

        protected ILogger Logger;

        public JsonResult GetSuccessJsonResponse(object data = null)
        {
            var response = MyJsonHelper.GetSuccessResponse(data);
            return Json(response);
        }

        public JsonResult GetErrorJsonResponse(Exception exception)
        {
            var response = MyJsonHelper.GetErrorResponse(exception);
            return Json(response);
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
                return Request.Headers["Referer"].ToString();
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

        private readonly IStorageFactory _storageFactory = DependencyInjection.GetServicePersistent<IStorageFactory>();

        protected readonly ITimeService TimeService = DependencyInjection.GetServicePersistent<ITimeService>();

        private IStorage _storage;

        public IStorage GetStorage()
        {
            if (_storage == null)
                _storage = _storageFactory.GetStorage();
            return _storage;
        }
    }
}