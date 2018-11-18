using System;
using Zidium.Core.AccountsDb;
using Zidium.Core.Common.Helpers;

namespace Zidium.UserAccount.Models.UnitTests
{
    public class ShowSettingsPingModel
    {
        public Guid UnitTestId { get; set; }
        public string Host { get; set; }
        public TimeSpan Timeout { get; set; }

        public static ShowSettingsPingModel Create(UnitTest unitTest)
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
            return new ShowSettingsPingModel()
            {
                UnitTestId = unitTest.Id,
                Timeout = TimeSpanHelper.FromMilliseconds(ping.TimeoutMs).Value,
                Host = ping.Host
            };
        }
    }
}