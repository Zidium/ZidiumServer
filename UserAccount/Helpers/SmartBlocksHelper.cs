using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace Zidium.UserAccount
{
    public static class SmartBlocksHelper
    {
        public static MvcForm BeginSmartForm(this HtmlHelper htmlHelper, SmartBlocksFormOptions options, object htmlAttributes = null)
        {
            var attributes = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);

            var className = (string) attributes["class"];
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


        /// <summary>
        /// Определение, что запрос отправлен через smart-blocks и требует соответствующего формата ответа
        /// </summary>
        public static bool IsSmartBlocksRequest(this HttpRequestBase request)
        {
            if (request == null)
                throw new ArgumentNullException("request");

            return request.Headers["SmartBlocksRequest"] == "true";
        }

        /// <summary>
        /// Определение, что запрос отправлен через smart-blocks и требует соответствующего формата ответа
        /// </summary>
        public static bool IsSmartBlocksRequest(this HttpRequest request)
        {
            if (request == null)
                throw new ArgumentNullException("request");

            return request.Headers["SmartBlocksRequest"] == "true";
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