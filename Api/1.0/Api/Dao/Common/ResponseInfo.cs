using System;

namespace Zidium.Api
{
    public class ResponseInfo
    {
        public Response Response { get; protected set; }

        public DateTime Date { get; protected set; }

        public string RequestName { get; protected set; }

        public ResponseInfo(string action, Response response)
        {
            RequestName = action;
            Response = response;
            Date = DateTime.Now;
        }
    }
}
