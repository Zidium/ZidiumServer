using System;

namespace Zidium.Core.Api
{
    public class GetChangedWebLogConfigsRequestData
    {
        public DateTime? LastUpdateDate { get; set; }

        public Guid[] ComponentIds { get; set; }
    }
}
