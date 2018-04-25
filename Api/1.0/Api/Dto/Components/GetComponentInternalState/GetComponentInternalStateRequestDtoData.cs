using System;

namespace Zidium.Api.Dto
{
    public class GetComponentInternalStateRequestDtoData
    {
        public Guid? ComponentId { get; set; }

        public bool Recalc { get; set; }
    }
}
