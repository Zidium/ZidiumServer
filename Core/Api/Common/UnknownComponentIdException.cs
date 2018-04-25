using System;
using Zidium.Api;
using Zidium.Api.Dto;

namespace Zidium.Core.Api
{
    public class UnknownComponentIdException : ResponseCodeException
    {
        public UnknownComponentIdException(Guid componentId, Guid accountId)
            : base(ResponseCode.UnknownComponentId, "Не удалось найти компонент с ID " + componentId + " в аккаунте " + accountId)
        {
            
        }
    }
}
