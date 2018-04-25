using System;

namespace Zidium.Core.Api
{
    public class GetMetricRequestData
    {
        public Guid? ComponentId { get; set; }

        public string Name { get; set; }
    }
}
