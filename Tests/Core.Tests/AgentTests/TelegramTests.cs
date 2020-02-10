using System;
using System.Threading;
using NLog;
using Xunit;
using Zidium.Agent.AgentTasks.SendMessages;
using Zidium.Core.AccountsDb;
using Zidium.Core.Api;
using Zidium.TestTools;

namespace Zidium.Core.Tests.AgentTests
{
    public class TelegramTests
    {
        [Fact]
        public void SendSimpleMessageTest()
        {
            var account = TestHelper.GetTestAccount();
            Guid commandId;

            using (var accountDbContext = account.CreateAccountDbContext())
            {
                // создадим сообщение
                var command = new SendMessageCommand()
                {
                    Channel = SubscriptionChannel.Telegram,
                    Body = "test body",
                    CreateDate = DateTime.Now,
                    Id = Guid.NewGuid(),
                    Status = MessageStatus.InQueue,
                    To = "12345",
                    ReferenceId = null
                };
                accountDbContext.SendMessageCommands.Add(command);
                accountDbContext.SaveChanges();
                commandId = command.Id;
            }

            // обработаем очередь
            var botToken = "1234567890";
            var processor = new SendToTelegramProcessor(LogManager.GetCurrentClassLogger(), new CancellationToken(), botToken);
            processor.FakeMode = true;
            processor.Process(account.Id, commandId);

            // проверим, что сообщение отправлено
            using (var accountDbContext = account.CreateAccountDbContext())
            {
                var command = accountDbContext.GetSendMessageCommandRepository().GetById(commandId);
                Assert.Null(command.ErrorMessage);
                Assert.Equal(MessageStatus.Sent, command.Status);
            }
        }

        [Fact]
        public void SendNotificationMessageTest()
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

            Guid commandId;
            Guid notificationId;

            using (var accountDbContext = account.CreateAccountDbContext())
            {
                // создадим уведомление
                var notification = new Notification()
                {
                    Id = Guid.NewGuid(),
                    UserId = user.Id,
                    EventId = eventResponse.Data.EventId,
                    Type = SubscriptionChannel.Telegram,
                    Address = user.Login,
                    Status = NotificationStatus.InQueue,
                    CreationDate = DateTime.Now
                };

                var notificationRepository = accountDbContext.GetNotificationRepository();
                notificationRepository.Add(notification);
                notificationId = notification.Id;
                accountDbContext.SaveChanges();

                // создадим сообщение
                var command = new SendMessageCommand()
                {
                    Channel = SubscriptionChannel.Telegram,
                    Body = "test body",
                    CreateDate = DateTime.Now,
                    Id = Guid.NewGuid(),
                    Status = MessageStatus.InQueue,
                    To = "12345",
                    ReferenceId = notificationId
                };
                accountDbContext.SendMessageCommands.Add(command);
                accountDbContext.SaveChanges();
                commandId = command.Id;
            }

            // обработаем очередь
            var botToken = "1234567890";
            var processor = new SendToTelegramProcessor(LogManager.GetCurrentClassLogger(), new CancellationToken(), botToken);
            processor.FakeMode = true;
            processor.Process(account.Id, commandId);

            using (var accountDbContext = account.CreateAccountDbContext())
            {
                // проверим, что сообщение отправлено
                var command = accountDbContext.GetSendMessageCommandRepository().GetById(commandId);
                Assert.Null(command.ErrorMessage);
                Assert.Equal(MessageStatus.Sent, command.Status);

                // проверим, что статус уведомления поменялся
                var notificationRepository = accountDbContext.GetNotificationRepository();
                var notification = notificationRepository.Find(notificationId);
                Assert.Equal(NotificationStatus.Sent, notification.Status);
            }
        }

    }
}