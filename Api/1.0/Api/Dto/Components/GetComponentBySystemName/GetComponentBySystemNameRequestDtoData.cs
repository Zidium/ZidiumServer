using System;

namespace Zidium.Api.Dto
{
    public class GetComponentBySystemNameRequestDtoData
    {
        public Guid? ParentId { get; set; }

        public string SystemName { get; set; }
    }
}
