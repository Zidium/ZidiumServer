using System;
using Xunit;
using Zidium.Agent.AgentTasks.UnitTests.VirusTotal.Client;

namespace Zidium.Agent.Tests
{
    public class VirusTotalHelperTests : BaseTest
    {
        [Fact]
        public void ParseDateTimeTest()
        {
            var date = VirusTotalHelper.ParseDateTime("2020-02-03 10:18:57");
            Assert.Equal(2020, date.Year);
            Assert.Equal(2, date.Month);
            Assert.Equal(3, date.Day);
            Assert.Equal(10, date.Hour);
            Assert.Equal(18, date.Minute);
            Assert.Equal(57, date.Second);
            Assert.Equal(DateTimeKind.Utc, date.Kind);
        }
    }
}
