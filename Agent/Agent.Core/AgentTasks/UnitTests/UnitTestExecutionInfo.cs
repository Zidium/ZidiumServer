using System;
using Zidium.Api.Dto;

namespace Zidium.Agent.AgentTasks
{
    public class UnitTestExecutionInfo
    {
        public bool IsNetworkProblem { get; set; }

        public DateTime? NextStepProcessTime { get; set; }

        public SendUnitTestResultRequestDataDto ResultRequest { get; set; }
    }
}
