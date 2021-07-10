using System;

namespace Zidium.Api.Dto
{
    public class GetComponentTotalStateRequestDataDto
    {
        public Guid? ComponentId { get; set; }

        public bool Recalc { get; set; }
    }
}
