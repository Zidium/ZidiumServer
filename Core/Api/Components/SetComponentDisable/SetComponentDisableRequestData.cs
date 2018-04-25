using System;

namespace Zidium.Core.Api
{
    public class SetComponentDisableRequestData
    {
        public Guid? ComponentId { get; set; }

        public DateTime? ToDate { get; set; }

        public string Comment { get; set; }
    }
}
