using System;
using Zidium.Api.Dto;

namespace Zidium.Agent.AgentTasks.Notifications
{
    /// <summary>
    /// Задача по созданию уведомлений
    /// </summary>
    public class CreateNotificationsTask : AgentTaskBase
    {
        public CreateNotificationsTask()
        {
            ExecutionPeriod = TimeSpan.FromMinutes(1);
            WaitOnErrorTime = TimeSpan.FromMinutes(15);
        }

        protected override AgentTaskResult Do()
        {
            var processor = new CreateNotificationsProcessor(Logger, CancellationToken);
            processor.Process();

            var result = string.Format("Создано уведомлений: {0}", processor.CreatedNotificationsCount);
            return new AgentTaskResult(UnitTestResult.Success, result);
        }
    }
}
