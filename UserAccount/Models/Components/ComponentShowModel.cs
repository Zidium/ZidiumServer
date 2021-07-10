using System;
using Microsoft.AspNetCore.Mvc;
using Zidium.Storage;
using Zidium.UserAccount.Helpers;

namespace Zidium.UserAccount.Models
{
    public class ComponentShowModel
    {
        public DateTime Now;

        public ComponentBreadCrumbsModel ComponentBreadCrumbs { get; set; }

        public ComponentForRead Component { get; set; }

        public ComponentForRead Parent { get; set; }

        public ComponentTypeForRead ComponentType { get; set; }

        public ComponentPropertyForRead[] Properties;

        public LogConfigForRead LogConfig { get; set; }

        public BulbForRead ExternalState { get; set; }

        public GetGuiComponentShowInfo.MetricInfo[] Metrics { get; set; }

        public GetGuiComponentShowInfo.UnitTestInfo[] UnitTests { get; set; }

        public GetGuiComponentShowInfo.ChildInfo[] Childs { get; set; }

        public ComponentMiniStatusModel EventsMiniStatus { get; set; }

        public ComponentMiniStatusModel UnittestsMiniStatus { get; set; }

        public ComponentMiniStatusModel MetricsMiniStatus { get; set; }

        public ComponentMiniStatusModel ChildsMiniStatus { get; set; }

        public string GetLogUrl(IUrlHelper urlHelper, TimeSpan period)
        {
            var fromDate = DateTime.Now - period;
            return LinkHelper.GenerateUrl(urlHelper, "Index", "Logs", new { ComponentId = Component.Id, Date = GuiHelper.GetUrlDateTimeString(fromDate) });
        }
    }
}