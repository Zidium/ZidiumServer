using System;

namespace Zidium.Agent.AgentTasks.UnitTests.VirusTotal.Client
{
    public class VirusTotalException : Exception
    {
        public VirusTotalException(string message) : base(message)
        {

        }
    }
}
