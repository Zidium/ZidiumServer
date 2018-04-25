using System;

namespace Zidium.Core.Api
{
    public class SetMetricDisableRequestData
    {
        public Guid MetricId { get; set; }

        public DateTime? ToDate { get; set; }

        public string Comment { get; set; }
    }
}
