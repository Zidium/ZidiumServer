using System;
using Zidium.Api;
using Xunit;
using Zidium.TestTools;

namespace ApiTests_1._0.Events.Common
{
    public class FutureEventTests : BaseTest
    {
        /// <summary>
        /// Тест проверяет, что нельзя отправить событие с startDate силно больше, 
        /// чем текущая дата (это называется событие из будущего)
        /// </summary>
        [Fact]
        public void Test()
        {
            // создадим компонент
            var account = TestHelper.GetTestAccount();
            var component = account.CreateRandomComponentControl();

            // статус у компонента должен быть "Неизвестно"
            var statusResponse = component.GetTotalState(false);
            Assert.True(statusResponse.Success);
            Assert.Equal(MonitoringStatus.Unknown, statusResponse.Data.Status);

            // отправим событие из будущего = получим ошибку
            var eventData = component.CreateComponentEvent("Событие из будущего!");
            eventData.StartDate = DateTime.Now.AddHours(1);
            eventData.Importance = EventImportance.Success;
            eventData.JoinInterval = TimeSpan.FromDays(1000);

            var eventResponse = eventData.Send();
            Assert.False(eventResponse.Success); // ошибка
            Assert.Equal(ResponseCode.FutureEvent, eventResponse.Code);

            // проверим, что статус изменился (т.к. диспетчер сам отправил предупреждение о событии из будущего!)
            statusResponse = component.GetTotalState(false);
            Assert.True(statusResponse.Success);
            Assert.Equal(MonitoringStatus.Warning, statusResponse.Data.Status);
            //var eventData2 = component.GetEventById(statusResponse.Data.LastEventId.Value);
            //Assert.True(eventData2.Data.TypeDisplayName.Contains("Время начала события значительно больше, чем текущее время"));
        }
    }
}
