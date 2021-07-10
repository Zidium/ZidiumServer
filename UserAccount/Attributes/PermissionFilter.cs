using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Zidium.UserAccount.Helpers;
using Zidium.UserAccount.Models;
using Zidium.UserAccount.Models.Errors;

namespace Zidium.UserAccount
{
    public abstract class PermissionFilter : IActionFilter
    {
        public PermissionFilter(ITempDataDictionaryFactory tempDataDictionaryFactory)
        {
            _tempDataDictionaryFactory = tempDataDictionaryFactory;
        }

        private readonly ITempDataDictionaryFactory _tempDataDictionaryFactory;

        public abstract bool CheckPermissions(HttpContext httpContext);

        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!CheckPermissions(filterContext.HttpContext))
            {
                var exception = new NoAccessToPageException();

                if (!filterContext.HttpContext.Request.IsAjaxRequest())
                {
                    var controllerName = (string)filterContext.RouteData.Values["controller"];
                    var actionName = (string)filterContext.RouteData.Values["action"];
                    var model = new HandleErrorInfo(actionName, controllerName, exception);

                    filterContext.Result = new ViewResult
                    {
                        ViewName = "~/Views/Errors/AccessDeniedError.cshtml",
                        ViewData = new ViewDataDictionary<HandleErrorInfo>(new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary()), model),
                        TempData = _tempDataDictionaryFactory.GetTempData(filterContext.HttpContext)
                    };

                }
                else
                {
                    var response = MyJsonHelper.GetErrorResponse(exception);
                    filterContext.Result = new JsonResult(response);
                }
            }
        }

        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
        }
    }
}