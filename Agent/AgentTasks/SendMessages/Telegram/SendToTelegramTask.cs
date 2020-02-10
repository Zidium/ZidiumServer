using System;

namespace Zidium.Agent.AgentTasks.SendMessages
{
    /// <summary>
    /// Отправляет сообщения через Telegram
    /// </summary>
    public class SendToTelegramTask : AgentTaskBase
    {
        public SendToTelegramTask()
        {
            ExecutionPeriod = TimeSpan.FromSeconds(10);
        }

        protected override AgentTaskResult Do()
        {
            var botToken = ServiceConfiguration.TelegramBotToken;

            var processor = new SendToTelegramProcessor(Logger, CancellationToken, botToken);
            processor.Process();

            var result = string.Format("Отправлено {0} сообщений", processor.SuccessSendCount);
            return GetResult(processor.DbProcessor, result);
        }
    }
}