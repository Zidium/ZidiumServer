using System;

namespace Zidium.Agent.AgentTasks.Notifications
{
    /// <summary>
    /// Задача для каждого Http-уведомления выполняет post-запрос
    /// </summary>
    public class HttpNotificationsTask : AgentTaskBase
    {
        public HttpNotificationsTask()
        {
            ExecutionPeriod = TimeSpan.FromSeconds(10);
        }

        protected override AgentTaskResult Do()
        {
            var processor = new HttpNotificationsProcessor(Logger, CancellationToken);
            processor.Process();

            var result = string.Format("Выполнено {0} post-запросов", processor.CreatedNotificationsCount);
            return GetResult(processor.DbProcessor, result);
        }
    }
}
