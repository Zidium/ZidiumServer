using System;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Zidium.Storage;
using Zidium.UserAccount.Models;
using Zidium.UserAccount.Models.Controls;

namespace Zidium.UserAccount.Helpers
{
    public static class GuiControlsHelper
    {
        public static IHtmlContent PartialObjectStatus(this IHtmlHelper htmlHelper, ObjectStatusModel model)
        {
            return htmlHelper.Partial("~/Views/Controls/ObjectStatus.cshtml", model);
        }

        public static IHtmlContent PartialComponentBreadCrumbs(this IHtmlHelper htmlHelper, Guid componentId, IStorage storage)
        {
            var model = ComponentBreadCrumbsModel.Create(componentId, storage);
            return htmlHelper.Partial("~/Views/Controls/ComponentBreadCrumbs.cshtml", model);
        }

        public static IHtmlContent PartialUnitTestBreadCrumbs(this IHtmlHelper htmlHelper, Guid unitTestId, IStorage storage)
        {
            var model = UnitTestBreadCrumbsModel.Create(unitTestId, storage);
            return htmlHelper.Partial("~/Views/Controls/UnitTestBreadCrumbs.cshtml", model);
        }

        public static IHtmlContent PartialMetricBreadCrumbs(this IHtmlHelper htmlHelper, Guid metricId, IStorage storage)
        {
            var model = MetricBreadCrumbsModel.Create(metricId, storage);
            return htmlHelper.Partial("~/Views/Controls/MetricBreadCrumbs.cshtml", model);
        }
    }
}