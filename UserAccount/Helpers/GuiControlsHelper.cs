using System;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using Zidium.Storage;
using Zidium.UserAccount.Models;
using Zidium.UserAccount.Models.Controls;

namespace Zidium.UserAccount.Helpers
{
    public static class GuiControlsHelper
    {
        public static MvcHtmlString PartialObjectStatus(this HtmlHelper htmlHelper, ObjectStatusModel model)
        {
            return htmlHelper.Partial("~/Views/Controls/ObjectStatus.cshtml", model);
        }

        public static MvcHtmlString PartialComponentBreadCrumbs(this HtmlHelper htmlHelper, Guid componentId, IStorage storage)
        {
            var model = ComponentBreadCrumbsModel.Create(componentId, storage);
            return htmlHelper.Partial("~/Views/Controls/ComponentBreadCrumbs.cshtml", model);
        }

        public static MvcHtmlString PartialUnitTestBreadCrumbs(this HtmlHelper htmlHelper, Guid unitTestId, IStorage storage)
        {
            var model = UnitTestBreadCrumbsModel.Create(unitTestId, storage);
            return htmlHelper.Partial("~/Views/Controls/UnitTestBreadCrumbs.cshtml", model);
        }

        public static MvcHtmlString PartialMetricBreadCrumbs(this HtmlHelper htmlHelper, Guid metricId, IStorage storage)
        {
            var model = MetricBreadCrumbsModel.Create(metricId, storage);
            return htmlHelper.Partial("~/Views/Controls/MetricBreadCrumbs.cshtml", model);
        }
    }
}