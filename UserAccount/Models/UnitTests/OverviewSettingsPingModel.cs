using System;
using Zidium.Core.AccountsDb;
using Zidium.Core.Common.Helpers;

namespace Zidium.UserAccount.Models.UnitTests
{
    public class OverviewSettingsPingModel
    {
        public Guid UnitTestId { get; set; }
        public TimeSpan Period { get; set; }
        public string Host { get; set; }
        public TimeSpan Timeout { get; set; }

        public static OverviewSettingsPingModel Create(UnitTest unitTest)
        {
            if (unitTest == null)
            {
                throw new ArgumentNullException("unitTest");
            }
            var ping = unitTest.PingRule;
            if (ping == null)
            {
                throw new Exception("unittest ping data is null");
            }
            return new OverviewSettingsPingModel()
            {
                UnitTestId = unitTest.Id,
                Period = TimeSpanHelper.FromSeconds(unitTest.PeriodSeconds).Value,
                Timeout = TimeSpanHelper.FromMilliseconds(ping.TimeoutMs).Value,
                Host = ping.Host
            };
        }
    }
}