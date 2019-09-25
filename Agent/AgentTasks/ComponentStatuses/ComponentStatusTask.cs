using System;

namespace Zidium.Agent.AgentTasks.ComponentStatuses
{
    public class ComponentStatusTask : AgentTaskBase
    {
        public ComponentStatusTask()
        {
            ExecutionPeriod = TimeSpan.FromHours(5);
        }

        protected override AgentTaskResult Do()
        {
            var processor = new ComponentStatusProcessor(Logger, CancellationToken);
            processor.Process();

            var result = string.Format("Обновлено {0} статусов", processor.UpdateStatesCount);
            return GetResult(processor.DbProcessor, result);
        }
    }
}
