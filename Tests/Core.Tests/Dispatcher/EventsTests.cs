using System;
using System.Linq;
using Xunit;
using Zidium.Api;
using Zidium.Core.Api;
using Zidium.TestTools;
using EventCategory = Zidium.Core.Api.EventCategory;
using EventImportance = Zidium.Core.Api.EventImportance;
using MonitoringStatus = Zidium.Core.Api.MonitoringStatus;
using SendEventData = Zidium.Core.Api.SendEventData;

namespace Zidium.Core.Tests.Dispatcher
{
    public class EventsTests
    {
        [Fact]
        public void DisableRedComponentTest()
        {
            // Создадим компонент
            var account = TestHelper.GetTestAccount();
            var component = account.CreateTestApplicationComponent();
            var dispatcher = TestHelper.GetDispatcherClient();

            // Отключим компонент
            var disableData = new SetComponentDisableRequestData()
            {
                ComponentId = component.Id
            };
            dispatcher.SetComponentDisable(account.Id, disableData);

            // Отправим красное событие
            var eventType = TestHelper.GetTestEventType(account.Id);
            var eventResponse = dispatcher.SendEvent(account.Id, new SendEventData()
            {
                TypeSystemName = eventType.SystemName,
                ComponentId = component.Id,
                Category = EventCategory.ApplicationError,
                Importance = EventImportance.Alarm
            });
            Assert.Equal(ResponseCode.ObjectDisabled, eventResponse.Code);

            // Проверим события
            using (var context = account.CreateAccountDbContext())
            {
                var eventRepository = context.GetEventRepository();
                var events = eventRepository.QueryAll(component.Id).ToList();

                // Должно быть 2 серого события ("Нет данных", "Выключено") внешнего статуса
                var externalGrayEvents = events
                    .Where(t => t.Category == EventCategory.ComponentInternalStatus && t.Importance == EventImportance.Unknown)
                    .ToList();

                Assert.Equal(2, externalGrayEvents.Count);

                // Должно быть 2 серого внутреннего статуса
                Assert.Equal(2, events.Count(t => t.Category == EventCategory.ComponentInternalStatus && t.Importance == EventImportance.Unknown));
            }

            // Проверим, что компонент не стал красным
            var stateResponse = dispatcher.GetComponentTotalState(account.Id, component.Id);
            Assert.NotEqual(MonitoringStatus.Alarm, stateResponse.Data.Status);

            // Включим компонент
            dispatcher.SetComponentEnable(account.Id, component.Id);

            // Проверим, что компонент не стал красным
            stateResponse = dispatcher.GetComponentTotalState(account.Id, component.Id);
            Assert.NotEqual(MonitoringStatus.Alarm, stateResponse.Data.Status);

            // Проверим события
            using (var context = account.CreateAccountDbContext())
            {
                var eventRepository = context.GetEventRepository();
                var events = eventRepository.QueryAll(component.Id).ToList();

                // Должно быть 3 серых события внешнего статуса
                Assert.Equal(3, events.Count(t => t.Category == EventCategory.ComponentInternalStatus && t.Importance == EventImportance.Unknown));

                // Должно быть 3 серых события внутреннего статуса
                Assert.Equal(3, events.Count(t => t.Category == EventCategory.ComponentInternalStatus && t.Importance == EventImportance.Unknown));

                // Красных событий статуса быть не должно
                Assert.Equal(0, events.Count(t => t.Category == EventCategory.ComponentInternalStatus && t.Importance == EventImportance.Alarm));
                Assert.Equal(0, events.Count(t => t.Category == EventCategory.ComponentInternalStatus && t.Importance == EventImportance.Alarm));
            }
        }

        /// <summary>
        /// Тест проверяет, что в типе события можно задать перекрытие важности события
        /// </summary>
        [Fact]
        public void EventImportanceOverrideTest()
        {
            // Создадим компонент
            var account = TestHelper.GetTestAccount();
            var component = account.CreateTestApplicationComponent();
            var dispatcher = TestHelper.GetDispatcherClient();
            var eventTypeSystemName = "EventType.Test." + Guid.NewGuid();

            // Отправим красное событие
            var firstEventResponse = dispatcher.SendEvent(account.Id, new SendEventData()
            {
                TypeSystemName = eventTypeSystemName,
                ComponentId = component.Id,
                Category = EventCategory.ApplicationError,
                Importance = EventImportance.Alarm
            });
            var firstEventInfo = firstEventResponse.Data;

            // Обновим тип события - укажем серую важность
            var updateTypeResponse = dispatcher.UpdateEventType(account.Id, new UpdateEventTypeRequestData()
            {
                EventTypeId = firstEventInfo.EventTypeId,
                ImportanceForNew = EventImportance.Unknown
            });
            updateTypeResponse.Check();

            // Снова отправим красное событие
            var secondEventResponse = dispatcher.SendEvent(account.Id, new SendEventData()
            {
                TypeSystemName = eventTypeSystemName,
                ComponentId = component.Id,
                Category = EventCategory.ApplicationError,
                Importance = EventImportance.Alarm,
                JoinInterval = 0
            });
            var secondEventInfo = secondEventResponse.Data;

            // Проверим, что событие получило тот же тип
            Assert.Equal(firstEventInfo.EventTypeId, secondEventInfo.EventTypeId);

            // Проверим, что событие сохранилось как серое
            var eventResponse = dispatcher.GetEventById(account.Id, secondEventInfo.EventId);
            var eventData = eventResponse.Data;
            Assert.Equal(EventImportance.Unknown, eventData.Importance);
        }

        [Fact]
        public void SendEventPerformanceTest()
        {
            // Создадим компонент
            var account = TestHelper.GetTestAccount();
            var component = account.CreateTestApplicationComponent();
            var dispatcher = TestHelper.GetDispatcherClient();
            var eventTypeSystemName = "EventType.Test." + Guid.NewGuid();

            // Отправим зелёное событие
            var firstEventResponse = dispatcher.SendEvent(account.Id, new SendEventData()
            {
                TypeSystemName = eventTypeSystemName,
                ComponentId = component.Id,
                Category = EventCategory.ComponentEvent,
                Importance = EventImportance.Success
            });
            firstEventResponse.Check();

            // Отправим красное событие
            var secondEventResponse = dispatcher.SendEvent(account.Id, new SendEventData()
            {
                TypeSystemName = eventTypeSystemName,
                ComponentId = component.Id,
                Category = EventCategory.ComponentEvent,
                Importance = EventImportance.Alarm,
                JoinInterval = 0
            });
            secondEventResponse.Check();
        }

    }
}
