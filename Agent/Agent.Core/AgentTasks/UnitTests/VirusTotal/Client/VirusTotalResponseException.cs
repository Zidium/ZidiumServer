using System;
using System.Net;

namespace Zidium.Agent.AgentTasks.UnitTests.VirusTotal.Client
{
    public class VirusTotalResponseException : VirusTotalException
    {
        public HttpStatusCode StatusCode { get; private set; }

        public VirusTotalResponseException(HttpStatusCode statusCode, string message) : base(message)
        {
            StatusCode = statusCode;
        }

        public bool ApiQuotaLimits
        {
            get { return Message.Contains("API quota limits"); }
        }
    }
}
