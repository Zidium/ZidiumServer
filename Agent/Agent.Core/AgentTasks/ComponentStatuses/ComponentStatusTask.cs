using System;
using Zidium.Api.Dto;

namespace Zidium.Agent.AgentTasks.ComponentStatuses
{
    public class ComponentStatusTask : AgentTaskBase
    {
        public ComponentStatusTask()
        {
            ExecutionPeriod = TimeSpan.FromHours(6);
        }

        protected override AgentTaskResult Do()
        {
            var processor = new ComponentStatusProcessor(Logger, CancellationToken);
            var error = processor.Process();

            var result = string.Format("Обновлено {0} статусов", processor.UpdateStatesCount);
            return error == null ?
                new AgentTaskResult(UnitTestResult.Success, result) :
                new AgentTaskResult(UnitTestResult.Warning, error);
        }
    }
}
