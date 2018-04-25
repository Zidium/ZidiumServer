using System;

namespace Zidium.Api.Dto
{
    public class MetricDto
    {
        public Guid ComponentId { get; set; }

        public string Name { get; set; }

        public double? Value { get; set; }

        public DateTime BeginDate { get; set; }

        public DateTime ActualDate { get; set; }

        public MonitoringStatus Status { get; set; }

    }
}
