﻿using System;
using Xunit;
using Zidium.Agent.AgentTasks;
using Zidium.Storage;

namespace Zidium.Agent.Tests
{
    public class PingCheckTests : BaseTest
    {
        [Fact]
        public void CommonTest()
        {
            var pingResult = PingProcessor.Ping("ya.ru", TimeSpan.FromSeconds(5));
            Assert.Equal(PingErrorCode.Success, pingResult.ErrorCode);

            pingResult = PingProcessor.Ping("ya2323.ru", TimeSpan.FromSeconds(5));
            Assert.Equal(PingErrorCode.UnknownDomain, pingResult.ErrorCode);

            pingResult = PingProcessor.Ping("192.168.254.254", TimeSpan.FromSeconds(5));
            Assert.Equal(PingErrorCode.Timeout, pingResult.ErrorCode);
        }
    }
}
