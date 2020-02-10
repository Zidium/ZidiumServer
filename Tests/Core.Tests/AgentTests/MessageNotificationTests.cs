using System;
using System.Linq;
using System.Threading;
using NLog;
using Xunit;
using Zidium.Agent.AgentTasks.Notifications;
using Zidium.Core.AccountsDb;
using Zidium.Core.Api;
using Zidium.TestTools;

namespace Zidium.Core.Tests.AgentTests
{
    public class MessageNotificationTests
    {
        [Theory]
        [InlineData(SubscriptionChannel.Telegram)]
        [InlineData(SubscriptionChannel.VKontakte)]
        public void SendMessageNotificationTest(SubscriptionChannel channel)
        {
            // Создадим компонент
            var account = TestHelper.GetTestAccount();
            var user = TestHelper.CreateTestUser(account.Id);
            var component = account.CreateTestApplicationComponent();
            var dispatcher = TestHelper.GetDispatcherClient();

            // Отправим красное событие
            var eventType = TestHelper.GetTestEventType(account.Id);
            var eventResponse = dispatcher.SendEvent(account.Id, new SendEventData()
            {
                TypeSystemName = eventType.SystemName,
                ComponentId = component.Id,
                Category = EventCategory.ApplicationError,
                Importance = EventImportance.Alarm
            });
            Assert.True(eventResponse.Success);
            account.SaveAllCaches();

            // Создадим уведомление
            Guid notificationId;
            using (var context = account.CreateAccountDbContext())
            {
                var notification = new Notification()
                {
                    Id = Guid.NewGuid(),
                    UserId = user.Id,
                    EventId = eventResponse.Data.EventId,
                    Type = channel,
                    Address = user.Login,
                    Status = NotificationStatus.InQueue,
                    CreationDate = DateTime.Now
                };

                var notificationRepository = context.GetNotificationRepository();
                notificationRepository.Add(notification);
                notificationId = notification.Id;
                context.SaveChanges();
            }

            // Запустим обработку
            var processor = new MessangerNotificationsProcessor(LogManager.GetCurrentClassLogger(), new CancellationToken());
            processor.Process(account.Id, component.Id);

            using (var context = account.CreateAccountDbContext())
            {
                // Должно появиться сообщение в очереди
                var messageCommandRepository = context.GetSendMessageCommandRepository();
                var command = messageCommandRepository.QueryAll().FirstOrDefault(t => t.ReferenceId == notificationId);

                Assert.NotNull(command);
                Assert.NotNull(command.To);
                Assert.NotNull(command.Body);
                Assert.Equal(channel, command.Channel);

                // У уведомления должна появиться ссылка на сообщение
                var notificationRepository = context.GetNotificationRepository();
                var notification = notificationRepository.Find(notificationId);

                Assert.Equal(command.Id, notification.SendMessageCommandId);
            }
        }
    }
}