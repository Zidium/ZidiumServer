using System;

namespace Zidium.Api.Dto
{
    public class GetChangedWebLogConfigsRequestDataDto
    {
        public DateTime? LastUpdateDate { get; set; }

        public Guid[] ComponentIds { get; set; }
    }
}
