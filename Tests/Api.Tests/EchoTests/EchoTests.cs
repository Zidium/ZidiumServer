using System;
using Xunit;
using Zidium.Common;
using Zidium.TestTools;

namespace Zidium.Api.Tests.Others
{
    public class EchoTests : BaseTest
    {
        [Fact]
        public void ApiServiceTest()
        {
            var account = TestHelper.GetTestAccount();
            var client = account.GetClient();
            var echoMessage = Ulid.NewUlid().ToString();
            var response = client.ApiService.GetEcho(echoMessage);
            response.Check();
            Assert.Equal(echoMessage, response.GetDataAndCheck());
        }
    }
}
