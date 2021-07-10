using System;
using Zidium.Api.Dto;

namespace Zidium.Agent.AgentTasks
{
    internal class SqlCheckTask : AgentTaskBase
    {
        public SqlCheckTask()
        {
            ExecutionPeriod = TimeSpan.FromSeconds(10);
        }

        protected override AgentTaskResult Do()
        {
            var processor = new SqlCheckProcessor(Logger, CancellationToken, TimeService);
            processor.Process();

            var result = string.Format(
                "Ошибок {0}, успешно {1}",
                processor.ErrorCount,
                processor.SuccessCount);

            return new AgentTaskResult(UnitTestResult.Success, result);
        }
    }
}
