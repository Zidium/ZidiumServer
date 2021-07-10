using Xunit;
using Zidium.TestTools;

namespace Zidium.Api.Tests.Events.Common
{
    public class EventTypesTests : BaseTest
    {
        [Fact]
        public void Test()
        {
            var account = TestHelper.GetTestAccount();
            var client = account.GetClient();
            var component = client.GetRootComponentControl();
            var response1 = component.SendApplicationError("test");
            var response2 = component.CreateComponentEvent("test").Send();
            Assert.True(response1.Success);
            Assert.True(response2.Success);
            Assert.NotEqual(response1.GetDataAndCheck().EventTypeId, response2.GetDataAndCheck().EventTypeId);
            Assert.NotEqual(response1.GetDataAndCheck().EventId, response2.GetDataAndCheck().EventId);
        }
    }
}
