using System;
using Zidium.Api.Dto;

namespace Zidium.Agent.AgentTasks.DeleteEvents
{
    public class DeleteUnittestEventsTask : AgentTaskBase
    {
        public DeleteUnittestEventsTask()
        {
            ExecutionPeriod = TimeSpan.FromHours(1);
            WaitOnErrorTime = TimeSpan.FromHours(1);
        }

        protected override AgentTaskResult Do()
        {
            var maxDeleteCount = AgentConfiguration.EventsMaxDeleteCount;

            var processor = new DeleteUnittestEventsProcessor(Logger, CancellationToken);

            if (maxDeleteCount.HasValue)
                processor.MaxDeleteCount = maxDeleteCount.Value;

            processor.Process();

            var result = string.Format(
                "Удалено событий: {0}; удалено свойств: {1}",
                processor.DeletedEventsCount,
                processor.DeletedPropertiesCount);

            return new AgentTaskResult(UnitTestResult.Success, result);
        }
    }
}
