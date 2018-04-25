using System;
using Xunit;
using Zidium.Core.AccountsDb;
using Zidium.Core.AccountsDb.Classes;
using Zidium.Core.Api;
using Zidium.TestTools;

namespace Zidium.Core.Tests.Dispatcher
{
    public class DefectsTests
    {
        /// <summary>
        /// Тест проверяет, что событие без версии переоткрывает закрытый дефект
        /// </summary>
        [Fact]
        public void AutoReopenDefectForEventWithoutVersionTest()
        {
            var account = TestHelper.GetTestAccount();
            var user = TestHelper.GetAccountAdminUser(account.Id);
            var component = TestHelper.GetTestApplicationComponent(account);
            var dispatcher = TestHelper.GetDispatcherClient();

            // Отправим ошибку
            var eventTypeName = "TestEventType." + Guid.NewGuid();
            var eventResponse = dispatcher.SendEvent(account.Id, new SendEventData()
            {
                TypeSystemName = eventTypeName,
                ComponentId = component.Id,
                Category = EventCategory.ApplicationError,
                Importance = EventImportance.Alarm
            });
            eventResponse.Check();

            // Создадим по ней дефект
            Guid defectId;
            using (var context = AccountDbContext.CreateFromAccountId(account.Id))
            {
                var eventTypeRepository = context.GetEventTypeRepository();
                var eventType = eventTypeRepository.GetOneOrNullBySystemName(eventTypeName);

                var service = context.GetDefectService();
                var defect = service.GetOrCreateDefectForEventType(account.Id, eventType, user, user, "Comment for open");
                context.SaveChanges();

                defectId = defect.Id;
            }

            // Закроем дефект
            using (var context = AccountDbContext.CreateFromAccountId(account.Id))
            {
                var defectRepository = context.GetDefectRepository();
                var defect = defectRepository.GetById(defectId);

                var service = context.GetDefectService();
                service.ChangeStatus(account.Id, defect, DefectStatus.Closed, user, "Comment for close");
                context.SaveChanges();
            }

            // Снова отправим ошибку
            eventResponse = dispatcher.SendEvent(account.Id, new SendEventData()
            {
                TypeSystemName = eventTypeName,
                ComponentId = component.Id,
                Category = EventCategory.ApplicationError,
                Importance = EventImportance.Alarm
            });
            eventResponse.Check();

            // Проверим, что дефект переоткрылся
            using (var context = AccountDbContext.CreateFromAccountId(account.Id))
            {
                var defectRepository = context.GetDefectRepository();
                var defect = defectRepository.GetById(defectId);

                Assert.Equal(DefectStatus.ReOpen, defect.GetStatus());
            }
        }

        /// <summary>
        /// Тест проверяет, что событие новой версии переоткрывает закрытый дефект
        /// </summary>
        [Fact]
        public void AutoReopenDefectForNewEvent()
        {
            var account = TestHelper.GetTestAccount();
            var user = TestHelper.GetAccountAdminUser(account.Id);
            var component = TestHelper.GetTestApplicationComponent(account);
            var dispatcher = TestHelper.GetDispatcherClient();

            // Отправим ошибку
            var eventTypeName = "TestEventType." + Guid.NewGuid();
            var eventResponse = dispatcher.SendEvent(account.Id, new SendEventData()
            {
                TypeSystemName = eventTypeName,
                ComponentId = component.Id,
                Category = EventCategory.ApplicationError,
                Importance = EventImportance.Alarm,
                Version = "1.0.0.0"
            });
            eventResponse.Check();

            // Создадим по ней дефект
            Guid defectId;
            using (var context = AccountDbContext.CreateFromAccountId(account.Id))
            {
                var eventTypeRepository = context.GetEventTypeRepository();
                var eventType = eventTypeRepository.GetOneOrNullBySystemName(eventTypeName);

                var service = context.GetDefectService();
                var defect = service.GetOrCreateDefectForEventType(account.Id, eventType, user, user, "Comment for open");
                context.SaveChanges();

                defectId = defect.Id;
            }

            // Закроем дефект
            using (var context = AccountDbContext.CreateFromAccountId(account.Id))
            {
                var defectRepository = context.GetDefectRepository();
                var defect = defectRepository.GetById(defectId);

                var service = context.GetDefectService();
                service.ChangeStatus(account.Id, defect, DefectStatus.Closed, user, "Comment for close");
                context.SaveChanges();
            }

            // Отправим ошибку новой версии
            eventResponse = dispatcher.SendEvent(account.Id, new SendEventData()
            {
                TypeSystemName = eventTypeName,
                ComponentId = component.Id,
                Category = EventCategory.ApplicationError,
                Importance = EventImportance.Alarm,
                Version = "1.1.0.0"
            });
            eventResponse.Check();

            // Проверим, что дефект переоткрылся
            using (var context = AccountDbContext.CreateFromAccountId(account.Id))
            {
                var defectRepository = context.GetDefectRepository();
                var defect = defectRepository.GetById(defectId);

                Assert.Equal(DefectStatus.ReOpen, defect.GetStatus());
            }
        }

        /// <summary>
        /// Тест проверяет, что событие старой версии не переоткрывает закрытый дефект
        /// </summary>
        [Fact]
        public void NoAutoReopenDefectForOldEventTest()
        {
            var account = TestHelper.GetTestAccount();
            var user = TestHelper.GetAccountAdminUser(account.Id);
            var component = TestHelper.GetTestApplicationComponent(account);
            var dispatcher = TestHelper.GetDispatcherClient();

            // Отправим ошибку
            var eventTypeName = "TestEventType." + Guid.NewGuid();
            var eventResponse = dispatcher.SendEvent(account.Id, new SendEventData()
            {
                TypeSystemName = eventTypeName,
                ComponentId = component.Id,
                Category = EventCategory.ApplicationError,
                Importance = EventImportance.Alarm,
                Version = "1.0.0.0"
            });
            eventResponse.Check();

            // Создадим по ней дефект
            Guid defectId;
            using (var context = AccountDbContext.CreateFromAccountId(account.Id))
            {
                var eventTypeRepository = context.GetEventTypeRepository();
                var eventType = eventTypeRepository.GetOneOrNullBySystemName(eventTypeName);

                var service = context.GetDefectService();
                var defect = service.GetOrCreateDefectForEventType(account.Id, eventType, user, user, "Comment for open");
                context.SaveChanges();

                defectId = defect.Id;
            }

            // Закроем дефект
            using (var context = AccountDbContext.CreateFromAccountId(account.Id))
            {
                var defectRepository = context.GetDefectRepository();
                var defect = defectRepository.GetById(defectId);

                var service = context.GetDefectService();
                service.ChangeStatus(account.Id, defect, DefectStatus.Closed, user, "Comment for close");
                context.SaveChanges();
            }

            // Снова отправим ошибку той же версии (теперь это старая версия)
            eventResponse = dispatcher.SendEvent(account.Id, new SendEventData()
            {
                TypeSystemName = eventTypeName,
                ComponentId = component.Id,
                Category = EventCategory.ApplicationError,
                Importance = EventImportance.Alarm,
                Version = "1.0.0.0"
            });
            eventResponse.Check();

            // Проверим, что дефект не переоткрылся
            using (var context = AccountDbContext.CreateFromAccountId(account.Id))
            {
                var defectRepository = context.GetDefectRepository();
                var defect = defectRepository.GetById(defectId);

                Assert.Equal(DefectStatus.Closed, defect.GetStatus());
            }
        }
    }
}
