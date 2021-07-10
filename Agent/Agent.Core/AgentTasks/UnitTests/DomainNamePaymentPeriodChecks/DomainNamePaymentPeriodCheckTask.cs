using System;
using Zidium.Api.Dto;

namespace Zidium.Agent.AgentTasks
{
    internal class DomainNamePaymentPeriodCheckTask : AgentTaskBase
    {
        public DomainNamePaymentPeriodCheckTask()
        {
            ExecutionPeriod = TimeSpan.FromMinutes(1);
            WaitOnErrorTime = TimeSpan.FromMinutes(10); // чтобы не было большого количества запросов из-за ошибок
        }

        protected override AgentTaskResult Do()
        {
            var processor = new DomainNamePaymentPeriodCheckProcessor(Logger, CancellationToken, TimeService);
            processor.Process();

            var result = string.Format(
                "Ошибок {0}, успешно {1}",
                processor.ErrorCount,
                processor.SuccessCount);

            return processor.LastError == null ?
                new AgentTaskResult(UnitTestResult.Success, result) :
                new AgentTaskResult(UnitTestResult.Warning, processor.LastError);
        }
    }
}
