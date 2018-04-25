using System.Web.Mvc;
using Zidium.UserAccount.Helpers;
using Zidium.UserAccount.Models;

namespace Zidium.UserAccount
{
    public abstract class PermissionAttribute : FilterAttribute, IActionFilter
    {
        public abstract bool CheckPermissions();

        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!CheckPermissions())
            {
                var exception = new NoAccessToPageException();

                if (!filterContext.HttpContext.Request.IsAjaxRequest())
                {
                    var controllerName = (string) filterContext.RouteData.Values["controller"];
                    var actionName = (string) filterContext.RouteData.Values["action"];
                    var model = new HandleErrorInfo(exception, controllerName, actionName);

                    filterContext.Result = new ViewResult
                    {
                        ViewName = "~/Views/Errors/AccessDeniedError.cshtml",
                        ViewData = new ViewDataDictionary<HandleErrorInfo>(model),
                        TempData = filterContext.Controller.TempData
                    };

                }
                else
                {
                    var response = MyJsonHelper.GetErrorResponse(exception);
                    filterContext.Result = new JsonResult()
                    {
                        Data = response,
                        JsonRequestBehavior = JsonRequestBehavior.AllowGet
                    };
                }
            }
        }

        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
        }
    }
}