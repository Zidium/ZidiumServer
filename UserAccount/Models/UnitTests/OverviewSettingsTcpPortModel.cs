using System;
using Zidium.Core.Common.Helpers;
using Zidium.Storage;

namespace Zidium.UserAccount.Models.UnitTests
{
    public class OverviewSettingsTcpPortModel
    {
        public Guid UnitTestId { get; set; }
        public TimeSpan Period { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
        public bool Opened { get; set; }

        public static OverviewSettingsTcpPortModel Create(UnitTestForRead unitTest, IStorage storage)
        {
            if (unitTest == null)
            {
                throw new ArgumentNullException("unitTest");
            }

            var rule = storage.UnitTestTcpPortRules.GetOneByUnitTestId(unitTest.Id);
            
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