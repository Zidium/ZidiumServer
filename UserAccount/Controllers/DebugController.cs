using System.Web.Mvc;

namespace Zidium.UserAccount.Controllers
{
    [Authorize]
    public class DebugController : BaseController
    {
        [ValidateInput(false)]
        [HttpPost]
        public ActionResult LogJsError(
            string pageUrl,
            string message, 
            string scriptUrl, 
            string line,
            string column, 
            string error, 
            string stack)
        {
            var control = MvcApplication.ComponentControl;
            if (control == null || control.IsFake())
            {
                return null;
            }

            var type = string.Format(
                "js error: {0} (source: {1}, line: {2})",
                message,
                scriptUrl,
                line);

            control.CreateApplicationError(type)
                .SetProperty("PageUrl", pageUrl)
                .SetProperty("ScriptUrl", scriptUrl)
                .SetProperty("Line", line)
                .SetProperty("Column", column)
                .SetProperty("Error", error)
                .SetProperty("Stack", stack)
                .SetImportance(Api.EventImportance.Warning)
                .Add();

            return null;
        }
    }
}