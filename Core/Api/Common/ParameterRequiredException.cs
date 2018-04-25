using Zidium.Api;
using Zidium.Api.Dto;

namespace Zidium.Core.Api
{
    public class ParameterRequiredException : ResponseCodeException
    {
        public ParameterRequiredException(string parameterName)
            : base(ResponseCode.RequiredParameterError, "В запросе отсутствует параметр: " + parameterName)
        {
            
        }
    }
}
