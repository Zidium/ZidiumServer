using System;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using Zidium.UserAccount.Models;

namespace Zidium.UserAccount
{
    public static class CommonHelper
    {
        public static MvcHtmlString Error(this HtmlHelper htmlHelper, Exception exception)
        {
            if (exception != null)
            {
                var message = HttpUtility.JavaScriptStringEncode(exception.Message);
                var details = HttpUtility.JavaScriptStringEncode(exception.ToString());
                var number = exception.HelpLink;
                var result = new StringBuilder();
                result.Append(@"<script>");
                result.Append(string.Format(@"$(function() {{ showError('{0}', '{1}', '{2}'); }} );", message, details, number));
                result.Append(@"</script>");
                return new MvcHtmlString(result.ToString());
            }
            else
            {
                var result = new StringBuilder();
                result.Append(@"<script>");
                result.Append(@"$(function() { hideError(); } );");
                result.Append(@"</script>");
                return new MvcHtmlString(result.ToString());
            }
        }

        public static MvcHtmlString Error(this HtmlHelper htmlHelper, string messageText, int hash)
        {
            var message = HttpUtility.JavaScriptStringEncode(messageText);
            var result = new StringBuilder();
            result.Append(@"<script>");
            result.Append(string.Format(@"$(function() {{ showError('{0}', '{1}', '{2}'); }} );", message, "", hash));
            result.Append(@"</script>");
            return new MvcHtmlString(result.ToString());
        }

        public static MvcHtmlString MiniHint(this HtmlHelper htmlHelper, string hint)
        {
            var model = new MiniHintModel()
            {
                Hint = hint
            };
            return htmlHelper.Partial("~/Views/Controls/MiniHint.cshtml", model);
        }

    }
}