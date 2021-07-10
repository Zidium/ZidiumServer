using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.DependencyInjection;
using Zidium.Api.Dto;
using Zidium.UserAccount.Helpers;

namespace Zidium.UserAccount
{
    public static class LogLevelIconHelper
    {
        public static string GetLogLevelImageUrl(IUrlHelper urlHelper, LogLevel level)
        {
            if (level == LogLevel.Error || level == LogLevel.Fatal)
                return urlHelper.Content("~/Content/Icons/Circle-Red.png");

            if (level == LogLevel.Warning)
                return urlHelper.Content("~/Content/Icons/Circle-Yellow.png");

            if (level == LogLevel.Info)
                return urlHelper.Content("~/Content/Icons/Circle-Green.png");

            return urlHelper.Content("~/Content/Icons/Circle-Gray.png");
        }

        public static HtmlString LogLevelIcon(this IHtmlHelper htmlHelper, LogLevel level)
        {
            var urlHelperFactory = htmlHelper.ViewContext.HttpContext.RequestServices.GetRequiredService<IUrlHelperFactory>();
            var urlHelper = urlHelperFactory.GetUrlHelper(htmlHelper.ViewContext);
            var img = new TagBuilder("img");
            img.Attributes.Add("src", GetLogLevelImageUrl(urlHelper, level));
            img.Attributes.Add("title", EnumHelper.EnumToString(level));
            return new HtmlString(htmlHelper.HtmlBlock(x => img));
        }

    }
}