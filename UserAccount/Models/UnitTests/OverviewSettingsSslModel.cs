using System;
using Zidium.Core.AccountsDb;
using Zidium.Core.Common.Helpers;

namespace Zidium.UserAccount.Models.UnitTests
{
    public class OverviewSettingsSslModel
    {
        public Guid UnitTestId { get; set; }
        public TimeSpan Period { get; set; }
        public string Url { get; set; }
        public int AlarmDays { get; set; }
        public int WarningDays { get; set; }

        public static OverviewSettingsSslModel Create(UnitTest unitTest)
        {
            if (unitTest == null)
            {
                throw new ArgumentNullException("unitTest");
            }
            var rule = unitTest.SslCertificateExpirationDateRule;
            if (rule == null)
            {
                throw new Exception("unittest domain rule is null");
            }
            return new OverviewSettingsSslModel()
            {
                UnitTestId = unitTest.Id,
                Period = TimeSpanHelper.FromSeconds(unitTest.PeriodSeconds).Value,
                Url = rule.Url,
                AlarmDays = rule.AlarmDaysCount,
                WarningDays = rule.WarningDaysCount
            };
        }
    }
}