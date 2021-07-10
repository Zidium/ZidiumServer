using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Zidium.Api;
using Zidium.Api.Dto;

namespace Zidium.UserAccount.Controllers
{
    [Authorize]
    public class DebugController : BaseController
    {
        public DebugController(IComponentControl componentControl)
        {
            _componentControl = componentControl;
        }

        private readonly IComponentControl _componentControl;

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
            var control = _componentControl;
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
                .SetImportance(EventImportance.Warning)
                .Add();

            return null;
        }
    }
}