using Microsoft.AspNetCore.Routing;

namespace Zidium.UserAccount.Helpers
{
    public class ControllerActionUrlData
    {
        public string ControllerName { get; private set; }
        public string ActionName { get; private set; }
        public RouteValueDictionary RouteValues { get; private set; }

        public ControllerActionUrlData(string controller, string action, RouteValueDictionary routeValues)
        {
            ControllerName = controller;
            ActionName = action;
            RouteValues = routeValues;
        }
    }
}