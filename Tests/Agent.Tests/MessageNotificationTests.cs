﻿using System;
using System.Linq;
using System.Threading;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;
using Zidium.Agent.AgentTasks.Notifications;
using Zidium.Api.Dto;
using Zidium.Common;
using Zidium.Storage;
using Zidium.Storage.Ef;
using Zidium.TestTools;

namespace Zidium.Agent.Tests
{
    public class MessageNotificationTests : BaseTest
    {
        [Theory]
        [InlineData(SubscriptionChannel.Telegram)]
        [InlineData(SubscriptionChannel.VKontakte)]
        public void SendMessageNotificationTest(SubscriptionChannel channel)
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
                    Id = Ulid.NewUlid(),
                    UserId = user.Id,
                    EventId = eventResponse.Data.EventId,
                    Type = channel,
                    Address = "address",
                    Status = NotificationStatus.InQueue,
                    CreationDate = DateTime.Now
                };

                context.Notifications.Add(notification);
                notificationId = notification.Id;
                context.SaveChanges();
            }

            // Запустим обработку
            var processor = new MessangerNotificationsProcessor(NullLogger.Instance, new CancellationToken());
            processor.Process(component.Id);

            using (var context = account.GetDbContext())
            {
                // Должно появиться сообщение в очереди
                var command = context.SendMessageCommands.FirstOrDefault(t => t.ReferenceId == notificationId);

                Assert.NotNull(command);
                Assert.NotNull(command.To);
                Assert.NotNull(command.Body);
                Assert.Equal(channel, command.Channel);

                // У уведомления должна появиться ссылка на сообщение
                var notification = context.Notifications.Find(notificationId);

                Assert.Equal(command.Id, notification.SendMessageCommandId);
            }
        }
    }
}