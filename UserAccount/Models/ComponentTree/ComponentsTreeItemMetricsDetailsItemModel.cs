using System;
using Zidium.Core.Api;

namespace Zidium.UserAccount.Models.ComponentTree
{
    public class ComponentsTreeItemMetricsDetailsItemModel
    {
        public Guid Id { get; set; }

        public string DisplayName { get; set; }

        public MonitoringStatus Status { get; set; }

        public TimeSpan StatusDuration { get; set; }

        public double? Value { get; set; }
    }
}