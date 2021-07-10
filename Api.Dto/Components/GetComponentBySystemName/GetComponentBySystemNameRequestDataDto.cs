using System;

namespace Zidium.Api.Dto
{
    public class GetComponentBySystemNameRequestDataDto
    {
        public Guid? ParentId { get; set; }

        public string SystemName { get; set; }
    }
}
