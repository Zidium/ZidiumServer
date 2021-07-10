using System;

namespace Zidium.Api.Dto
{
    public class ResponseException : Exception
    {
        public ResponseDto Response { get; protected set; }

        public ResponseException(ResponseDto response) : base(response.ErrorMessage)
        {
            Response = response;
        }

        public ResponseException(ResponseDto response, Exception innerException)
            : base(response.ErrorMessage, innerException)
        {
            Response = response;
        }
    }
}
