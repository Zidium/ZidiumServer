using System;
using System.Text;
using System.Web;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Zidium.UserAccount.Models;

namespace Zidium.UserAccount
{
    public static class CommonHelper
    {
        public static IHtmlContent Error(this IHtmlHelper htmlHelper, Exception exception)
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
                return new HtmlString(result.ToString());
            }
            else
            {
                var result = new StringBuilder();
                result.Append(@"<script>");
                result.Append(@"$(function() { hideError(); } );");
                result.Append(@"</script>");
                return new HtmlString(result.ToString());
            }
        }

        public static IHtmlContent Error(this IHtmlHelper htmlHelper, string messageText, int hash)
        {
            var message = HttpUtility.JavaScriptStringEncode(messageText);
            var result = new StringBuilder();
            result.Append(@"<script>");
            result.Append(string.Format(@"$(function() {{ showError('{0}', '{1}', '{2}'); }} );", message, "", hash));
            result.Append(@"</script>");
            return new HtmlString(result.ToString());
        }

        public static IHtmlContent MiniHint(this IHtmlHelper htmlHelper, string hint)
        {
            var model = new MiniHintModel()
            {
                Hint = hint
            };
            return htmlHelper.Partial("~/Views/Controls/MiniHint.cshtml", model);
        }

    }
}