using System;
using System.Threading;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;
using Zidium.Agent.AgentTasks.SendMessages;
using Zidium.Api.Dto;
using Zidium.Common;
using Zidium.Storage;
using Zidium.Storage.Ef;
using Zidium.TestTools;

namespace Zidium.Agent.Tests
{
    public class TelegramTests : BaseTest
    {
        [Fact]
        public void SendSimpleMessageTest()
        {
            var account = TestHelper.GetTestAccount();
            Guid commandId;

            using (var accountDbContext = account.GetDbContext())
            {
                // создадим сообщение
                var command = new DbSendMessageCommand()
                {
                    Channel = SubscriptionChannel.Telegram,
                    Body = "test body",
                    CreateDate = DateTime.Now,
                    Id = Ulid.NewUlid(),
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
            var processor = new SendToTelegramProcessor(NullLogger.Instance, new CancellationToken(), botToken);
            processor.FakeMode = true;
            processor.Process(commandId);

            // проверим, что сообщение отправлено
            using (var accountDbContext = account.GetDbContext())
            {
                var command = accountDbContext.SendMessageCommands.Find(commandId);
                Assert.Null(command.ErrorMessage);
                Assert.Equal(MessageStatus.Sent, command.Status);
            }
        }

        [Fact]
        public void SendNotificationMessageTest()
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

            Guid commandId;
            Guid notificationId;

            using (var accountDbContext = account.GetDbContext())
            {
                // создадим уведомление
                var notification = new DbNotification()
                {
                    Id = Ulid.NewUlid(),
                    UserId = user.Id,
                    EventId = eventResponse.Data.EventId,
                    Type = SubscriptionChannel.Telegram,
                    Address = user.Login,
                    Status = NotificationStatus.InQueue,
                    CreationDate = DateTime.Now
                };

                accountDbContext.Notifications.Add(notification);
                notificationId = notification.Id;
                accountDbContext.SaveChanges();

                // создадим сообщение
                var command = new DbSendMessageCommand()
                {
                    Channel = SubscriptionChannel.Telegram,
                    Body = "test body",
                    CreateDate = DateTime.Now,
                    Id = Ulid.NewUlid(),
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
            var processor = new SendToTelegramProcessor(NullLogger.Instance, new CancellationToken(), botToken);
            processor.FakeMode = true;
            processor.Process(commandId);

            using (var accountDbContext = account.GetDbContext())
            {
                // проверим, что сообщение отправлено
                var command = accountDbContext.SendMessageCommands.Find(commandId);
                Assert.Null(command.ErrorMessage);
                Assert.Equal(MessageStatus.Sent, command.Status);

                // проверим, что статус уведомления поменялся
                var notification = accountDbContext.Notifications.Find(notificationId);
                Assert.Equal(NotificationStatus.Sent, notification.Status);
            }
        }

    }
}