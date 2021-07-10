using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.DependencyInjection;
using Zidium.Api.Dto;
using Zidium.UserAccount.Helpers;

namespace Zidium.UserAccount
{
    public static class MonitoringStatusIconHelper
    {
        public static string GetMonitoringStatusImageUrl(IUrlHelper urlHelper, MonitoringStatus status)
        {
            if (status == MonitoringStatus.Alarm)
                return urlHelper.Content("~/Content/Icons/Circle-Red.png");

            if (status == MonitoringStatus.Warning)
                return urlHelper.Content("~/Content/Icons/Circle-Yellow.png");

            if (status == MonitoringStatus.Success)
                return urlHelper.Content("~/Content/Icons/Circle-Green.png");

            if (status == MonitoringStatus.Disabled)
                return urlHelper.Content("~/Content/Icons/Circle-Disabled.png");

            return urlHelper.Content("~/Content/Icons/Circle-Gray.png");
        }

        public static HtmlString MonitoringStatusIcon(this IHtmlHelper htmlHelper, MonitoringStatus status)
        {
            var urlHelperFactory = htmlHelper.ViewContext.HttpContext.RequestServices.GetRequiredService<IUrlHelperFactory>();
            var urlHelper = urlHelperFactory.GetUrlHelper(htmlHelper.ViewContext);
            var img = new TagBuilder("img");
            img.Attributes.Add("src", GetMonitoringStatusImageUrl(urlHelper, status));
            img.Attributes.Add("title", EnumHelper.EnumToString(status));
            img.AddCssClass("img-status-icon");
            return new HtmlString(htmlHelper.HtmlBlock(x => img));
        }

    }
}