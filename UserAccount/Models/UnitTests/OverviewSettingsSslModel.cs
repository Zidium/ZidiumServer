using System;
using Zidium.Core.Common.Helpers;
using Zidium.Storage;

namespace Zidium.UserAccount.Models.UnitTests
{
    public class OverviewSettingsSslModel
    {
        public Guid UnitTestId { get; set; }
        public TimeSpan Period { get; set; }
        public string Url { get; set; }
        public int AlarmDays { get; set; }
        public int WarningDays { get; set; }

        public static OverviewSettingsSslModel Create(UnitTestForRead unitTest, IStorage storage)
        {
            if (unitTest == null)
            {
                throw new ArgumentNullException("unitTest");
            }

            var rule = storage.UnitTestSslCertificateExpirationDateRules.GetOneByUnitTestId(unitTest.Id);
            
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