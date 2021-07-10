using System;
using Zidium.Api.Dto;

namespace Zidium.Agent.AgentTasks.Notifications
{
    /// <summary>
    /// Задача создает для каждого уведомления через мессенджеры задачу по отправке сообщения
    /// </summary>
    public class MessangerNotificationsTask : AgentTaskBase
    {
        public MessangerNotificationsTask()
        {
            ExecutionPeriod = TimeSpan.FromSeconds(10);
        }

        protected override AgentTaskResult Do()
        {
            var processor = new MessangerNotificationsProcessor(Logger, CancellationToken);
            processor.Process();

            var result = string.Format("Отправлено {0} уведомлений", processor.CreatedNotificationsCount);
            return new AgentTaskResult(UnitTestResult.Success, result);
        }
    }
}