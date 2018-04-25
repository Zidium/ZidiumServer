using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace Zidium.UserAccount
{
    public static class ScrollToMeControl
    {
        public static MvcHtmlString ScrollToMe(this HtmlHelper htmlHelper, int? offset = null)
        {
            return htmlHelper.Partial("~/Views/Controls/ScrollToMe/ScrollToMe.cshtml", offset);
        }
    }
}