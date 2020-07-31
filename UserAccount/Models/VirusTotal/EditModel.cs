using System;
using System.ComponentModel.DataAnnotations;
using Zidium.Common;
using Zidium.Core;
using Zidium.Core.AccountsDb;
using Zidium.Storage;
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
            get { return SystemComponentType.Others.Id; }
        }

        public override Guid UnitTestTypeId
        {
            get { return SystemUnitTestType.VirusTotalTestType.Id; }
        }

        public void LoadRule(Guid? id, IStorage storage)
        {
            if (id != null)
            {
                var rule = storage.UnitTestVirusTotalRules.GetOneOrNullByUnitTestId(id.Value);
                if (rule != null)
                {
                    Url = rule.Url;
                }
            }

            // общий ключ для всего аккаунта
            var service = new AccountSettingService(storage);
            ApiKey = service.VirusTotalApiKey;
        }

        public void SaveRule(IStorage storage)
        {
            using (var transaction = storage.BeginTransaction())
            {
                var rule = storage.UnitTestVirusTotalRules.GetOneOrNullByUnitTestId(Id.Value);
                if (rule == null)
                {
                    var ruleForAdd = new UnitTestVirusTotalRuleForAdd()
                    {
                        UnitTestId = Id.Value,
                        NextStep = VirusTotalStep.Scan
                    };
                    storage.UnitTestVirusTotalRules.Add(ruleForAdd);
                    rule = storage.UnitTestVirusTotalRules.GetOneOrNullByUnitTestId(Id.Value);
                }

                var ruleForUpdate = rule.GetForUpdate();
                ruleForUpdate.Url.Set(Url);
                storage.UnitTestVirusTotalRules.Update(ruleForUpdate);

                transaction.Commit();
            }

            // общий ключ для всего аккаунта
            var service = new AccountSettingService(storage);
            service.VirusTotalApiKey = ApiKey;
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

        public UnitTestBreadCrumbsModel UnitTestBreadCrumbs { get; set; }

    }
}