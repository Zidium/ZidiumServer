using System;
using System.ComponentModel.DataAnnotations;
using Zidium.Common;
using Zidium.Core;
using Zidium.Core.AccountsDb;
using Zidium.Storage;
using Zidium.UserAccount.Models.CheckModels;

namespace Zidium.UserAccount.Models.SslCertificateExpirationDateChecksModels
{
    public class EditModel : UnitTestCommonSettingsModel
    {
        [Display(Name = "Url сайта")]
        public string Url { get; set; }

        [Display(Name = "Количество дней красной проверки")]
        public int AlarmDaysCount { get; set; }

        [Display(Name = "Количество дней жёлтой проверки")]
        public int WarningDaysCount { get; set; }

        public override string ComponentLabelText
        {
            get { return "Выберите компонент (сайт)"; }
        }

        public override string DefaultCheckName
        {
            get { return "Проверка срока годности ssl-сертификата сайта"; }
        }

        public override UserFolder NewComponentFolder
        {
            get { return UserFolder.WebSites; }
        }

        public override Guid NewComponentTypeId
        {
            get { return SystemComponentType.WebSite.Id; }
        }

        public override Guid UnitTestTypeId
        {
            get { return SystemUnitTestType.SslTestType.Id; }
        }

        public void LoadRule(Guid? id, IStorage storage)
        {
            AlarmDaysCount = 14;
            WarningDaysCount = 30;

            if (id != null)
            {
                var rule = storage.UnitTestSslCertificateExpirationDateRules.GetOneOrNullByUnitTestId(id.Value);
                if (rule != null)
                {
                    Url = rule.Url;
                    AlarmDaysCount = rule.AlarmDaysCount;
                    WarningDaysCount = rule.WarningDaysCount;
                }
            }
            else
            {
                Period = TimeSpan.FromDays(1);
                ActualTime = TimeSpan.FromDays(2);
            }
        }

        public void SaveRule(IStorage storage)
        {
            using (var transaction = storage.BeginTransaction())
            {
                var rule = storage.UnitTestSslCertificateExpirationDateRules.GetOneOrNullByUnitTestId(Id.Value);
                if (rule == null)
                {
                    var ruleForAdd = new UnitTestSslCertificateExpirationDateRuleForAdd()
                    {
                        UnitTestId = Id.Value
                    };
                    storage.UnitTestSslCertificateExpirationDateRules.Add(ruleForAdd);
                    rule = storage.UnitTestSslCertificateExpirationDateRules.GetOneOrNullByUnitTestId(Id.Value);
                }

                var ruleForUpdate = rule.GetForUpdate();
                ruleForUpdate.Url.Set(Url);
                ruleForUpdate.WarningDaysCount.Set(WarningDaysCount);
                ruleForUpdate.AlarmDaysCount.Set(AlarmDaysCount);
                storage.UnitTestSslCertificateExpirationDateRules.Update(ruleForUpdate);

                transaction.Commit();
            }
        }

        protected override void ValidateRule()
        {
            if (string.IsNullOrEmpty(Url))
            {
                throw new UserFriendlyException("Заполните Url сайта");
            }
            try
            {
                var uri = new Uri(Url);
                if (uri.Scheme != "https")
                {
                    throw new UserFriendlyException("Url должен начинаться на https://");
                }
            }
            catch (UriFormatException)
            {
                throw new UserFriendlyException("Неверный формат Url сайта");
            }
            if (AlarmDaysCount < 1)
            {
                throw new UserFriendlyException("Количество дней для красной проверки должно быть больше нуля");
            }
            if (WarningDaysCount < 1)
            {
                throw new UserFriendlyException("Количество дней для жёлтой проверки должно быть больше нуля");
            }
            if (AlarmDaysCount > WarningDaysCount)
            {
                throw new UserFriendlyException("Количество дней для красной проверки не может быть больше количества дней жёлтой проверки");
            }
        }

        public UnitTestBreadCrumbsModel UnitTestBreadCrumbs { get; set; }

    }
}