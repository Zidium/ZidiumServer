using System;

namespace Zidium.Api
{
    public class SendMetricData
    {
        public string Name { get; set; }

        public double? Value { get; set; }

        public TimeSpan? ActualInterval { get; set; }
    }
}
