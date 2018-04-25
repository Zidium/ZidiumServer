using System;

namespace Zidium.Api.Dto
{
    public class GetMetricRequestDtoData
    {
        public Guid? ComponentId { get; set; }

        public string Name { get; set; }
    }
}
