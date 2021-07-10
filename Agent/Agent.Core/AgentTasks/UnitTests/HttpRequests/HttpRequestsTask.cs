using System;
using Zidium.Api.Dto;

namespace Zidium.Agent.AgentTasks.HttpRequests
{
    internal class HttpRequestsTask : AgentTaskBase
    {
        public HttpRequestsTask()
        {
            ExecutionPeriod = TimeSpan.FromSeconds(10);
        }

        protected override AgentTaskResult Do()
        {
            var processor = new HttpRequestsProcessor(Logger, CancellationToken, TimeService);
            processor.Process();

            var result = string.Format(
                "Выполнено проверок {0}, из них ошибок {1}, успешно {2}",
                processor.AllCount,
                processor.ErrorCount,
                processor.SuccessCount);

            return processor.LastError == null ?
                new AgentTaskResult(UnitTestResult.Success, result) :
                new AgentTaskResult(UnitTestResult.Warning, processor.LastError);
        }
    }
}
