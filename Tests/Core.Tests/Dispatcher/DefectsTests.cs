using System;
using Xunit;
using Zidium.Core.AccountsDb;
using Zidium.Core.Api;
using Zidium.Storage;
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
            var storage = TestHelper.GetStorage(account.Id);

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
            var eventType = storage.EventTypes.GetOneOrNullBySystemName(eventTypeName);

            var service = new DefectService(storage);
            var defect = service.GetOrCreateDefectForEventType(account.Id, eventType, user, user, "Comment for open");

            defectId = defect.Id;

            // Закроем дефект
            defect = storage.Defects.GetOneById(defectId);
            service.ChangeStatus(account.Id, defect, DefectStatus.Closed, user, "Comment for close");

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
            Assert.Equal(DefectStatus.Reopened, service.GetStatus(defectId));
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
            var storage = TestHelper.GetStorage(account.Id);

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
            var eventType = storage.EventTypes.GetOneOrNullBySystemName(eventTypeName);

            var service = new DefectService(storage);
            var defect = service.GetOrCreateDefectForEventType(account.Id, eventType, user, user, "Comment for open");

            defectId = defect.Id;

            // Закроем дефект
            defect = storage.Defects.GetOneById(defectId);
            service.ChangeStatus(account.Id, defect, DefectStatus.Closed, user, "Comment for close");

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
            Assert.Equal(DefectStatus.Reopened, service.GetStatus(defectId));
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
            var storage = TestHelper.GetStorage(account.Id);

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
            var eventType = storage.EventTypes.GetOneOrNullBySystemName(eventTypeName);

            var service = new DefectService(storage);
            var defect = service.GetOrCreateDefectForEventType(account.Id, eventType, user, user, "Comment for open");

            defectId = defect.Id;

            // Закроем дефект
            defect = storage.Defects.GetOneById(defectId);
            service.ChangeStatus(account.Id, defect, DefectStatus.Closed, user, "Comment for close");

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
            Assert.Equal(DefectStatus.Closed, service.GetStatus(defectId));
        }
    }
}
