using System;

namespace Zidium.Api.Dto
{
    public class GetComponentInternalStateRequestDataDto
    {
        public Guid? ComponentId { get; set; }

        public bool Recalc { get; set; }
    }
}
