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
    public class SmsNotificationTests
    {
        [Fact]
        public void SendSmsNotificationTest()
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
            var phone = "+7 916 111-22-33";
            Guid notificationId;
            using (var context = account.CreateAccountDbContext())
            {
                var notification = new Notification()
                {
                    Id = Guid.NewGuid(),
                    UserId = user.Id,
                    EventId = eventResponse.Data.EventId,
                    Type = SubscriptionChannel.Sms,
                    Address = phone,
                    Status = NotificationStatus.InQueue,
                    CreationDate = DateTime.Now
                };

                var notificationRepository = context.GetNotificationRepository();
                notificationRepository.Add(notification);
                notificationId = notification.Id;
                context.SaveChanges();
            }

            // Запустим обработку
            var processor = new SmsNotificationsProcessor(LogManager.GetCurrentClassLogger(), new CancellationToken());
            processor.Process(account.Id, component.Id);
            Assert.Null(processor.DbProcessor.FirstException);

            // Должно появиться sms
            using (var context = account.CreateAccountDbContext())
            {
                var smsCommandRepository = context.GetSendSmsCommandRepository();
                var smsCommand = smsCommandRepository.QueryAll().FirstOrDefault(t => t.ReferenceId == notificationId);

                Assert.NotNull(smsCommand);
                Assert.Equal(phone, smsCommand.Phone);
            }
        }

    }
}
