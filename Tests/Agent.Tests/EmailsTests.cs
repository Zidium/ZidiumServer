using System;
using System.Threading;
using Zidium.Agent.AgentTasks.SendEMails;
using Xunit;
using Zidium.Storage;
using Zidium.Storage.Ef;
using Zidium.TestTools;
using Zidium.Api.Dto;
using Microsoft.Extensions.Logging.Abstractions;
using Zidium.Common;

namespace Zidium.Agent.Tests
{
    public class EmailsTests : BaseTest
    {
        [Fact]
        public void SendSimpleEmailTest()
        {
            var account = TestHelper.GetTestAccount();
            Guid emailId;

            using (var accountDbContext = account.GetDbContext())
            {

                // создадим письмо
                var email = new DbSendEmailCommand()
                {
                    Body = "test body",
                    CreateDate = DateTime.Now,
                    From = "zidium@yandex.ru",
                    Id = Ulid.NewUlid(),
                    Status = EmailStatus.InQueue,
                    Subject = "app test subject",
                    IsHtml = false,
                    To = "zem_ao@mail.ru",
                    ReferenceId = null
                };
                accountDbContext.SendEmailCommands.Add(email);
                accountDbContext.SaveChanges();
                emailId = email.Id;
            }

            // обработаем очередь
            string server = "mail.server.ru";
            int port = 465;
            bool useMailKit = true;
            bool useSsl = true;
            string login = "robot";
            string from = "robor@mail.server.ru";
            string password = "12345";
            var processor = new SendEmailsProcessor(NullLogger.Instance, new CancellationToken(), server, port, login, from, password, useMailKit, useSsl);
            processor.FakeMode = true;
            processor.Process(emailId);

            // проверим, что письмо отправлено
            using (var accountDbContext = account.GetDbContext())
            {
                var email = accountDbContext.SendEmailCommands.Find(emailId);
                Assert.Null(email.ErrorMessage);
                Assert.Equal(EmailStatus.Sent, email.Status);
            }
        }

        [Fact]
        public void SendNotificationEmailTest()
        {
            var account = TestHelper.GetTestAccount();
            var user = TestHelper.CreateTestUser();
            var component = account.CreateTestApplicationComponent();
            var dispatcher = TestHelper.GetDispatcherClient();

            // Отправим событие
            var eventType = TestHelper.GetTestEventType();
            var eventResponse = dispatcher.SendEvent(new SendEventRequestDataDto()
            {
                TypeSystemName = eventType.SystemName,
                ComponentId = component.Id,
                Category = SendEventCategory.ComponentEvent,
                Importance = EventImportance.Unknown
            });
            Assert.True(eventResponse.Success);
            account.SaveAllCaches();

            Guid emailId;
            Guid notificationId;

            using (var accountDbContext = account.GetDbContext())
            {
                // создадим уведомление
                var notification = new DbNotification()
                {
                    Id = Ulid.NewUlid(),
                    UserId = user.Id,
                    EventId = eventResponse.Data.EventId,
                    Type = SubscriptionChannel.Email,
                    Address = user.Email,
                    Status = NotificationStatus.InQueue,
                    CreationDate = DateTime.Now
                };

                accountDbContext.Notifications.Add(notification);
                notificationId = notification.Id;
                accountDbContext.SaveChanges();

                // создадим письмо
                var email = new DbSendEmailCommand()
                {
                    Body = "test body",
                    CreateDate = DateTime.Now,
                    From = "zidium@yandex.ru",
                    Id = Ulid.NewUlid(),
                    Status = EmailStatus.InQueue,
                    Subject = "app test subject",
                    IsHtml = false,
                    To = "zem_ao@mail.ru",
                    ReferenceId = notificationId
                };
                accountDbContext.SendEmailCommands.Add(email);
                accountDbContext.SaveChanges();
                emailId = email.Id;
            }

            // обработаем очередь
            string server = "mail.server.ru";
            int port = 465;
            bool useMailKit = true;
            bool useSsl = true;
            string login = "robot";
            string from = "robor@mail.server.ru";
            string password = "12345";
            var processor = new SendEmailsProcessor(NullLogger.Instance, new CancellationToken(), server, port, login, from, password, useMailKit, useSsl);
            processor.FakeMode = true;
            processor.Process(emailId);

            using (var accountDbContext = account.GetDbContext())
            {
                // проверим, что письмо отправлено
                var email = accountDbContext.SendEmailCommands.Find(emailId);
                Assert.Null(email.ErrorMessage);
                Assert.Equal(EmailStatus.Sent, email.Status);

                // проверим, что статус уведомления поменялся
                var notification = accountDbContext.Notifications.Find(notificationId);
                Assert.Equal(NotificationStatus.Sent, notification.Status);
            }
        }
    }
}
