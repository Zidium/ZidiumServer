using System;
using Zidium.Core.AccountsDb;
using Zidium.Storage;

namespace Zidium.UserAccount.Models.UnitTests
{
    public class ShowSettingsVirusTotalModel
    {
        public Guid UnitTestId { get; set; }
        public string Url { get; set; }
        public string ApiKey { get; set; }

        public static ShowSettingsVirusTotalModel Create(UnitTestForRead unitTest, IStorage storage)
        {
            if (unitTest == null)
            {
                throw new ArgumentNullException("unitTest");
            }

            var rule = storage.UnitTestVirusTotalRules.GetOneByUnitTestId(unitTest.Id);

            var settingService = new AccountSettingService(storage);
            var apiKey = settingService.VirusTotalApiKey;

            return new ShowSettingsVirusTotalModel()
            {
                UnitTestId = unitTest.Id,
                Url = rule.Url,
                ApiKey = apiKey
            };
        }
    }
}