using System;

namespace Zidium.Agent.AgentTasks.SendMessages
{
    /// <summary>
    /// Отправляет сообщения через VKontakte
    /// </summary>
    public class SendToVKontakteTask : AgentTaskBase
    {
        public SendToVKontakteTask()
        {
            ExecutionPeriod = TimeSpan.FromSeconds(10);
        }

        protected override AgentTaskResult Do()
        {
            var authToken = AgentConfiguration.VKontakteAuthToken;

            var processor = new SendToVKontakteProcessor(Logger, CancellationToken, authToken);
            processor.Process();

            var result = string.Format("Отправлено {0} сообщений", processor.SuccessSendCount);
            return GetResult(processor.DbProcessor, result);
        }
    }
}