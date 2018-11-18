using System;
using System.Linq;
using System.Web.Routing;

namespace Zidium.UserAccount.Helpers
{
    public class ControllerActionUrlParser
    {
        private static string GetOneParameter(string url)
        {
            int index = url.LastIndexOf('/');
            string parameter = url.Substring(index + 1);
            return parameter;
        }

        public static string GetControllerName(string url)
        {
            string[] units = url.Split('/');
            string name = units.FirstOrDefault(x => string.IsNullOrWhiteSpace(x) == false);
            if (name == null)
            {
                throw new Exception("controller name not found: " + url);
            }
            return name;
        }

        public static string GetActionName(string url, string controllerName)
        {
            int index = url.IndexOf(controllerName + "/");
            if (index < 1)
            {
                throw new Exception("controller action not found: " + url);
            }
            string action = url.Substring(index + controllerName.Length + 1);
            index = action.IndexOf('/');
            if (index >= 0)
            {
                return action.Substring(0, index);
            }
            index = action.IndexOf('?');
            if (index >= 0)
            {
                return action.Substring(0, index);
            }
            return action;
        }

        public static string GetRelativeUrl(string url)
        {
            int index = url.IndexOf('/');
            if (index > 0)
            {
                return url.Substring(index);
            }
            return url;
        }

        public static ControllerActionUrlData Parse(string url)
        {
            // сейчас реализован только один вид url-ов: /controller/action/id

            url = GetRelativeUrl(url);

            string controller = GetControllerName(url);
            string action = GetActionName(url, controller);
            string parameter = GetOneParameter(url);

            RouteValueDictionary values = new RouteValueDictionary();
            values.Add("id", parameter);

            return new ControllerActionUrlData(controller, action, values);
        }
    }
}