using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.DependencyInjection;
using Zidium.Api.Dto;
using Zidium.UserAccount.Helpers;

namespace Zidium.UserAccount
{
    public static class ObjectColorIconHelper
    {
        public static string GetObjectColorImageUrl(IUrlHelper urlHelper, ObjectColor color)
        {
            if (color == ObjectColor.Red)
                return urlHelper.Content("~/Content/Icons/Circle-Red.png");

            if (color == ObjectColor.Yellow)
                return urlHelper.Content("~/Content/Icons/Circle-Yellow.png");

            if (color == ObjectColor.Green)
                return urlHelper.Content("~/Content/Icons/Circle-Green.png");

            return urlHelper.Content("~/Content/Icons/Circle-Gray.png");
        }

        public static HtmlString ObjectColorIcon(this IHtmlHelper htmlHelper, ObjectColor color)
        {
            var urlHelperFactory = htmlHelper.ViewContext.HttpContext.RequestServices.GetRequiredService<IUrlHelperFactory>();
            var urlHelper = urlHelperFactory.GetUrlHelper(htmlHelper.ViewContext);
            var img = new TagBuilder("img");
            img.Attributes.Add("src", GetObjectColorImageUrl(urlHelper, color));
            img.Attributes.Add("title", EnumHelper.EnumToString(color));
            return new HtmlString(htmlHelper.HtmlBlock(x => img));
        }

    }
}