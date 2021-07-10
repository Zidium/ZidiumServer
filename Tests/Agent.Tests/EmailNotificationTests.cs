﻿using System;
using System.Linq;
using System.Threading;
using NLog;
using Xunit;
using Zidium.Agent.AgentTasks.Notifications;
using Zidium.Api.Dto;
using Zidium.Storage;
using Zidium.Storage.Ef;
using Zidium.TestTools;

namespace Zidium.Agent.Tests
{
    public class EmailNotificationTests : BaseTest
    {
        [Fact]
        public void SendEmailNotificationTest()
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
            account.SaveAllCaches();

            // Создадим уведомление
            Guid notificationId;
            using (var context = account.GetDbContext())
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
            processor.Process(component.Id);

            using (var context = account.GetDbContext())
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
