using System;
using Zidium.Core.AccountsDb;

namespace Zidium.UserAccount.Models.UnitTests
{
    public class ShowSettingsVirusTotalModel
    {
        public Guid UnitTestId { get; set; }
        public string Url { get; set; }
        public string ApiKey { get; set; }

        public static ShowSettingsVirusTotalModel Create(UnitTest unitTest)
        {
            if (unitTest == null)
            {
                throw new ArgumentNullException("unitTest");
            }
            var rule = unitTest.VirusTotalRule;
            if (rule == null)
            {
                throw new Exception("unittest rule is null");
            }

            var accountDbContext = FullRequestContext.Current.AccountDbContext;
            var settingService = accountDbContext.GetAccountSettingService();
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