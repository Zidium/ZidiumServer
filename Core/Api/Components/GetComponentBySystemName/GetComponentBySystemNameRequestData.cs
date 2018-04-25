using System;

namespace Zidium.Core.Api
{
    public class GetComponentBySystemNameRequestData
    {
        public Guid? ParentId { get; set; }

        public string SystemName { get; set; }
    }
}
