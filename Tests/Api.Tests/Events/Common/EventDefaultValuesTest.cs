using Xunit;
using Zidium.Api.Dto;
using Zidium.TestTools;

namespace Zidium.Api.Tests.Events.Common
{
    public class EventDefaultValuesTest : BaseTest
    {
        [Fact]
        public void Test()
        {
            // создадим компонент
            var account = TestHelper.GetTestAccount();
            var component = account.CreateRandomComponentControl();

            // отправим ошибку
            var response = component.SendApplicationError("myEvent");
            var eventId = response.GetDataAndCheck().EventId;

            // проверим значения по умолчанию
            var eventObj = component.Client.ApiService.GetEventById(eventId).GetDataAndCheck();
            Assert.Equal(EventCategory.ApplicationError, eventObj.Category);
            Assert.Equal(EventImportance.Alarm, eventObj.Importance); // по умолчанию важность Alarm

            // отправим событие компонента
            response = component.CreateComponentEvent("componentEvent").Send();
            eventId = response.GetDataAndCheck().EventId;

            // проверим значения по умолчанию для событий компонента
            eventObj = component.Client.ApiService.GetEventById(eventId).GetDataAndCheck();
            Assert.Equal(EventCategory.ComponentEvent, eventObj.Category);
            Assert.Equal(EventImportance.Unknown, eventObj.Importance); // по умолчанию важность Unknown
        }
    }
}
