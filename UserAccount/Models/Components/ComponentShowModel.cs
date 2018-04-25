using System;
using Zidium.Core.AccountsDb;
using Zidium.UserAccount.Helpers;

namespace Zidium.UserAccount.Models
{
    public class ComponentShowModel
    {
        public ComponentShowModel()
        {
            LogConfig = new LogConfig();
        }

        public Component Component { get; set; }

        public LogConfig LogConfig { get; set; }

        public Bulb ExternalState { get; set; }

        public Bulb InternalState { get; set; }

        public Metric[] Metrics { get; set; }

        public UnitTest[] UnitTests { get; set; }

        public Component[] Childs { get; set; }

        public ComponentMiniStatusModel EventsMiniStatus { get; set; }

        public ComponentMiniStatusModel UnittestsMiniStatus { get; set; }

        public ComponentMiniStatusModel MetricsMiniStatus { get; set; }

        public ComponentMiniStatusModel ChildsMiniStatus { get; set; }

        public string GetLogUrl(TimeSpan period)
        {
            var fromDate = DateTime.Now - period;
            return LinkHelper.GenerateUrl("Index", "Logs", new { ComponentId = Component.Id, Date = GuiHelper.GetUrlDateTimeString(fromDate) });
        }
    }
}