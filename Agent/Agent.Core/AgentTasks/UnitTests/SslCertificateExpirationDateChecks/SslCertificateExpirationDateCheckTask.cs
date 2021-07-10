using System;
using Zidium.Api.Dto;

namespace Zidium.Agent.AgentTasks
{
    internal class SslCertificateExpirationDateCheckTask : AgentTaskBase
    {
        public SslCertificateExpirationDateCheckTask()
        {
            ExecutionPeriod = TimeSpan.FromMinutes(1);
        }

        protected override AgentTaskResult Do()
        {
            var processor = new SslCertificateExpirationDateCheckProcessor(Logger, CancellationToken, TimeService);
            processor.Process();

            var result = string.Format(
                "Ошибок {0}, успешно {1}",
                processor.ErrorCount,
                processor.SuccessCount);

            return new AgentTaskResult(UnitTestResult.Success, result);
        }
    }
}
