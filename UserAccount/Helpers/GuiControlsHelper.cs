using System.Web.Mvc;
using System.Web.Mvc.Html;
using Zidium.Core.AccountsDb;
using Zidium.UserAccount.Models.Controls;

namespace Zidium.UserAccount.Helpers
{
    public static class GuiControlsHelper
    {
        public static MvcHtmlString PartialObjectStatus(this HtmlHelper htmlHelper, ObjectStatusModel model)
        {
            return htmlHelper.Partial("~/Views/Controls/ObjectStatus.cshtml", model);
        }

        public static MvcHtmlString PartialComponentBreadCrumbs(this HtmlHelper htmlHelper, Component component)
        {
            return htmlHelper.Partial("~/Views/Controls/ComponentBreadCrumbs.cshtml", component);
        }

        public static MvcHtmlString PartialUnitTestBreadCrumbs(this HtmlHelper htmlHelper, UnitTest unitTest)
        {
            return htmlHelper.Partial("~/Views/Controls/UnitTestBreadCrumbs.cshtml", unitTest);
        }

        public static MvcHtmlString PartialMetricBreadCrumbs(this HtmlHelper htmlHelper, Metric metric)
        {
            return htmlHelper.Partial("~/Views/Controls/MetricBreadCrumbs.cshtml", metric);
        }
    }
}