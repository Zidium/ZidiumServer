using Xunit;
using Zidium.Api;
using Zidium.TestTools;

namespace ApiTests_1._0.Events.Common
{
    public class EventDefaultValuesTest
    {
        [Fact]
        public void Test()
        {
            // создадим компонент
            var account = TestHelper.GetTestAccount();
            var component = account.CreateRandomComponentControl();

            // отправим ошибку
            var response = component.SendApplicationError("myEvent");
            var eventId = response.Data.EventId;

            // проверим значения по умолчанию
            var eventObj = component.Client.ApiService.GetEventById(eventId).Data;
            Assert.Equal(EventCategory.ApplicationError, eventObj.Category);
            Assert.Equal(EventImportance.Alarm, eventObj.Importance); // по умолчанию важность Alarm

            // отправим событие компонента
            response = component.CreateComponentEvent("componentEvent").Send();
            eventId = response.Data.EventId;

            // проверим значения по умолчанию для событий компонента
            eventObj = component.Client.ApiService.GetEventById(eventId).Data;
            Assert.Equal(EventCategory.ComponentEvent, eventObj.Category);
            Assert.Equal(EventImportance.Unknown, eventObj.Importance); // по умолчанию важность Unknown
        }
    }
}
