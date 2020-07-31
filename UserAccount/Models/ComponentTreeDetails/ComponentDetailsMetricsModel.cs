using System;
using Zidium.Storage;

namespace Zidium.UserAccount.Models.ComponentTreeDetails
{
    public class ComponentDetailsMetricsModel
    {
        public Guid Id { get; set; }

        public bool CanEdit { get; set; }

        public MetricInfo[] Metrics { get; set; }

        public class MetricInfo
        {
            public Guid Id { get; set; }

            public MonitoringStatus Status { get; set; }

            public TimeSpan StatusDuration { get; set; }

            public string Name { get; set; }

            public DateTime LastResultDate { get; set; }

            public double? LastResult { get; set; }

            public DateTime ActualDate { get; set; }

            public TimeSpan ActualInterval { get; set; }

            public bool IsEnabled { get; set; }

            public bool HasSignal { get; set; }
        }
    }
}