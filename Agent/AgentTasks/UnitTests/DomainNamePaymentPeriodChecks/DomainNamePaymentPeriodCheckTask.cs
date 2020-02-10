using System;

namespace Zidium.Agent.AgentTasks
{
    internal class DomainNamePaymentPeriodCheckTask : AgentTaskBase
    {
        public DomainNamePaymentPeriodCheckTask()
        {
            ExecutionPeriod = TimeSpan.FromSeconds(5);
            WaitOnErrorTime = TimeSpan.FromMinutes(10); // чтобы не было большого количества запросов из-за ошибок
        }

        protected override AgentTaskResult Do()
        {
            var processor = new DomainNamePaymentPeriodCheckProcessor(Logger, CancellationToken, TimeService);
            processor.ProcessAll();

            var result = string.Format(
                "Ошибок {0}, успешно {1}",
                processor.ErrorCount,
                processor.SuccessCount);

            return GetResult(processor.DbProcessor, result);
        }
    }
}
