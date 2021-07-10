using System;
using System.Collections.Generic;
using System.Text;
using Zidium.Core.AccountsDb;
using Zidium.Storage;

namespace Zidium.Core.Common.Helpers
{
    public static class ComponentHelper
    {
        public static string GetDynamicSystemName(Guid componentId)
        {
            return "Component_" + componentId;
        }

        public static string GetComponentPathHtml(ComponentForRead component, string accountName, IStorage storage)
        {
            var components = GetComponentsForPath(component, storage);

            var html = new StringBuilder();
            foreach (var componentObj in components)
            {
                var componentUrl = UrlHelper.GetComponentUrl(componentObj.Id);
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

        public static string GetComponentPathText(ComponentForRead component, IStorage storage)
        {
            var components = GetComponentsForPath(component, storage);

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

        internal static List<ComponentForRead> GetComponentsForPath(ComponentForRead component, IStorage storage)
        {
            var components = new List<ComponentForRead>();
            var current = component;
            while (true)
            {
                components.Add(current);

                if (current.ParentId == null)
                    break;

                current = storage.Components.GetOneById(current.ParentId.Value);

                // root НЕ включаем в путь, чтобы быть лаконичнее в письмах
                if (current.IsRoot())
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
