using System;

namespace Zidium.Api.Dto
{
    public class GetMetricRequestDataDto
    {
        public Guid? ComponentId { get; set; }

        public string Name { get; set; }
    }
}
