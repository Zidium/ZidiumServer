using System;
using Zidium.Api.Dto;

namespace Zidium.Agent.AgentTasks.SendEMails
{
    /// <summary>
    /// Отправляет письма из очереди в SMTP шлюз
    /// </summary>
    public class SendEmailsTask : AgentTaskBase
    {
        public SendEmailsTask()
        {
            ExecutionPeriod = TimeSpan.FromSeconds(10);
        }

        protected override AgentTaskResult Do()
        {
            var server = AgentConfiguration.SmtpServer;
            var port = AgentConfiguration.SmtpPort;
            var login = AgentConfiguration.SmtpLogin;
            var from = AgentConfiguration.SmtpFrom;
            var fromEmail = AgentConfiguration.SmtpFromEmail;
            var password = AgentConfiguration.SmtpPassword;
            var useMailKit = AgentConfiguration.SmtpUseMailKit;
            var useSsl = AgentConfiguration.SmtpUseSsl;
            var localServerName = AgentConfiguration.SmtpLocalServerName;

            var processor = new SendEmailsProcessor(Logger, CancellationToken, server, port, login, from, fromEmail, password, useMailKit, useSsl, localServerName);
            var error = processor.Process();

            var result = string.Format("Отправлено {0} писем", processor.SuccessSendCount);
            return error == null ?
                new AgentTaskResult(UnitTestResult.Success, result) :
                new AgentTaskResult(UnitTestResult.Warning, error);
        }
    }
}
