using System;
using Zidium.Core.AccountsDb;
using Zidium.Core.Common.Helpers;

namespace Zidium.UserAccount.Models.UnitTests
{
    public class OverviewSettingsTcpPortModel
    {
        public Guid UnitTestId { get; set; }
        public TimeSpan Period { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
        public bool Opened { get; set; }

        public static OverviewSettingsTcpPortModel Create(UnitTest unitTest)
        {
            if (unitTest == null)
            {
                throw new ArgumentNullException("unitTest");
            }
            var rule = unitTest.TcpPortRule;
            if (rule == null)
            {
                throw new Exception("unittest TcpPortRule data is null");
            }
            return new OverviewSettingsTcpPortModel()
            {
                UnitTestId = unitTest.Id,
                Period = TimeSpanHelper.FromSeconds(unitTest.PeriodSeconds).Value,
                Host = rule.Host,
                Port = rule.Port,
                Opened = rule.Opened
            };
        }
    }
}