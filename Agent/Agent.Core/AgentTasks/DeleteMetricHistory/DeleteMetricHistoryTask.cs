using System;
using Zidium.Api.Dto;

namespace Zidium.Agent.AgentTasks.DeleteMetricHistory
{
    public class DeleteMetricHistoryTask : AgentTaskBase
    {
        public DeleteMetricHistoryTask()
        {
            ExecutionPeriod = TimeSpan.FromHours(1);
            WaitOnErrorTime = TimeSpan.FromHours(1);
        }

        protected override AgentTaskResult Do()
        {
            var processor = new DeleteMetricHistoryProcessor(Logger, CancellationToken);
            processor.Process();

            var result = string.Format(
                "Удалено значений метрик: {0}",
                processor.DeletedMetricValueCount);

            return new AgentTaskResult(UnitTestResult.Success, result);
        }
    }
}
