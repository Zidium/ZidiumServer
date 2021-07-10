using System;

namespace Zidium.Api.Dto
{
    public class SetComponentDisableRequestDataDto
    {
        public Guid? ComponentId { get; set; }

        public DateTime? ToDate { get; set; }

        public string Comment { get; set; }
    }
}
