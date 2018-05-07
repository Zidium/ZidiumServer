using System;
using Xunit;
using Zidium.Agent.AgentTasks;
using Zidium.Core.AccountsDb;

namespace Zidium.Core.Tests.AgentTests
{
    public class PingCheckTests
    {
        [Fact]
        public void CommonTest()
        {
            var pingResult = PingProcessor.Ping("ya.ru", TimeSpan.FromSeconds(5));
            Assert.Equal(PingErrorCode.Success, pingResult.ErrorCode);

            pingResult = PingProcessor.Ping("ya2323.ru", TimeSpan.FromSeconds(5));
            Assert.Equal(PingErrorCode.UnknownDomain, pingResult.ErrorCode);

            pingResult = PingProcessor.Ping("192.168.0.254", TimeSpan.FromSeconds(5));
            Assert.Equal(PingErrorCode.Timeout, pingResult.ErrorCode);
        }
    }
}
