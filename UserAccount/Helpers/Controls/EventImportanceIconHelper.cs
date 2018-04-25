using System.Web;
using System.Web.Mvc;
using Zidium.Core.Api;
using Zidium.UserAccount.Helpers;

namespace Zidium.UserAccount
{
    public static class EventImportanceIconHelper
    {
        public static string GetEventImportanceImageUrl(EventImportance importance)
        {
            var url = new UrlHelper(HttpContext.Current.Request.RequestContext);

            if (importance == EventImportance.Alarm)
                return url.Content("~/Content/Icons/Circle-Red.png");

            if (importance == EventImportance.Warning)
                return url.Content("~/Content/Icons/Circle-Yellow.png");

            if (importance == EventImportance.Success)
                return url.Content("~/Content/Icons/Circle-Green.png");

            return url.Content("~/Content/Icons/Circle-Gray.png");
        }

        public static MvcHtmlString EventImportanceIcon(this HtmlHelper htmlHelper, EventImportance importance)
        {
            var img = new TagBuilder("img");
            img.Attributes.Add("src", GetEventImportanceImageUrl(importance));
            img.Attributes.Add("title", EnumHelper.EnumToString(importance));
            return new MvcHtmlString(img.ToString());
        }

    }
}