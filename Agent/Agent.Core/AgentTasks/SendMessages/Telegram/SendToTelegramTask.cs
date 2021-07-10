using System;
using Zidium.Api.Dto;

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
            var botToken = AgentConfiguration.TelegramBotToken;

            var processor = new SendToTelegramProcessor(Logger, CancellationToken, botToken);
            var error = processor.Process();

            var result = string.Format("Отправлено {0} сообщений", processor.SuccessSendCount);
            return error == null ?
                new AgentTaskResult(UnitTestResult.Success, result) :
                new AgentTaskResult(UnitTestResult.Warning, error);
        }
    }
}