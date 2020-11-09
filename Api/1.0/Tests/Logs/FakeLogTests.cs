using Xunit;
using Zidium.Api;

namespace ApiTests_1._0.Logs
{
    public class FakeLogTests : BaseTest
    {
        [Fact]
        public void FakeSendLogTest()
        {
            var log = new FakeLog();
            log.Info("Test");
            Assert.True(log.IsFake());
        }
    }
}
