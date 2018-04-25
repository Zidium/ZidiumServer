using System;

namespace Zidium.Core.Api
{
    public class GetComponentTotalStateRequestData
    {
        public Guid? ComponentId { get; set; }

        public bool Recalc { get; set; }
    }
}
