using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using Zidium.Common;
using Zidium.Core;
using Zidium.UserAccount.Controllers;
using Zidium.UserAccount.Helpers;
using Zidium.UserAccount.Models;
using Zidium.UserAccount.Models.Errors;

namespace Zidium.UserAccount
{
    public class GlobalExceptionFilterAttribute : Attribute, IExceptionFilter
    {
        public GlobalExceptionFilterAttribute(ITempDataDictionaryFactory tempDataDictionaryFactory, ILogger<GlobalExceptionFilterAttribute> logger)
        {
            _tempDataDictionaryFactory = tempDataDictionaryFactory;
            _logger = logger;
        }

        private readonly ITempDataDictionaryFactory _tempDataDictionaryFactory;
        private readonly ILogger _logger;

        public void OnException(ExceptionContext context)
        {
            if (context.HttpContext.Request.IsAjaxRequest() && context.HttpContext.Request.IsSmartBlocksRequest())
            {
                ExceptionHelper.HandleException(context.Exception, _logger);
                context.ExceptionHandled = true;
                context.Result = new JsonResult(MyJsonHelper.GetErrorResponse(context.Exception));
                return;
            }

            string viewName = null;

            if (context.Exception is ObjectNotFoundException)
            {
                viewName = "~/Views/Errors/ObjectNotFoundError.cshtml";
            }
            else if (context.Exception is AccessDeniedException)
            {
                viewName = "~/Views/Errors/AccessDeniedError.cshtml";
            }
            else if (context.Exception is UserFriendlyException)
            {
                viewName = "~/Views/Errors/UserFriendlyError.cshtml";
            }
            else
            {
                viewName = "~/Views/Errors/CommonError.cshtml";
                ExceptionHelper.HandleException(context.Exception, _logger);
            }

            context.ExceptionHandled = true;
            var controllerName = (string)context.RouteData.Values["controller"];
            var actionName = (string)context.RouteData.Values["action"];
            var model = new HandleErrorInfo(actionName, controllerName, context.Exception);

            var result = new ViewResult
            {
                ViewName = viewName,
                ViewData = new ViewDataDictionary<HandleErrorInfo>(new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary()), model),
                TempData = _tempDataDictionaryFactory.GetTempData(context.HttpContext)
            };

            var controller = context.HttpContext.Items["ControllerObject"] as BaseController;
            if (controller != null)
            {
                controller.ViewBag.IsChildAction = context.HttpContext.Items.ContainsKey("ChildAction");
                var fullContext = new FullRequestContext(controller);
                FullRequestContext.SetCurrent(controller.HttpContext, fullContext);
            }

            context.Result = result;
        }
    }
}
