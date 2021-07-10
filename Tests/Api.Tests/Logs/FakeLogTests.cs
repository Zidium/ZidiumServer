using Xunit;

namespace Zidium.Api.Tests.Logs
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
