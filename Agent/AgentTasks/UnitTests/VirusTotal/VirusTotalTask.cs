using System;

namespace Zidium.Agent.AgentTasks.UnitTests.VirusTotal
{
    public class VirusTotalTask : AgentTaskBase
    {
        private VirusTotalTaskProcessor processor;

        public VirusTotalTask()
        {
            ExecutionPeriod = TimeSpan.FromMinutes(1);
            processor = new VirusTotalTaskProcessor(Logger, CancellationToken, TimeService);
        }

        protected override AgentTaskResult Do()
        {
            processor.ProcessAll();

            var result = string.Format(
                "Ошибок {0}, успешно {1}",
                processor.ErrorCount,
                processor.SuccessCount);

            return GetResult(processor.DbProcessor, result);
        }
    }
}
