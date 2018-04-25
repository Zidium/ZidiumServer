using System;

namespace Zidium.Api
{
    public class GetComponentBySystemNameData
    {
        public Guid? ParentId { get; set; }

        public string SystemName { get; set; }
    }
}
