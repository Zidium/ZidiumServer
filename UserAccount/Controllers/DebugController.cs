using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Zidium.Api;
using Zidium.Api.Dto;

namespace Zidium.UserAccount.Controllers
{
    [Authorize]
    public class DebugController : BaseController
    {
        public DebugController(IComponentControl componentControl, ILogger<DebugController> logger) : base(logger)
        {
            _componentControl = componentControl;
        }

        private readonly IComponentControl _componentControl;

        [HttpPost]
        public IActionResult LogJsError(
            string pageUrl,
            string message,
            string scriptUrl,
            string line,
            string column,
            string error,
            string stack)
        {
            // Log by logger
            var control = _componentControl;
            if (control == null || control.IsFake())
            {
                return new JsonResult(null);
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

            return new JsonResult(null);
        }
    }
}