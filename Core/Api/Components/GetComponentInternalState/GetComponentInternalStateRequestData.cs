using System;

namespace Zidium.Core.Api
{
    public class GetComponentInternalStateRequestData
    {
        public Guid? ComponentId { get; set; }

        public bool Recalc { get; set; }
    }
}
