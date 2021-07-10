using System;
using Zidium.Storage;

namespace Zidium.UserAccount.Models.UnitTests
{
    public class ShowSettingsSslModel
    {
        public Guid UnitTestId { get; set; }
        public string Url { get; set; }
        public int AlarmDays { get; set; }
        public int WarningDays { get; set; }

        public static ShowSettingsSslModel Create(UnitTestForRead unitTest, IStorage storage)
        {
            if (unitTest == null)
            {
                throw new ArgumentNullException("unitTest");
            }

            var rule = storage.UnitTestSslCertificateExpirationDateRules.GetOneByUnitTestId(unitTest.Id);
            
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