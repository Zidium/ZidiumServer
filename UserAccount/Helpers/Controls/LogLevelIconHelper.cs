using System.Web;
using System.Web.Mvc;
using Zidium.Storage;
using Zidium.UserAccount.Helpers;

namespace Zidium.UserAccount
{
    public static class LogLevelIconHelper
    {
        public static string GetLogLevelImageUrl(LogLevel level)
        {
            var url = new UrlHelper(HttpContext.Current.Request.RequestContext);

            if (level == LogLevel.Error || level == LogLevel.Fatal)
                return url.Content("~/Content/Icons/Circle-Red.png");

            if (level == LogLevel.Warning)
                return url.Content("~/Content/Icons/Circle-Yellow.png");

            if (level == LogLevel.Info)
                return url.Content("~/Content/Icons/Circle-Green.png");

            return url.Content("~/Content/Icons/Circle-Gray.png");
        }

        public static MvcHtmlString LogLevelIcon(this HtmlHelper htmlHelper, LogLevel level)
        {
            var img = new TagBuilder("img");
            img.Attributes.Add("src", GetLogLevelImageUrl(level));
            img.Attributes.Add("title", EnumHelper.EnumToString(level));
            return new MvcHtmlString(img.ToString());
        }

    }
}