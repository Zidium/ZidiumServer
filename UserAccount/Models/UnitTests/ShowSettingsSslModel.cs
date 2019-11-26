using System;
using Zidium.Core.AccountsDb;

namespace Zidium.UserAccount.Models.UnitTests
{
    public class ShowSettingsSslModel
    {
        public Guid UnitTestId { get; set; }
        public string Url { get; set; }
        public int AlarmDays { get; set; }
        public int WarningDays { get; set; }

        public static ShowSettingsSslModel Create(UnitTest unitTest)
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
            return new ShowSettingsSslModel()
            {
                UnitTestId = unitTest.Id,
                Url = rule.Url,
                AlarmDays = rule.AlarmDaysCount,
                WarningDays = rule.WarningDaysCount
            };
        }
    }
}