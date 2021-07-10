using System;

namespace Zidium.Api.Dto
{
    public class ResponseCodeException : Exception
    {
        public int Code { get; protected set; }

        public ResponseCodeException(int code, string message) : base(message)
        {
            Code = code;
        }
    }
}
