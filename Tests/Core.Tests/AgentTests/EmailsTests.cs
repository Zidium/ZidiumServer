using System;
using System.Threading;
using NLog;
using Zidium.Agent.AgentTasks.SendEMails;
using Xunit;
using Zidium.Core.Api;
using Zidium.Storage;
using Zidium.Storage.Ef;
using Zidium.TestTools;

namespace Zidium.Core.Tests.AgentTests
{
    public class EmailsTests
    {
        [Fact]
        public void SendSimpleEmailTest()
        {
            var account = TestHelper.GetTestAccount();
            Guid emailId;

            using (var accountDbContext = account.GetAccountDbContext())
            {

                // создадим письмо
                var email = new DbSendEmailCommand()
                {
                    Body = "test body",
                    CreateDate = DateTime.Now,
                    From = "zidium@yandex.ru",
                    Id = Guid.NewGuid(),
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
            var processor = new SendEmailsProcessor(LogManager.GetCurrentClassLogger(), new CancellationToken(), server, port, login, from, password, useMailKit, useSsl);
            processor.FakeMode = true;
            processor.Process(account.Id, emailId);

            // проверим, что письмо отправлено
            using (var accountDbContext = account.GetAccountDbContext())
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
            var user = TestHelper.CreateTestUser(account.Id);
            var component = account.CreateTestApplicationComponent();
            var dispatcher = TestHelper.GetDispatcherClient();

            // Отправим событие
            var eventType = TestHelper.GetTestEventType(account.Id);
            var eventResponse = dispatcher.SendEvent(account.Id, new SendEventData()
            {
                TypeSystemName = eventType.SystemName,
                ComponentId = component.Id,
                Category = EventCategory.ComponentEvent,
                Importance = EventImportance.Unknown
            });
            Assert.True(eventResponse.Success);
            account.SaveAllCaches();

            Guid emailId;
            Guid notificationId;

            using (var accountDbContext = account.GetAccountDbContext())
            {
                // создадим уведомление
                var notification = new DbNotification()
                {
                    Id = Guid.NewGuid(),
                    UserId = user.Id,
                    EventId = eventResponse.Data.EventId,
                    Type = SubscriptionChannel.Email,
                    Address = user.Login,
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
                    Id = Guid.NewGuid(),
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
            var processor = new SendEmailsProcessor(LogManager.GetCurrentClassLogger(), new CancellationToken(), server, port, login, from, password, useMailKit, useSsl);
            processor.FakeMode = true;
            processor.Process(account.Id, emailId);

            using (var accountDbContext = account.GetAccountDbContext())
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
