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
    public class SmsNotificationTests : BaseTest
    {
        [Fact]
        public void SendSmsNotificationTest()
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
            var phone = "+7 916 111-22-33";
            Guid notificationId;
            using (var context = account.GetDbContext())
            {
                var notification = new DbNotification()
                {
                    Id = Guid.NewGuid(),
                    UserId = user.Id,
                    EventId = eventResponse.Data.EventId,
                    Type = SubscriptionChannel.Sms,
                    Address = phone,
                    Status = NotificationStatus.InQueue,
                    CreationDate = DateTime.Now
                };

                context.Notifications.Add(notification);
                notificationId = notification.Id;
                context.SaveChanges();
            }

            // Запустим обработку
            var processor = new SmsNotificationsProcessor(LogManager.GetCurrentClassLogger(), new CancellationToken());
            processor.Process(component.Id);

            // Должно появиться sms
            using (var context = account.GetDbContext())
            {
                var smsCommand = context.SendSmsCommands.FirstOrDefault(t => t.ReferenceId == notificationId);

                Assert.NotNull(smsCommand);
                Assert.Equal(phone, smsCommand.Phone);
            }
        }

    }
}