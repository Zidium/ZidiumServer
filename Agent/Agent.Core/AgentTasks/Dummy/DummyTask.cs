using System;
using Microsoft.Extensions.Logging;
using Zidium.Api.Dto;

namespace Zidium.Agent.AgentTasks.Dummy
{
    /// <summary>
    /// Тестовая задача для отладки
    /// Ничего не делает
    /// </summary>
    public class DummyTask : AgentTaskBase
    {
        public DummyTask()
        {
            ExecutionPeriod = TimeSpan.FromMinutes(1);
        }

        protected override AgentTaskResult Do()
        {
            Logger.LogInformation("Dummy task action");
            var processor = new DummyProcessor(Logger, CancellationToken);
            processor.Process();
            return new AgentTaskResult(UnitTestResult.Success, "OK");
        }
    }
}
