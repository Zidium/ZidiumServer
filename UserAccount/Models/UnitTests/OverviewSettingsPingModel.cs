using System;
using Zidium.Core.Common.Helpers;
using Zidium.Storage;

namespace Zidium.UserAccount.Models.UnitTests
{
    public class OverviewSettingsPingModel
    {
        public Guid UnitTestId { get; set; }
        public TimeSpan Period { get; set; }
        public string Host { get; set; }
        public TimeSpan Timeout { get; set; }

        public static OverviewSettingsPingModel Create(UnitTestForRead unitTest, IStorage storage)
        {
            if (unitTest == null)
            {
                throw new ArgumentNullException("unitTest");
            }

            var rule = storage.UnitTestPingRules.GetOneByUnitTestId(unitTest.Id);

            return new OverviewSettingsPingModel()
            {
                UnitTestId = unitTest.Id,
                Period = TimeSpanHelper.FromSeconds(unitTest.PeriodSeconds).Value,
                Timeout = TimeSpanHelper.FromMilliseconds(rule.TimeoutMs).Value,
                Host = rule.Host
            };
        }
    }
}