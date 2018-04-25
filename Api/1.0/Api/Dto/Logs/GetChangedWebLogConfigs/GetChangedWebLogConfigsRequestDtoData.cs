using System;
using System.Collections.Generic;

namespace Zidium.Api.Dto
{
    public class GetChangedWebLogConfigsRequestDtoData
    {
        public DateTime? LastUpdateDate { get; set; }

        public List<Guid> ComponentIds { get; set; }
    }
}
