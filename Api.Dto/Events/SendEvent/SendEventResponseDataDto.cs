using System;

namespace Zidium.Api.Dto
{
    public class SendEventResponseDataDto
    {
        public Guid EventId { get; set; }

        public Guid EventTypeId { get; set; }
    }
}
