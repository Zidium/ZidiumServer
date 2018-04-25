using System;
using Zidium.Api;
using Zidium.Api.Dto;

namespace Zidium.Core.Api
{
    public class UnknownEventIdException : ResponseCodeException
    {
        public UnknownEventIdException(Guid eventId)
            : base(ResponseCode.UnknownEventId, "Не удалось найти событие с ID " + eventId)
        {
            
        }
    }
}
