﻿using System;

namespace Zidium.Agent.AgentTasks.DeleteEvents
{
    public class DeleteMetricEventsTask : AgentTaskBase
    {
        public DeleteMetricEventsTask()
        {
            ExecutionPeriod = TimeSpan.FromHours(1);
            WaitOnErrorTime = TimeSpan.FromHours(1);
        }

        protected override AgentTaskResult Do()
        {
            var maxDeleteCount = ServiceConfiguration.EventsMaxDeleteCount;

            var processor = new DeleteMetricEventsProcessor(Logger, CancellationToken);

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
