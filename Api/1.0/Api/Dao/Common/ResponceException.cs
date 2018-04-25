using System;

namespace Zidium.Api
{
    public class ResponseException: Exception
    {
        public Response Response { get; protected set; }

        public ResponseException(Response response):base(response.ErrorMessage)
        {
            Response = response;
        }

        public ResponseException(Response response, Exception innerException)
            : base(response.ErrorMessage, innerException)
        {
            Response = response;
        }
    }
}
