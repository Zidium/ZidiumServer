using System;
using System.ComponentModel.DataAnnotations;
using Zidium.Core;
using Zidium.Core.AccountsDb;
using Zidium.UserAccount.Models.CheckModels;

namespace Zidium.UserAccount.Models.VirusTotal
{
    public class EditModel : UnitTestCommonSettingsModel
    {
        [Display(Name = "Url")]
        public string Url { get; set; }

        [Display(Name = "ApiKey")]
        public string ApiKey { get; set; }

        public string CommonWebsiteUrl { get; set; }

        public override string ComponentLabelText
        {
            get { return "Выберите компонент"; }
        }

        public override string DefaultCheckName
        {
            get { return "Проверка VirusTotal"; }
        }

        public override UserFolder NewComponentFolder
        {
            get { return UserFolder.OtherComponents; }
        }

        public override Guid NewComponentTypeId
        {
            get { return SystemComponentTypes.Others.Id; }
        }

        public override Guid UnitTestTypeId
        {
            get { return SystemUnitTestTypes.VirusTotalTestType.Id; }
        }

        public void LoadRule()
        {
            if (UnitTest != null)
            {
                var rule = UnitTest.VirusTotalRule;
                if (rule != null)
                {
                    Url = rule.Url;
                }
            }

            // общий ключ для всего аккаунта
            ApiKey = AccountDbContext.GetAccountSettingService().VirusTotalApiKey;
        }

        public void SaveRule()
        {
            if (UnitTest.VirusTotalRule == null)
            {
                var newRule = new UnitTestVirusTotalRule();
                newRule.UnitTestId = UnitTest.Id;
                newRule.UnitTest = UnitTest;
                newRule.NextStep = VirusTotalStep.Scan;
                UnitTest.VirusTotalRule = newRule;
            }
            var rule = UnitTest.VirusTotalRule;
            rule.Url = Url;

            // общий ключ для всего аккаунта
            AccountDbContext.GetAccountSettingService().VirusTotalApiKey = ApiKey;
        }

        protected override void ValidateRule()
        {
            if (string.IsNullOrEmpty(Url))
            {
                throw new UserFriendlyException("Заполните Url");
            }
            if (string.IsNullOrEmpty(ApiKey))
            {
                throw new UserFriendlyException("Заполните ApiKey");
            }
        }
    }
}