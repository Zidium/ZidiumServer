using System;
using Xunit;
using Zidium.Core.Api;
using Zidium.Storage;
using Zidium.TestTools;

namespace Zidium.Core.Tests.Dispatcher
{
    public class ProcessEventsTests : BaseTest
    {
        /// <summary>
        /// Проверяет, что настройки типа события влияют на устанавливаемые у экземпляра события свойства (важность, признак обработки)
        /// </summary>
        [Fact]
        public void ProcessApplicationErrorTest()
        {
            var account = TestHelper.GetTestAccount();
            var client = account.GetClient();
            var component = account.CreateRandomComponentControl();

            // Отправляем ошибку
            var error = TestHelper.CreateRandomApplicationError(component);
            error.JoinInterval = TimeSpan.Zero;
            error.Importance = Zidium.Api.EventImportance.Alarm;
            error.StartDate = DateTime.Now;
            error.Version = "2.5.0.0";
            var eventResponse = error.Send();
            Assert.True(eventResponse.Success);
            var eventId = eventResponse.Data.EventId;

            // Проверим, что ошибка не обработана и важность равна указанной
            var eventInfo = client.ApiService.GetEventById(eventId);
            Assert.Equal(error.Importance, eventInfo.Data.Importance);
            Assert.Equal(false, eventInfo.Data.IsUserHandled);
            Assert.Equal(error.Version, eventInfo.Data.Version);

            // Отметим в типе события, что для данной версии нужно снижать важность и считать их обработанными
            var dispatcher = account.GetDispatcherClient();
            dispatcher.UpdateEventType(account.Id, new UpdateEventTypeRequestData()
            {
                EventTypeId = eventResponse.Data.EventTypeId,
                DisplayName = null,
                OldVersion = error.Version,
                ImportanceForOld = EventImportance.Warning,
                ImportanceForNew = null,
                UpdateActualEvents = true
            }).Check();

            // Проверим, что старое событие поменялось
            account.SaveAllCaches();
            eventInfo = component.Client.ApiService.GetEventById(eventId);
            Assert.Equal(Zidium.Api.EventImportance.Warning, eventInfo.Data.Importance);
            //Assert.Equal(true, eventInfo.Data.IsUserHandled);

            // Отправим ещё старое событие
            error.StartDate = DateTime.Now;
            eventResponse = error.Send();
            Assert.True(eventResponse.Success);
            eventId = eventResponse.Data.EventId;

            // Проверим, что его параметры сразу поменялись
            eventInfo = client.ApiService.GetEventById(eventId);
            Assert.Equal(Zidium.Api.EventImportance.Warning, eventInfo.Data.Importance);
            //Assert.Equal(true, eventInfo.Data.IsUserHandled);

            // Отправим новое событие
            error.StartDate = DateTime.Now;
            error.Version = "3.0.0.1";
            eventResponse = error.Send();
            Assert.True(eventResponse.Success);
            eventId = eventResponse.Data.EventId;

            // Проверим, что его параметры не изменились
            eventInfo = client.ApiService.GetEventById(eventId);
            Assert.Equal(error.Importance, eventInfo.Data.Importance);
            //Assert.Equal(false, eventInfo.Data.IsUserHandled);
            Assert.Equal(error.Version, eventInfo.Data.Version);

            // Отметим в типе события, что новым событиям тоже нужно снижать важность и считать их обработанными
            dispatcher.UpdateEventType(account.Id, new UpdateEventTypeRequestData()
            {
                EventTypeId = eventResponse.Data.EventTypeId,
                DisplayName = null,
                OldVersion = error.Version,
                ImportanceForOld = EventImportance.Warning,
                ImportanceForNew = EventImportance.Success,
                UpdateActualEvents = true
            }).Check();

            // Отправим новое событие
            error.StartDate = DateTime.Now;
            error.Version = "3.0.0.2";
            eventResponse = error.Send();
            Assert.True(eventResponse.Success);
            eventId = eventResponse.Data.EventId;

            // Проверим, что его параметры сразу поменялись
            eventInfo = client.ApiService.GetEventById(eventId);
            Assert.Equal(Zidium.Api.EventImportance.Success, eventInfo.Data.Importance);
            //Assert.Equal(true, eventInfo.Data.IsUserHandled);
        }

        /// <summary>
        /// Тест проверяет, что события влияют на статус компонента
        /// </summary>
        [Fact]
        public void UpdateComponentStatusTest()
        {
            var account = TestHelper.GetTestAccount();
            var client = account.GetClient();
            var component = account.CreateRandomComponentControl();

            // Отправляем ошибку
            var error = TestHelper.CreateRandomApplicationError(component);
            error.JoinInterval = TimeSpan.Zero;
            error.Importance = Zidium.Api.EventImportance.Alarm;
            error.StartDate = DateTime.Now;
            error.Version = "2.0.0.0";
            var eventResponse = error.Send();
            eventResponse.Check();

            // Проверим, что статус компонента - Alarm
            var state = client.ApiService.GetComponentInternalState(component.Info.Id, false);
            Assert.Equal(Zidium.Api.MonitoringStatus.Alarm, state.Data.Status);

            // Отметим в типе события, что для данной версии нужно снижать важность и считать их обработанными
            var dispatcher = account.GetDispatcherClient();
            var updateData = new UpdateEventTypeRequestData()
            {
                EventTypeId = eventResponse.Data.EventTypeId,
                DisplayName = null,
                OldVersion = error.Version,
                ImportanceForOld = EventImportance.Warning,
                ImportanceForNew = null,
                UpdateActualEvents = true
            };
            dispatcher.UpdateEventType(account.Id, updateData).Check();

            // Проверим, что статус компонента - Warning
            account.SaveAllCaches();
            state = client.ApiService.GetComponentInternalState(component.Info.Id, false);
            Assert.Equal(Zidium.Api.MonitoringStatus.Warning, state.Data.Status);
        }

        [Fact]
        public void UpdateComponentStatus2Test()
        {
            var account = TestHelper.GetTestAccount();
            var client = account.GetClient();
            var component = account.CreateRandomComponentControl();

            // Отправляем ошибку
            var error = TestHelper.CreateRandomApplicationError(component);
            error.JoinInterval = TimeSpan.Zero;
            error.Importance = Zidium.Api.EventImportance.Alarm;
            error.StartDate = DateTime.Now;
            error.Version = "2.0.0.0";
            var eventResponse = error.Send();
            Assert.True(eventResponse.Success);

            // Проверим, что статус компонента Alarm
            var state = component.GetTotalState(false);
            Assert.Equal(Zidium.Api.MonitoringStatus.Alarm, state.Data.Status);
            
            // Отметим в типе события, что для данной версии нужно снижать важность и считать их обработанными
            var dispatcher = account.GetDispatcherClient();
            var updateData = new UpdateEventTypeRequestData()
            {
                EventTypeId = eventResponse.Data.EventTypeId,
                DisplayName = null,
                OldVersion = error.Version,
                ImportanceForOld = EventImportance.Warning,
                ImportanceForNew = null,
                UpdateActualEvents = true
            };
            dispatcher.UpdateEventType(account.Id, updateData).Check();

            // Отправим ещё раз
            error.Send();
            Assert.True(eventResponse.Success);

            // Проверим, что статус компонента Alarm
            account.SaveAllCaches();
            state = client.ApiService.GetComponentTotalState(component.Info.Id, false);
            Assert.Equal(Zidium.Api.MonitoringStatus.Warning, state.Data.Status);
        }
    }
}
