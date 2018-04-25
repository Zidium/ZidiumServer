using System.Web;
using System.Web.Mvc;
using Zidium.Core.Api;
using Zidium.UserAccount.Helpers;

namespace Zidium.UserAccount
{
    public static class MonitoringStatusIconHelper
    {
        public static string GetMonitoringStatusImageUrl(MonitoringStatus status)
        {
            var url = new UrlHelper(HttpContext.Current.Request.RequestContext);

            if (status == MonitoringStatus.Alarm)
                return url.Content("~/Content/Icons/Circle-Red.png");

            if (status == MonitoringStatus.Warning)
                return url.Content("~/Content/Icons/Circle-Yellow.png");

            if (status == MonitoringStatus.Success)
                return url.Content("~/Content/Icons/Circle-Green.png");

            if (status == MonitoringStatus.Disabled)
                return url.Content("~/Content/Icons/Circle-Disabled.png");

            return url.Content("~/Content/Icons/Circle-Gray.png");
        }

        public static MvcHtmlString MonitoringStatusIcon(this HtmlHelper htmlHelper, MonitoringStatus status)
        {
            var img = new TagBuilder("img");
            img.Attributes.Add("src", GetMonitoringStatusImageUrl(status));
            img.Attributes.Add("title", EnumHelper.EnumToString(status));
            img.AddCssClass("img-status-icon");
            return new MvcHtmlString(img.ToString());
        }

    }
}