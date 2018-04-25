using System.Collections.Generic;

namespace Zidium.Core.Api
{
    public class JoinEventsRequest : RequestT<List<JoinEventData>>
    {
        public JoinEventsRequest()
        {
            Data = new List<JoinEventData>();
        }
    }
}
