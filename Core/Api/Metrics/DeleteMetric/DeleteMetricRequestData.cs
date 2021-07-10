using System;

namespace Zidium.Core.Api
{
    public class DeleteMetricRequestData
    {
        public Guid? MetricId { get; set; }

        public bool? UpdateComponentStatus { get; set; }
    }
}
