using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.DependencyInjection;
using Zidium.Api.Dto;
using Zidium.UserAccount.Helpers;

namespace Zidium.UserAccount
{
    public static class EventImportanceIconHelper
    {
        public static string GetEventImportanceImageUrl(IUrlHelper urlHelper, EventImportance importance)
        {
            if (importance == EventImportance.Alarm)
                return urlHelper.Content("~/Content/Icons/Circle-Red.png");

            if (importance == EventImportance.Warning)
                return urlHelper.Content("~/Content/Icons/Circle-Yellow.png");

            if (importance == EventImportance.Success)
                return urlHelper.Content("~/Content/Icons/Circle-Green.png");

            return urlHelper.Content("~/Content/Icons/Circle-Gray.png");
        }

        public static string EventImportanceIcon(this IHtmlHelper htmlHelper, EventImportance importance)
        {
            var urlHelperFactory = htmlHelper.ViewContext.HttpContext.RequestServices.GetRequiredService<IUrlHelperFactory>();
            var urlHelper = urlHelperFactory.GetUrlHelper(htmlHelper.ViewContext);
            var img = new TagBuilder("img");
            img.Attributes.Add("src", GetEventImportanceImageUrl(urlHelper, importance));
            img.Attributes.Add("title", EnumHelper.EnumToString(importance));
            return htmlHelper.HtmlBlock(x => img);
        }

    }
}