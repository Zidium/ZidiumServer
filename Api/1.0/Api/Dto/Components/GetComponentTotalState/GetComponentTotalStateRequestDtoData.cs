using System;

namespace Zidium.Api.Dto
{
    public class GetComponentTotalStateRequestDtoData
    {
        public Guid? ComponentId { get; set; }

        public bool Recalc { get; set; }
    }
}
