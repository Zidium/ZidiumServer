using System;

namespace Zidium.Agent.AgentTasks.SendSms
{
    /// <summary>
    /// Отправляет письма из очереди в SMTP шлюз
    /// </summary>
    public class SendSmsTask : AgentTaskBase
    {
        public SendSmsTask()
        {
            ExecutionPeriod = TimeSpan.FromSeconds(10);
        }

        protected override AgentTaskResult Do()
        {
            var apiId = ServiceConfiguration.SmsRuApiId;
            var from = ServiceConfiguration.SmsRuFrom;

            var processor = new SendSmsProcessor(Logger, CancellationToken, apiId, from);
            processor.Process();

            var result = string.Format("Отправлено {0} sms", processor.SuccessSendCount);
            return GetResult(processor.DbProcessor, result);
        }
    }
}
