using Zidium.Api;

namespace Zidium.Agent.AgentTasks
{
    public class AgentTaskResult
    {
        public UnitTestResult Status { get; protected set; }

        public string Message { get; protected set; }

        public AgentTaskResult(UnitTestResult status, string message)
        {
            Message = message;
            Status = status;
        }
    }
}
