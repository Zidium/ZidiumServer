using System;
using Zidium.Api.Dto;

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
            var error = processor.Process();

            var result = string.Format("Отправлено {0} сообщений", processor.SuccessSendCount);
            return error == null ?
                new AgentTaskResult(UnitTestResult.Success, result) :
                new AgentTaskResult(UnitTestResult.Warning, error);
        }
    }
}