using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Zidium.UserAccount
{
    public static class SmartBlocksHelper
    {
        public static MvcForm BeginSmartForm(this IHtmlHelper htmlHelper, SmartBlocksFormOptions options, object htmlAttributes = null)
        {
            var attributes = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);

            string className = null;
            if (attributes.ContainsKey("class"))
                className = (string)attributes["class"];

            if (!string.IsNullOrEmpty(className))
                className += " ";
            className += "smart-block-form";
            attributes["class"] = className;

            if (!string.IsNullOrEmpty(options.ContainerHtmlId))
                attributes["data-smart-block-container"] = options.ContainerHtmlId;

            if (!string.IsNullOrEmpty(options.LoaderHtmlId))
                attributes["data-smart-block-loader"] = options.LoaderHtmlId;

            return htmlHelper.BeginForm(options.ActionName, options.ControllerName, options.Method, attributes);
        }
    }

    public class SmartBlocksFormOptions
    {
        public string ActionName;
        public string ControllerName;
        public FormMethod Method;
        public string ContainerHtmlId;
        public string LoaderHtmlId;
    }
}