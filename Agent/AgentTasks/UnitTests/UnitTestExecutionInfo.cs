using Zidium.Core.Api;

namespace Zidium.Agent.AgentTasks
{
    public class UnitTestExecutionInfo : SendUnitTestResultRequestData
    {
        public bool IsNetworkProblem { get; set; }
    }
}
