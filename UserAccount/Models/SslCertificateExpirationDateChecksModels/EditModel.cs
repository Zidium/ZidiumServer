using System;
using System.ComponentModel.DataAnnotations;
using Zidium.Core;
using Zidium.Core.AccountsDb;
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
            get { return SystemComponentTypes.WebSite.Id; }
        }

        public override Guid UnitTestTypeId
        {
            get { return SystemUnitTestTypes.SslTestType.Id; }
        }

        public void LoadRule()
        {
            AlarmDaysCount = 14;
            WarningDaysCount = 30;

            if (UnitTest != null)
            {
                var rule = UnitTest.SslCertificateExpirationDateRule;
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

        public void SaveRule()
        {
            if (UnitTest.SslCertificateExpirationDateRule == null)
            {
                var newRule = new UnitTestSslCertificateExpirationDateRule();
                newRule.UnitTestId = UnitTest.Id;
                newRule.UnitTest = UnitTest;
                UnitTest.SslCertificateExpirationDateRule = newRule;
            }
            var rule = UnitTest.SslCertificateExpirationDateRule;
            rule.Url = Url;
            rule.AlarmDaysCount = AlarmDaysCount;
            rule.WarningDaysCount = WarningDaysCount;
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
    }
}