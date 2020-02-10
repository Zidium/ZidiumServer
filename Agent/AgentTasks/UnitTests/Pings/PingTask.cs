using System;

namespace Zidium.Agent.AgentTasks
{
    internal class PingTask : AgentTaskBase
    {
        public PingTask()
        {
            ExecutionPeriod = TimeSpan.FromSeconds(5);
        }

        protected override AgentTaskResult Do()
        {
            var processor = new PingProcessor(Logger, CancellationToken, TimeService);
            processor.ProcessAll();

            var result = string.Format(
                "Ошибок {0}, успешно {1}",
                processor.ErrorCount,
                processor.SuccessCount);

            return GetResult(processor.DbProcessor, result);
        }
    }
}
