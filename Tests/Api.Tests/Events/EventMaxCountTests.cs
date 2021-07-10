using System;
using Xunit;
using Zidium.TestTools;

namespace Zidium.Api.Tests.Events
{
    public class EventMaxCountTests : BaseTest
    {
        [Fact]
        public void Test()
        {
            var account = TestHelper.GetTestAccount();
            var component = account.GetClient().GetRootComponentControl();
            var componentEvent = component.CreateComponentEvent("test event " + Guid.NewGuid());
            componentEvent.Count = int.MaxValue - 100;
            componentEvent.JoinInterval = TimeSpan.FromMinutes(10);

            // при 1-ом добавлении Count не меняется
            var result1 = componentEvent.Add();
            Assert.Equal(result1.Count, int.MaxValue - 100);

            // при 2-ом добавлении Count переполняется
            var result2 = componentEvent.Add();
            Assert.Equal(result2.Count, int.MaxValue);

            // отправляем на сервер 1-ый раз
            component.Client.EventManager.Flush();
            Guid eventId = result1.EventId.Value;

            // при 3-ом добавлении Count не меняется, т.к. уже переполнен
            var result3 = componentEvent.Add();
            component.Client.EventManager.Flush();

            // проверим что в БД все ОК
            var eventResponce = component.Client.ApiService.GetEventById(eventId);
            Assert.Equal(Int32.MaxValue, eventResponce.GetDataAndCheck().Count);
        }
    }
}
