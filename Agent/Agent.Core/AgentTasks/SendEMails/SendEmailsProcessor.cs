﻿using System;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using MimeKit;
using Zidium.Core.Common;
using Zidium.Core.Common.Helpers;
using Zidium.Storage;

namespace Zidium.Agent.AgentTasks.SendEMails
{
    public class SendEmailsProcessor
    {
        public string SmtpLogin { get; set; }

        public string SmtpFrom { get; set; }
        public string SmtpFromEmail { get; }
        public string SmtpPassword { get; set; }

        public string SmtpServer { get; set; }

        public int SmtpPort { get; set; }

        public bool UseMailKit { get; set; }

        public bool UseSsl { get; set; }

        public string LocalServerName { get; }

        public bool FakeMode { get; set; }

        protected ILogger Logger;

        protected CancellationToken CancellationToken;

        public int SuccessSendCount;

        public int SendMaxCount = 100;

        protected ITimeService TimeService;

        public SendEmailsProcessor(
            ILogger logger,
            CancellationToken cancellationToken,
            string smtpServer,
            int smtpPort,
            string smtpLogin,
            string smtpFrom,
            string smtpFromEmail,
            string smtpPassword,
            bool useMailKit, 
            bool useSsl, string 
            localServerName)
        {
            Logger = logger;
            CancellationToken = cancellationToken;
            SmtpServer = smtpServer;
            SmtpPort = smtpPort;
            SmtpLogin = smtpLogin;
            SmtpFrom = smtpFrom;
            SmtpFromEmail = smtpFromEmail;
            SmtpPassword = smtpPassword;
            UseMailKit = useMailKit;
            UseSsl = useSsl;
            LocalServerName = localServerName;
            TimeService = DependencyInjection.GetServicePersistent<ITimeService>();
        }

        public string Process(Guid? emailId = null)
        {
            var storage = DependencyInjection.GetServicePersistent<IStorageFactory>().GetStorage();
            var notificationRepository = storage.Notifications;

            var emails = storage.SendEmailCommands.GetForSend(SendMaxCount);

            if (emailId.HasValue)
                emails = emails.Where(t => t.Id == emailId.Value).ToArray();

            var count = emails.Length;
            if (count > 0)
            {
                Logger.LogDebug("Всего писем для отправки: " + count);
            }
            else
            {
                Logger.LogTrace("Нет писем для отправки");
                return null;
            }

            var lastSendTime = DateTime.MinValue;
            string error = null;

            using (var smtpClient = GetSmtpClient())
            {
                foreach (var email in emails)
                {
                    CancellationToken.ThrowIfCancellationRequested();

                    Logger.LogDebug("Отправляем: {0} Тема: {1}", email.To, email.Subject);

                    try
                    {
                        // todo в почтовом клиенте thunderbird письма иногда неверно сортируются по дате
                        // есть предположение, что это из-за того, что дата создания округляется до секунд
                        // поэтому искуственно сделаем так, чтобы дата отправки писем отличалась как минимум на 1 секунду
                        DateTimeHelper.WaitForTime(lastSendTime.AddMilliseconds(1100));

                        // отправим сообщение
                        SendEmail(smtpClient, email.To, email.Subject, email.Body, email.IsHtml);

                        // отправим событие = чтобы знать, сколько писем в день отправлено
                        /*
                        DbProcesor.ComponentControl
                            .CreateComponentEvent("Отправка почты")
                            .SetJoinInterval(TimeSpan.FromDays(1))
                            .SetImportance(EventImportance.Unknown)
                            .SetJoinKey(_timeService.Now().ToString("yyyy.MM.dd"))
                            .Add();
                        */
                        // TODO Тут надо писать метрику

                        // обновим статус  и статистику
                        storage.SendEmailCommands.MarkAsSendSuccessed(email.Id, TimeService.Now());
                        Interlocked.Increment(ref SuccessSendCount);

                        // если есть соответствующее уведомление, поменяем его статус
                        if (email.ReferenceId != null)
                        {
                            var notification = notificationRepository.GetOneOrNullById(email.ReferenceId.Value);
                            if (notification != null)
                            {
                                var notificationForUpdate = notification.GetForUpdate();
                                notificationForUpdate.Status.Set(NotificationStatus.Sent);
                                notificationRepository.Update(notificationForUpdate);
                            }
                        }

                        Logger.LogInformation("Отправлено письмо на адрес '{0}' с темой '{1}'", email.To, email.Subject);
                    }
                    catch (OperationCanceledException)
                    {
                        throw;
                    }
                    catch (Exception exception)
                    {
                        if (!IsRatelimitException(exception))
                        {
                            error = exception.Message;

                            exception.Data.Add("EMailId", email.Id);
                            Logger.LogError(exception, exception.Message);

                            storage.SendEmailCommands.MarkAsSendFail(email.Id, TimeService.Now(), exception.Message);

                            // если есть соответствующее уведомление, поменяем его статус
                            if (email.ReferenceId != null)
                            {
                                var notification = notificationRepository.GetOneOrNullById(email.ReferenceId.Value);
                                if (notification != null)
                                {
                                    var notificationForUpdate = notification.GetForUpdate();
                                    notificationForUpdate.Status.Set(NotificationStatus.Error);
                                    notificationForUpdate.SendError.Set(exception.Message);
                                    notificationRepository.Update(notificationForUpdate);
                                }
                            }
                        }
                        else
                        {
                            // Особый случай - превышение лимита нашего ящика
                            // За ошибку не считаем, в следующий раз отправится

                            Logger.LogError(exception, "Превышен лимит отправки ящика Zidium");

                            // Отправлять дальше нет смысла
                            break;
                        }
                    }
                }
            }

            return error;
        }

        public void SendEmail(IDisposable smtpClient, string to, string subject, string body, bool isHtml)
        {
            if (FakeMode)
            {
                return;
            }

            if (smtpClient == null)
                smtpClient = GetSmtpClient();

            if (!UseMailKit)
            {
                SendStandardEmail((SmtpClient)smtpClient, to, subject, body, isHtml);
            }
            else
            {
                SendMailKitEmail((MailKit.Net.Smtp.SmtpClient)smtpClient, to, subject, body, isHtml);
            }
        }

        protected void SendStandardEmail(SmtpClient smtpClient, string to, string subject, string body, bool isHtml)
        {
            using (var message = new MailMessage())
            {
                message.From = new MailAddress(SmtpFromEmail, SmtpFrom);
                message.To.Add(new MailAddress(to));
                message.Subject = subject;
                message.Body = body;
                message.IsBodyHtml = isHtml;
                smtpClient.Send(message);
            }
        }

        protected void SendMailKitEmail(MailKit.Net.Smtp.SmtpClient smtpClient, string to, string subject, string body, bool isHtml)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(SmtpFrom, SmtpFromEmail));
            message.To.Add(new MailboxAddress(to, to));
            message.Subject = subject;
            var builder = new BodyBuilder();
            if (isHtml)
            {
                builder.HtmlBody = body;
            }
            else
            {
                builder.TextBody = body;
            }
            message.Body = builder.ToMessageBody();
            message.Headers.Remove("Message-Id");
            smtpClient.Send(message);
        }

        protected IDisposable GetSmtpClient()
        {
            if (!UseMailKit)
            {
                var smtpClient = new SmtpClient(SmtpServer, SmtpPort);
                var credentials = new NetworkCredential(SmtpLogin, SmtpPassword);
                smtpClient.EnableSsl = UseSsl;
                smtpClient.Credentials = credentials;
                smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                return smtpClient;
            }
            else
            {
                var smtpClient = new MailKit.Net.Smtp.SmtpClient();
                smtpClient.LocalDomain = LocalServerName;
                if (!FakeMode)
                {
                    smtpClient.Connect(SmtpServer, SmtpPort, UseSsl ? SecureSocketOptions.SslOnConnect : SecureSocketOptions.Auto);

                    // Иногда не удаётся зайти в ящик, на следующий раз всё ок
                    // TODO сделать счётчик неудачных попыток 
                    try
                    {
                        smtpClient.Authenticate(SmtpLogin, SmtpPassword);
                    }
                    catch (AuthenticationException exception)
                    {
                        if (exception.Message.StartsWith("Authentication failed.", StringComparison.OrdinalIgnoreCase))
                        {
                            // Отменяем обработку до следующего запуска
                            throw new OperationCanceledException();
                        }

                        throw;
                    }
                }
                return smtpClient;
            }
        }

        protected bool IsRatelimitException(Exception exception)
        {
            var result = exception.Message.IndexOf("Ratelimit exceeded for mailbox", StringComparison.OrdinalIgnoreCase) >= 0;
            return result;
        }
    }
}
