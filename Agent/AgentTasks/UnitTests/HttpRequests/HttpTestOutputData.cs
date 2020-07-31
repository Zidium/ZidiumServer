using System;
using System.Collections.Generic;
using System.Net;
using Zidium.Storage;

namespace Zidium.Agent.AgentTasks.UnitTests.HttpRequests
{
    public class HttpTestOutputData
    {
        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public HttpRequestErrorCode ErrorCode { get; set; }

        public string ErrorMessage { get; set; }

        public string ResponseHtml { get; set; }

        public string ResponseHeaders { get; set; }

        public HttpStatusCode? HttpStatusCode { get; set; }

        public bool IsNetworkProblem { get; set; }

        public List<Exception> Exceptions { get; set; }

        public HttpTestOutputData()
        {
            Exceptions = new List<Exception>();
        }
    }
}
