using System;

namespace Zidium.UserAccount.Models.Errors
{
    public class HandleErrorInfo
    {
        public HandleErrorInfo(string actionName, string controllerName, Exception exception)
        {
            ActionName = actionName;
            ControllerName = controllerName;
            Exception = exception;
        }

        public string ActionName { get; set; }

        public string ControllerName { get; set; }

        public Exception Exception { get; set; }
    }
}
