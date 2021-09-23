using System;
using Xunit;
using Zidium.Core.AccountsDb;
using Zidium.Api.Dto;
using Zidium.Storage;
using Zidium.TestTools;
using Zidium.Common;

namespace Zidium.Core.Tests.Dispatcher
{
    public class DefectsTests : BaseTest
    {
        /// <summary>
        /// Тест проверяет, что событие без версии переоткрывает закрытый дефект
        /// </summary>
        [Fact]
        public void AutoReopenDefectForEventWithoutVersionTest()
        {
            var user = TestHelper.GetAccountAdminUser();
            var component = TestHelper.GetTestApplicationComponent();
            var dispatcher = TestHelper.GetDispatcherClient();
            var storage = TestHelper.GetStorage();

            // Отправим ошибку
            var eventTypeName = "TestEventType." + Ulid.NewUlid();
            var eventResponse = dispatcher.SendEvent(new SendEventRequestDataDto()
            {
                TypeSystemName = eventTypeName,
                ComponentId = component.Id,
                Category = SendEventCategory.ApplicationError,
                Importance = EventImportance.Alarm
            });
            eventResponse.Check();

            // Создадим по ней дефект
            Guid defectId;
            var eventType = storage.EventTypes.GetOneOrNullBySystemName(eventTypeName);

            var service = new DefectService(storage);
            var defect = service.GetOrCreateDefectForEventType(eventType, user, user, "Comment for open");

            defectId = defect.Id;

            // Закроем дефект
            defect = storage.Defects.GetOneById(defectId);
            service.ChangeStatus(defect, DefectStatus.Closed, user, "Comment for close");

            // Снова отправим ошибку
            eventResponse = dispatcher.SendEvent(new SendEventRequestDataDto()
            {
                TypeSystemName = eventTypeName,
                ComponentId = component.Id,
                Category = SendEventCategory.ApplicationError,
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
            var user = TestHelper.GetAccountAdminUser();
            var component = TestHelper.GetTestApplicationComponent();
            var dispatcher = TestHelper.GetDispatcherClient();
            var storage = TestHelper.GetStorage();

            // Отправим ошибку
            var eventTypeName = "TestEventType." + Ulid.NewUlid();
            var eventResponse = dispatcher.SendEvent(new SendEventRequestDataDto()
            {
                TypeSystemName = eventTypeName,
                ComponentId = component.Id,
                Category = SendEventCategory.ApplicationError,
                Importance = EventImportance.Alarm,
                Version = "1.0.0.0"
            });
            eventResponse.Check();

            // Создадим по ней дефект
            Guid defectId;
            var eventType = storage.EventTypes.GetOneOrNullBySystemName(eventTypeName);

            var service = new DefectService(storage);
            var defect = service.GetOrCreateDefectForEventType(eventType, user, user, "Comment for open");

            defectId = defect.Id;

            // Закроем дефект
            defect = storage.Defects.GetOneById(defectId);
            service.ChangeStatus(defect, DefectStatus.Closed, user, "Comment for close");

            // Отправим ошибку новой версии
            eventResponse = dispatcher.SendEvent(new SendEventRequestDataDto()
            {
                TypeSystemName = eventTypeName,
                ComponentId = component.Id,
                Category = SendEventCategory.ApplicationError,
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
            var user = TestHelper.GetAccountAdminUser();
            var component = TestHelper.GetTestApplicationComponent();
            var dispatcher = TestHelper.GetDispatcherClient();
            var storage = TestHelper.GetStorage();

            // Отправим ошибку
            var eventTypeName = "TestEventType." + Ulid.NewUlid();
            var eventResponse = dispatcher.SendEvent(new SendEventRequestDataDto()
            {
                TypeSystemName = eventTypeName,
                ComponentId = component.Id,
                Category = SendEventCategory.ApplicationError,
                Importance = EventImportance.Alarm,
                Version = "1.0.0.0"
            });
            eventResponse.Check();

            // Создадим по ней дефект
            Guid defectId;
            var eventType = storage.EventTypes.GetOneOrNullBySystemName(eventTypeName);

            var service = new DefectService(storage);
            var defect = service.GetOrCreateDefectForEventType(eventType, user, user, "Comment for open");

            defectId = defect.Id;

            // Закроем дефект
            defect = storage.Defects.GetOneById(defectId);
            service.ChangeStatus(defect, DefectStatus.Closed, user, "Comment for close");

            // Снова отправим ошибку той же версии (теперь это старая версия)
            eventResponse = dispatcher.SendEvent(new SendEventRequestDataDto()
            {
                TypeSystemName = eventTypeName,
                ComponentId = component.Id,
                Category = SendEventCategory.ApplicationError,
                Importance = EventImportance.Alarm,
                Version = "1.0.0.0"
            });
            eventResponse.Check();

            // Проверим, что дефект не переоткрылся
            Assert.Equal(DefectStatus.Closed, service.GetStatus(defectId));
        }
    }
}
