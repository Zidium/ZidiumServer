using System;
using Xunit;
using Zidium.TestTools;

namespace ApiTests_1._0.Events.ComponentEvents
{
    public class SendTests : BaseTest
    {
        [Fact]
        public void SimpleTest()
        {
            var account = TestHelper.GetTestAccount();
            var component = account.CreateRandomComponentControl();
            string type = "testComponentEvent";

            var response = component
                .CreateComponentEvent(type)
                .Send();

            Assert.True(response.Success);
            Guid eventId = response.Data.EventId;
            var eventObj = component.Client.ApiService.GetEventById(eventId).Data;
            Assert.Equal(eventObj.TypeSystemName, type);
        }

        [Fact]
        public void RandomTest()
        {
            var account = TestHelper.GetTestAccount();
            var component = account.CreateRandomComponentControl();
            var eventData = TestHelper.CreateRandomComponentEvent(component);
            var eventId = eventData.Send().Data.EventId;
            var eventObj = component.Client.ApiService.GetEventById(eventId).Data;
            TestHelper.CheckEvent(eventData, eventObj);
        }
    }
}
