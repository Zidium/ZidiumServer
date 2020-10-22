using System;

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
            var password = AgentConfiguration.SmtpPassword;
            var useMailKit = AgentConfiguration.SmtpUseMailKit;
            var useSsl = AgentConfiguration.SmtpUseSsl;

            var processor = new SendEmailsProcessor(Logger, CancellationToken, server, port, login, from, password, useMailKit, useSsl);
            processor.Process();

            var result = string.Format("Отправлено {0} писем", processor.SuccessSendCount);
            return GetResult(processor.DbProcessor, result);
        }
    }
}
