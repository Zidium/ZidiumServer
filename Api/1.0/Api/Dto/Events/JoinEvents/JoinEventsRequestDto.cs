using System.Collections.Generic;

namespace Zidium.Api.Dto
{
    public class JoinEventsRequestDto : RequestT<List<JoinEventDto>>
    {
        public JoinEventsRequestDto()
        {
            Data = new List<JoinEventDto>();
        }
    }
}
