using System;

namespace Zidium.Api.Dto
{
    public class SendMetricRequestDtoData
    {
        public Guid? ComponentId { get; set; }

        public string Name { get; set; }

        public double? Value { get; set; }

        public double? ActualIntervalSecs { get; set; }
    }
}
