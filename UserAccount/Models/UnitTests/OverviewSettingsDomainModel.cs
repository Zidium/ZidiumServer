using System;
using Zidium.Core.AccountsDb;
using Zidium.Core.Common.Helpers;

namespace Zidium.UserAccount.Models.UnitTests
{
    public class OverviewSettingsDomainModel
    {
        public Guid UnitTestId { get; set; }
        public TimeSpan Period { get; set; }
        public string Domain { get; set; }
        public int AlarmDays { get; set; }
        public int WarningDays { get; set; }

        public static OverviewSettingsDomainModel Create(UnitTest unitTest)
        {
            if (unitTest == null)
            {
                throw new ArgumentNullException("unitTest");
            }
            var rule = unitTest.DomainNamePaymentPeriodRule;
            if (rule == null)
            {
                throw new Exception("unittest domain rule is null");
            }
            return new OverviewSettingsDomainModel()
            {
                UnitTestId = unitTest.Id,
                Period = TimeSpanHelper.FromSeconds(unitTest.PeriodSeconds).Value,
                Domain = rule.Domain,
                AlarmDays = rule.AlarmDaysCount,
                WarningDays = rule.WarningDaysCount
            };
        }
    }
}