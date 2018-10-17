using System;

namespace Zidium.Core.Api
{
    public class MetricInfo
    {
        public Guid Id { get; set; }

        public Guid ComponentId { get; set; }

        public string SystemName { get; set; }

        public double? Value { get; set; }

        public DateTime BeginDate { get; set; }

        public DateTime ActualDate { get; set; }

        public MonitoringStatus Status { get; set; }

    }
}
