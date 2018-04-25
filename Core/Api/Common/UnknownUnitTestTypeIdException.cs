using System;
using Zidium.Api;
using Zidium.Api.Dto;

namespace Zidium.Core.Api
{
    public class UnknownUnitTestTypeIdException : ResponseCodeException
    {
        public UnknownUnitTestTypeIdException(Guid unitTestTypeId)
            : base(ResponseCode.UnknownUnitTestId, "Не удалось найти тип проверки с ID " + unitTestTypeId)
        {
            
        }
    }
}
