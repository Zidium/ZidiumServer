using System;
using Xunit;
using Zidium.TestTools;

namespace ApiTests_1._0.Others
{
    public class EchoTests : BaseTest
    {
        [Fact]
        public void ApiServiceTest()
        {
            var account = TestHelper.GetTestAccount();
            var client = account.GetClient();
            var echoMessage = Guid.NewGuid().ToString();
            var response = client.ApiService.GetEcho(echoMessage);
            response.Check();
            Assert.Equal(echoMessage, response.Data);
        }
    }
}
