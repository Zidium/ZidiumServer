using System;
using System.Collections.Generic;

namespace Zidium.Core.Api
{
    public class GetChangedWebLogConfigsRequestData
    {
        public DateTime? LastUpdateDate { get; set; }

        public List<Guid> ComponentIds { get; set; }
    }
}
