using System;

namespace Zidium.Agent.AgentTasks.DeleteEvents
{
    public class DeleteComponentEventsTask : AgentTaskBase
    {
        public DeleteComponentEventsTask()
        {
            ExecutionPeriod = TimeSpan.FromHours(1);
            WaitOnErrorTime = TimeSpan.FromHours(1);
        }

        protected override AgentTaskResult Do()
        {
            var maxDeleteCount = AgentConfiguration.EventsMaxDeleteCount;

            var processor = new DeleteComponentEventsProcessor(Logger, CancellationToken);

            if (maxDeleteCount.HasValue)
                processor.MaxDeleteCount = maxDeleteCount.Value;

            processor.Process();

            var result = string.Format(
                "Удалено событий: {0}; удалено свойств: {1}",
                processor.DeletedEventsCount,
                processor.DeletedPropertiesCount);

            return GetResult(processor.DbProcessor, result);
        }
    }
}
