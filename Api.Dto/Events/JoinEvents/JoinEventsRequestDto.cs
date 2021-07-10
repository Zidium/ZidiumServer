using System.Collections.Generic;

namespace Zidium.Api.Dto
{
    public class JoinEventsRequestDto : RequestDtoT<List<JoinEventRequestDataDto>>
    {
        public JoinEventsRequestDto()
        {
            Data = new List<JoinEventRequestDataDto>();
        }
    }
}
