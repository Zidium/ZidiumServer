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
            var server = ServiceConfiguration.SmtpServer;
            var port = ServiceConfiguration.SmtpPort; 
            var login = ServiceConfiguration.SmtpLogin;
            var from = ServiceConfiguration.SmtpFrom;
            var password = ServiceConfiguration.SmtpPassword;
            var useMailKit = ServiceConfiguration.SmtpUseMailKit;
            var useSsl = ServiceConfiguration.SmtpUseSsl;

            var processor = new SendEmailsProcessor(Logger, CancellationToken, server, port, login, from, password, useMailKit, useSsl);
            processor.Process();

            var result = string.Format("Отправлено {0} писем", processor.SuccessSendCount);
            return GetResult(processor.DbProcessor, result);
        }
    }
}
