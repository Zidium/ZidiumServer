using System;
using System.Linq;
using System.Threading;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;
using Zidium.Agent.AgentTasks.Notifications;
using Zidium.Api.Dto;
using Zidium.Storage;
using Zidium.Storage.Ef;
using Zidium.TestTools;

namespace Zidium.Agent.Tests
{
    public class HttpNotificationTests : BaseTest
    {
        [Fact]
        public void SendHttpNotificationTest()
        {
            // Создадим компонент
            var account = TestHelper.GetTestAccount();
            var user = TestHelper.CreateTestUser();
            var component = account.CreateTestApplicationComponent();
            var dispatcher = TestHelper.GetDispatcherClient();

            // Отправим красное событие
            var eventType = TestHelper.GetTestEventType();
            var eventResponse = dispatcher.SendEvent(new SendEventRequestDataDto()
            {
                TypeSystemName = eventType.SystemName,
                ComponentId = component.Id,
                Category = SendEventCategory.ApplicationError,
                Importance = EventImportance.Alarm
            });
            Assert.True(eventResponse.Success);

            // Создадим уведомление
            Guid notificationId;
            using (var context = account.GetDbContext())
            {
                var notification = new DbNotification()
                {
                    Id = Guid.NewGuid(),
                    UserId = user.Id,
                    EventId = eventResponse.Data.EventId,
                    Type = SubscriptionChannel.Http,
                    Address = @"http://fakesite.zidium.net/post",
                    Status = NotificationStatus.InQueue,
                    CreationDate = DateTime.Now,
                    NotificationHttp = new DbNotificationHttp()
                    {
                        Json = @"{ Component: { Id: 1}"
                    }
                };

                context.Notifications.Add(notification);
                notificationId = notification.Id;
                context.SaveChanges();
            }

            // Запустим обработку
            var processor = new HttpNotificationsProcessor(NullLogger.Instance, new CancellationToken());
            processor.Process(component.Id);

            // Проверим, что уведомление успешно отправилось
            using (var context = account.GetDbContext())
            {
                var notification = context.Notifications.First(t => t.Id == notificationId);
                Assert.Equal(NotificationStatus.Processed, notification.Status);
            }
        }
    }
}
