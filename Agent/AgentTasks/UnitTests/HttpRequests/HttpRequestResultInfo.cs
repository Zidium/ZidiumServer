using System;
using System.Net;
using Zidium.Core.AccountsDb;

namespace Zidium.Agent.AgentTasks.UnitTests.HttpRequests
{
    public class HttpRequestResultInfo
    {
        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public HttpRequestUnitTestRule Rule { get; set; }

        public HttpRequestErrorCode ErrorCode { get; set; }

        public string ErrorMessage { get; set; }

        public HttpWebRequest Request { get; set; }

        public string ResponseHtml { get; set; }

        public string ResponseHeaders { get; set; }

        public HttpStatusCode? HttpStatusCode { get; set; }
    }
}
