using System;

namespace Zidium.Agent.AgentTasks.Notifications
{
    /// <summary>
    /// Задача создает для каждого Email-уведомления задачу по отправке email
    /// </summary>
    public class EmailNotificationsTask : AgentTaskBase
    {
        public EmailNotificationsTask()
        {
            ExecutionPeriod = TimeSpan.FromSeconds(10);
        }

        protected override AgentTaskResult Do()
        {
            var processor = new EmailNotificationsProcessor(Logger, CancellationToken);
            processor.Process();

            var result = string.Format("Отправлено {0} уведомлений", processor.CreatedNotificationsCount);
            return GetResult(processor.DbProcessor, result);
        }
    }
}
