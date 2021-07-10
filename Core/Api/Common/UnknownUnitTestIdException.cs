using System;
using Zidium.Api;
using Zidium.Api.Dto;

namespace Zidium.Core.Api
{
    public class UnknownUnitTestIdException : ResponseCodeException
    {
        public UnknownUnitTestIdException(Guid unitTestId)
            : base(ResponseCode.UnknownUnitTestId, "Не удалось найти проверку с ID " + unitTestId)
        {
            
        }
    }
}
