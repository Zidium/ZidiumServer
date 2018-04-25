using System;
using Xunit;
using Zidium.TestTools;

namespace ApiTests_1._0.Events.ComponentEvents
{
    public class AddTests
    {
        [Fact]
        public void SimpleTest()
        {
            var account = TestHelper.GetTestAccount();
            var component = account.CreateRandomComponentControl();
            string type = "testComponentEvent";

            var addResult = component
                .CreateComponentEvent(type)
                .Add();

            component.Client.EventManager.Flush();
            Guid eventId = addResult.EventId.Value;
            var eventObj = component.Client.ApiService.GetEventById(eventId).Data;
            Assert.Equal(eventObj.TypeSystemName, type);
        }

        [Fact]
        public void RandomTest()
        {
            var account = TestHelper.GetTestAccount();
            var component = account.CreateRandomComponentControl();
            var eventData = TestHelper.CreateRandomComponentEvent(component);
            var addResult = eventData.Add();
            component.Client.EventManager.Flush();
            var eventId = addResult.EventId.Value;
            var eventObj = component.Client.ApiService.GetEventById(eventId).Data;
            TestHelper.CheckEvent(eventData, eventObj);
        }
    }
}
