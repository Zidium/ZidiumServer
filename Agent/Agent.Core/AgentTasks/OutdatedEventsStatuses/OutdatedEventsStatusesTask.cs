using System;
using Zidium.Api.Dto;

namespace Zidium.Agent.AgentTasks.OutdatedEventsStatuses
{
    public class OutdatedEventsStatusesTask : AgentTaskBase
    {
        public OutdatedEventsStatusesTask()
        {
            ExecutionPeriod = TimeSpan.FromMinutes(1);
        }

        protected override AgentTaskResult Do()
        {
            var processor = new OutdatedEventsStatusesProcessor(Logger, CancellationToken);
            processor.Process();

            var result = string.Format("Обновлено {0} статусов событий", processor.Count);
            return new AgentTaskResult(UnitTestResult.Success, result);
        }
    }
}
