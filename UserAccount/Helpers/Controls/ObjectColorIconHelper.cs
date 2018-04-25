using System.Web;
using System.Web.Mvc;
using Zidium.Core.Common;
using Zidium.UserAccount.Helpers;

namespace Zidium.UserAccount
{
    public static class ObjectColorIconHelper
    {
        public static string GetObjectColorImageUrl(ObjectColor color)
        {
            var url = new System.Web.Mvc.UrlHelper(HttpContext.Current.Request.RequestContext);

            if (color == ObjectColor.Red)
                return url.Content("~/Content/Icons/Circle-Red.png");

            if (color == ObjectColor.Yellow)
                return url.Content("~/Content/Icons/Circle-Yellow.png");

            if (color == ObjectColor.Green)
                return url.Content("~/Content/Icons/Circle-Green.png");

            return url.Content("~/Content/Icons/Circle-Gray.png");
        }

        public static MvcHtmlString ObjectColorIcon(this HtmlHelper htmlHelper, ObjectColor color)
        {
            var img = new TagBuilder("img");
            img.Attributes.Add("src", GetObjectColorImageUrl(color));
            img.Attributes.Add("title", EnumHelper.EnumToString(color));
            return new MvcHtmlString(img.ToString());
        }

    }
}