using System;
using Zidium.Core.Api;

namespace Zidium.Agent.AgentTasks
{
    public class UnitTestExecutionInfo 
    {
        public bool IsNetworkProblem { get; set; }

        public DateTime? NextStepProcessTime { get; set; }

        public SendUnitTestResultRequestData ResultRequest { get; set; }
    }
}
