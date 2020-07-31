using System;

namespace Zidium.Agent.AgentTasks.UnitTests.VirusTotal
{
    public class VirusTotalTask : AgentTaskBase
    {
        private readonly VirusTotalTaskProcessor _processor;

        public VirusTotalTask()
        {
            ExecutionPeriod = TimeSpan.FromSeconds(20);
            _processor = new VirusTotalTaskProcessor(Logger, CancellationToken, TimeService);
        }

        protected override AgentTaskResult Do()
        {
            _processor.ProcessAll();

            var result = string.Format(
                "Ошибок {0}, успешно {1}",
                _processor.ErrorCount,
                _processor.SuccessCount);

            return GetResult(_processor.DbProcessor, result);
        }
    }
}
