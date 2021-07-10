using System;
using Zidium.Api.Dto;

namespace Zidium.Api
{
    public class ResponseInfo
    {
        public ResponseDto Response { get; protected set; }

        public DateTime Date { get; protected set; }

        public string RequestName { get; protected set; }

        public ResponseInfo(string action, ResponseDto response)
        {
            RequestName = action;
            Response = response;
            Date = DateTime.Now;
        }
    }
}
