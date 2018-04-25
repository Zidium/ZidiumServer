using System;
using Zidium.Api;
using Zidium.Api.Dto;

namespace Zidium.Core.Api
{
    public class UnknownComponentTypeIdException : ResponseCodeException
    {
        public UnknownComponentTypeIdException(Guid componentTypeId)
            : base(ResponseCode.UnknownComponentTypeId, "Не удалось найти тип компонента с ID " + componentTypeId)
        {
            
        }
    }
}
