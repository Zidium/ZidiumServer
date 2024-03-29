﻿using System;
using Zidium.Api.Dto;

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
            var apiId = AgentConfiguration.SmsRuApiId;
            var from = AgentConfiguration.SmsRuFrom;

            var processor = new SendSmsProcessor(Logger, CancellationToken, apiId, from);
            var error = processor.Process();

            var result = string.Format("Отправлено {0} sms", processor.SuccessSendCount);
            return error == null ?
                new AgentTaskResult(UnitTestResult.Success, result) :
                new AgentTaskResult(UnitTestResult.Warning, error);
        }
    }
}
