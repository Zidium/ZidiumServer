using Xunit;
using Zidium.TestTools;

namespace ApiTests_1._0.Events.Common
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
            Assert.NotEqual(response1.Data.EventTypeId, response2.Data.EventTypeId);
            Assert.NotEqual(response1.Data.EventId, response2.Data.EventId);
        }
    }
}
