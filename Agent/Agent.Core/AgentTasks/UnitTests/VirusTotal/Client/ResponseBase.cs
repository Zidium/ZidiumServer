using System;

namespace Zidium.Agent.AgentTasks.UnitTests.VirusTotal.Client
{
    public class ResponseBase
    {
        public Int32? response_code { get; set; }
        public string verbose_msg { get; set; }
    }
}
