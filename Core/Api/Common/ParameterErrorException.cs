using Zidium.Api;
using Zidium.Api.Dto;

namespace Zidium.Core.Api
{
    public class ParameterErrorException : ResponseCodeException
    {
        public ParameterErrorException(string message)
            : base(ResponseCode.ParameterError, message)
        {
            
        }
    }
}
