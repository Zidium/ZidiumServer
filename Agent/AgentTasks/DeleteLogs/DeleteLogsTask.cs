using System;

namespace Zidium.Agent.AgentTasks.DeleteLogs
{
    public class DeleteLogsTask : AgentTaskBase
    {
        public DeleteLogsTask()
        {
            ExecutionPeriod = TimeSpan.FromHours(1);
            WaitOnErrorTime = TimeSpan.FromHours(1);
        }

        protected override AgentTaskResult Do()
        {
            var processor = new DeleteLogsProcessor(Logger, CancellationToken);
            processor.Process();

            var result = string.Format(
                "Удалено логов: {0}; удалено свойств: {1}",
                processor.DeletedLogsCount,
                processor.DeletedPropertiesCount);

            return GetResult(processor.DbProcessor, result);
        }

    }
}
