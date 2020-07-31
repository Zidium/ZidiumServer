using System;
using System.Linq;
using System.Threading;
using NLog;
using Xunit;
using Zidium.Agent.AgentTasks.Notifications;
using Zidium.Core.Api;
using Zidium.Storage;
using Zidium.Storage.Ef;
using Zidium.TestTools;

namespace Zidium.Core.Tests.AgentTests
{
    public class EmailNotificationTests
    {
        [Fact]
        public void SendEmailNotificationTest()
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
            using (var context = account.GetAccountDbContext())
            {
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

                context.Notifications.Add(notification);
                notificationId = notification.Id;
                context.SaveChanges();
            }

            // Запустим обработку
            var processor = new EmailNotificationsProcessor(LogManager.GetCurrentClassLogger(), new CancellationToken());
            processor.Process(account.Id, component.Id);

            using (var context = account.GetAccountDbContext())
            {
                // Должно появиться письмо
                var email = context.SendEmailCommands.FirstOrDefault(t => t.ReferenceId == notificationId);

                Assert.NotNull(email);
                Assert.Equal(user.Login, email.To);

                // У уведомления должна появиться ссылка на письмо
                var notification = context.Notifications.Find(notificationId);

                Assert.Equal(email.Id, notification.SendEmailCommandId);
            }
        }
    }
}
