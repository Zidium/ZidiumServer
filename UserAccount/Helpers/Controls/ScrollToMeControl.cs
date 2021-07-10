using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Zidium.UserAccount
{
    public static class ScrollToMeControl
    {
        public static IHtmlContent ScrollToMe(this IHtmlHelper htmlHelper, int? offset = null)
        {
            return htmlHelper.Partial("~/Views/Controls/ScrollToMe/ScrollToMe.cshtml", offset);
        }
    }
}