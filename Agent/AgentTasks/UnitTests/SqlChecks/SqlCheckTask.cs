using System;

namespace Zidium.Agent.AgentTasks
{
    internal class SqlCheckTask : AgentTaskBase
    {
        public SqlCheckTask()
        {
            ExecutionPeriod = TimeSpan.FromSeconds(5);
        }

        protected override AgentTaskResult Do()
        {
            var processor = new SqlCheckProcessor(Logger, CancellationToken, TimeService);
            processor.ProcessAll();

            var result = string.Format(
                "Ошибок {0}, успешно {1}",
                processor.ErrorCount,
                processor.SuccessCount);

            return GetResult(processor.DbProcessor, result);
        }
    }
}
