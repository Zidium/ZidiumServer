using System;
using System.Collections.Generic;
using System.Text;
using Zidium.Core.AccountsDb;

namespace Zidium.Core.Common.Helpers
{
    public static class ComponentHelper
    {
        public static string GetDynamicSystemName(Guid componentId)
        {
            return "Component_" + componentId;
        }

        public static string GetComponentPathHtml(Component component, string accountName)
        {
            var components = GetComponentsForPath(component);

            var html = new StringBuilder();
            foreach (var componentObj in components)
            {
                var componentUrl = UrlHelper.GetComponentUrl(componentObj.Id, accountName);
                var link = HtmlRender.GetLinkHtml(componentUrl, componentObj.DisplayName);
                html.Append(link);

                // ко всем кроме последнего добавляем разделитель
                if (component != componentObj)
                {
                    html.Append("<span>/<span>");
                }
            }
            return html.ToString();
        }

        public static string GetComponentPathText(Component component)
        {
            var components = GetComponentsForPath(component);

            var text = new StringBuilder();
            foreach (var componentObj in components)
            {
                text.Append(componentObj.DisplayName);

                // ко всем кроме последнего добавляем разделитель
                if (component != componentObj)
                {
                    text.Append("/");
                }
            }
            return text.ToString();
        }

        internal static List<Component> GetComponentsForPath(Component component)
        {
            var components = new List<Component>();
            var current = component;
            while (true)
            {
                components.Add(current);
                current = current.Parent;

                // root НЕ включаем в путь, чтобы быть лаконичнее в письмах
                if (current == null || current.IsRoot)
                {
                    break;
                }
            }
            components.Reverse();
            return components;
        }

        public static string GetSystemNameByHost(string host)
        {
            if (host == null)
            {
                throw new ArgumentNullException("host");
            }

            if (string.IsNullOrWhiteSpace(host))
            {
                throw new ArgumentException("host is empty");
            }
            return "Host_" + host;
        }

        public static string GetDisplayNameByHost(string host)
        {
            if (host == null)
            {
                throw new ArgumentNullException("host");
            }

            if (string.IsNullOrWhiteSpace(host))
            {
                throw new ArgumentException("host is empty");
            }
            return host;
        }
    }
}
