using System;
using Zidium.Api;
using Zidium.Api.Dto;

namespace Zidium.Core.Api
{
    public class CantUpdateSystemObjectException : ResponseCodeException
    {
        public CantUpdateSystemObjectException()
            : base(ResponseCode.CantUpdateSystemObject, "Нельзя изменять системный объект")
        {
            
        }
    }
}
