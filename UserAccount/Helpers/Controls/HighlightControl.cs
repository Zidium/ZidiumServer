using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;

namespace Zidium.UserAccount
{
    public static class HighlightControl
    {
        public static MvcHtmlString Highlight(this HtmlHelper htmlHelper, string text, string value)
        {
            if (string.IsNullOrEmpty(text) || string.IsNullOrEmpty(value))
                return new MvcHtmlString(htmlHelper.Encode(text));

            var list = Regex.Split(text, Regex.Escape(value), RegexOptions.IgnoreCase);
            var encodedValue = htmlHelper.Encode(value);
            var html = string.Join("<span class='text-highlight'>" + encodedValue + "</span>", list.Select(t => htmlHelper.Encode(t)));
            
            return new MvcHtmlString(html);
        }
    }
}