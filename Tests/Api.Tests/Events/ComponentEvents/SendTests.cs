using System;
using Xunit;
using Zidium.TestTools;

namespace Zidium.Api.Tests.Events.ComponentEvents
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
            Guid eventId = response.GetDataAndCheck().EventId;
            var eventObj = component.Client.ApiService.GetEventById(eventId).GetDataAndCheck();
            Assert.Equal(eventObj.TypeSystemName, type);
        }

        [Fact]
        public void RandomTest()
        {
            var account = TestHelper.GetTestAccount();
            var component = account.CreateRandomComponentControl();
            var eventData = TestHelper.CreateRandomComponentEvent(component);
            var eventId = eventData.Send().GetDataAndCheck().EventId;
            var eventObj = component.Client.ApiService.GetEventById(eventId).GetDataAndCheck();
            TestHelper.CheckEvent(eventData, new EventInfo(eventObj));
        }
    }
}
