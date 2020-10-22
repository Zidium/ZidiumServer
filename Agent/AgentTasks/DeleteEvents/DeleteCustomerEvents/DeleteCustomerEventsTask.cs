using System;

namespace Zidium.Agent.AgentTasks.DeleteEvents
{
    public class DeleteCustomerEventsTask : AgentTaskBase
    {
        public DeleteCustomerEventsTask()
        {
            ExecutionPeriod = TimeSpan.FromHours(1);
            WaitOnErrorTime = TimeSpan.FromHours(1);
        }

        protected override AgentTaskResult Do()
        {
            var maxDeleteCount = AgentConfiguration.EventsMaxDeleteCount;

            var processor = new DeleteCustomerEventsProcessor(Logger, CancellationToken);

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
