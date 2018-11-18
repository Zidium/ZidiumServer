using System;
using Zidium.Core.AccountsDb;

namespace Zidium.UserAccount.Models.UnitTests
{
    public class ShowSettingsDomainModel
    {
        public Guid UnitTestId { get; set; }
        public string Domain { get; set; }
        public int AlarmDays { get; set; }
        public int WarningDays { get; set; }

        public static ShowSettingsDomainModel Create(UnitTest unitTest)
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
            return new ShowSettingsDomainModel()
            {
                UnitTestId = unitTest.Id,
                Domain = rule.Domain,
                AlarmDays = rule.AlarmDaysCount,
                WarningDays = rule.WarningDaysCount
            };
        }
    }
}