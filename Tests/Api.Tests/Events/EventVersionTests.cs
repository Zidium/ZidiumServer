using System;
using Xunit;
using Zidium.TestTools;

namespace Zidium.Api.Tests.Events
{
    public class EventVersionTests : BaseTest
    {
        /// <summary>
        /// Тест проверяет, что версия компонента влияет на версию события, а версия события НЕ влияет на версию компонента.
        /// </summary>
        [Fact]
        public void Test1()
        {
            // отправка от компонента без версии
            var account = TestHelper.GetTestAccount();
            var client = account.GetClient();
            var root = client.GetRootComponentControl();
            var type = client.GetOrCreateComponentTypeControl("testType");

            var createData = new GetOrCreateComponentData("TestComponent" + Guid.NewGuid(), type)
            {
                Version = null
            };
            var component = root.GetOrCreateChildComponentControl(createData);

            var eventData = component.CreateComponentEvent("Запуск ракеты");
            var response = eventData.Send();
            Assert.True(response.Success);
            var eventInfo = client.ApiService.GetEventById(response.Data.EventId).GetDataAndCheck();
            Assert.Null(eventInfo.Version);

            // установим компоненту версию 1.0
            createData.Version = "1.0";
            component = root.GetOrCreateChildComponentControl(createData);
            eventData = component.CreateComponentEvent("Запуск ракеты");
            //component.Info.Version = "1.0";
            response = eventData.Send();
            Assert.True(response.Success);
            eventInfo = client.ApiService.GetEventById(response.Data.EventId).GetDataAndCheck();
            Assert.Equal("1.0", eventInfo.Version);

            // укажем у события версию явно 2.0
            eventData.Version = "2.0";
            response = eventData.Send();
            Assert.True(response.Success);
            eventInfo = client.ApiService.GetEventById(response.Data.EventId).GetDataAndCheck();
            Assert.Equal("2.0", eventInfo.Version);

            // проверим, что у компонента версия не поменялась
            eventData.Version = null;
            response = eventData.Send();
            Assert.True(response.Success);
            eventInfo = client.ApiService.GetEventById(response.Data.EventId).GetDataAndCheck();
            Assert.Equal("1.0", eventInfo.Version);
            var componentCopy = component.Client.ApiService.GetComponentById(component.Info.Id);
            Assert.Equal("1.0", componentCopy.GetDataAndCheck().Version);
        }

        /// <summary>
        /// Тест проверяет, что версия влияет на склейку
        /// </summary>
        [Fact]
        public void JoinTest()
        {
            var account = TestHelper.GetTestAccount();
            var client = account.GetClient();
            var component = client.GetRootComponentControl().GetOrCreateChildComponentControl("type", "name");
            var eventData = component.CreateComponentEvent("Запуск ракеты");
            eventData.JoinInterval = TimeSpan.FromHours(1);

            // отправим 1-ое событие
            eventData.Version = "1.0";
            var response = eventData.Send();
            Assert.True(response.Success);
            var eventInfo = component.Client.ApiService.GetEventById(response.Data.EventId).GetDataAndCheck();
            long joinKey = eventInfo.JoinKeyHash;
            Guid eventId = eventInfo.Id;
            Assert.Equal("1.0", eventInfo.Version);

            // 2-ое событие должно склеится (интервал склейки = 1 час)
            response = eventData.Send();
            eventInfo = component.Client.ApiService.GetEventById(response.Data.EventId).GetDataAndCheck();
            Assert.Equal(eventId, eventInfo.Id);
            Assert.Equal(joinKey, eventInfo.JoinKeyHash);
            Assert.Equal("1.0", eventInfo.Version);

            // 3-ое событие НЕ должно склеится (изменилась версия)
            eventData.Version = "2.0";
            response = eventData.Send();
            eventInfo = component.Client.ApiService.GetEventById(response.Data.EventId).GetDataAndCheck();
            Assert.NotEqual(eventId, eventInfo.Id);
            Assert.Equal(joinKey, eventInfo.JoinKeyHash); // ВНИМАНИЕ! Ключ склейки не изменился
            Assert.Equal("2.0", eventInfo.Version);
        }
    }
}
