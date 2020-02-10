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
    public class HttpNotificationTests
    {
        [Fact]
        public void SendHttpNotificationTest()
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

            // Создадим уведомление
            Guid notificationId;
            using (var context = account.CreateAccountDbContext())
            {
                var notification = new Notification()
                {
                    Id = Guid.NewGuid(),
                    UserId = user.Id,
                    EventId = eventResponse.Data.EventId,
                    Type = SubscriptionChannel.Http,
                    Address = @"http://fakesite.zidium.net/post",
                    Status = NotificationStatus.InQueue,
                    CreationDate = DateTime.Now,
                    NotificationHttp = new NotificationHttp()
                    {
                        Json = @"{ Component: { Id: 1}"
                    }
                };

                var notificationRepository = context.GetNotificationRepository();
                notificationRepository.Add(notification);
                notificationId = notification.Id;
                context.SaveChanges();
            }

            // Запустим обработку
            var processor = new HttpNotificationsProcessor(LogManager.GetCurrentClassLogger(), new CancellationToken());
            processor.Process(account.Id, component.Id);

            // Проверим, что уведомление успешно отправилось
            using (var context = account.CreateAccountDbContext())
            {
                var notificationRepository = context.GetNotificationRepository();
                var notification = notificationRepository.QueryAllByComponent(component.Id).First(t => t.Id == notificationId);
                Assert.Equal(NotificationStatus.Processed, notification.Status);
            }
        }
    }
}
