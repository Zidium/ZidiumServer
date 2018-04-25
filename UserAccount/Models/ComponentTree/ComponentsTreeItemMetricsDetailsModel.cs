using System;
using Zidium.Core.Api;

namespace Zidium.UserAccount.Models.ComponentTree
{
    public class ComponentsTreeItemMetricsDetailsModel
    {
        public MonitoringStatus Status { get; set; }

        public ComponentsTreeItemMetricsDetailsItemModel[] Items { get; set; }

        public TimeSpan StatusDuration { get; set; }

        public bool Expanded { get; set; }

        public Guid Id { get; set; }

        public string HtmlId
        {
            get
            {
                return "metrics-" + Id;
            }
        }

    }
}