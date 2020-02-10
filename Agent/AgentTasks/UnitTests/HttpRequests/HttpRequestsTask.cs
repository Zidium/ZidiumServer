using System;

namespace Zidium.Agent.AgentTasks.HttpRequests
{
    internal class HttpRequestsTask : AgentTaskBase
    {
        public HttpRequestsTask()
        {
            ExecutionPeriod = TimeSpan.FromSeconds(5);
        }

        protected override AgentTaskResult Do()
        {
            var processor = new HttpRequestsProcessor(Logger, CancellationToken, TimeService);
            processor.ProcessAll();

            var result = string.Format(
                "Выполнено проверок {0}, из них ошибок {1}, успешно {2}",
                processor.AllCount,
                processor.ErrorCount,
                processor.SuccessCount);

            return GetResult(processor.DbProcessor, result);
        }
    }
}
