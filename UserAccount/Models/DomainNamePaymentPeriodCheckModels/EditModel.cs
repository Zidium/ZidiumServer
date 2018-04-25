using System;
using System.ComponentModel.DataAnnotations;
using Zidium.Core;
using Zidium.Core.AccountsDb;
using Zidium.UserAccount.Models.CheckModels;

namespace Zidium.UserAccount.Models.DomainNamePaymentPeriodCheckModels
{
    public class EditModel : UnitTestCommonSettingsModel
    {
        [Display(Name = "Доменное имя")]
        public string Domain { get; set; }

        [Display(Name = "Количество дней красной проверки")]
        public int AlarmDaysCount { get; set; }

        [Display(Name = "Количество дней жёлтой проверки")]
        public int WarningDaysCount { get; set; }

        public override string ComponentLabelText
        {
            get { return "Выберите компонент (домен)"; }
        }

        public override string DefaultCheckName
        {
            get { return "Проверка срока оплаты домена"; }
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
            get { return SystemUnitTestTypes.DomainNameTestType.Id; }
        }

        public void LoadRule()
        {
            AlarmDaysCount = 14;
            WarningDaysCount = 30;

            if (UnitTest != null)
            {
                var rule = UnitTest.DomainNamePaymentPeriodRule;
                if (rule != null)
                {
                    Domain = rule.Domain;
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
            if (UnitTest.DomainNamePaymentPeriodRule == null)
            {
                var newRule = new UnitTestDomainNamePaymentPeriodRule();
                newRule.UnitTestId = UnitTest.Id;
                newRule.UnitTest = UnitTest;
                UnitTest.DomainNamePaymentPeriodRule = newRule;
            }
            var rule = UnitTest.DomainNamePaymentPeriodRule;
            rule.Domain = Domain;
            rule.AlarmDaysCount = AlarmDaysCount;
            rule.WarningDaysCount = WarningDaysCount;
        }

        protected override void ValidateRule()
        {
            if (string.IsNullOrEmpty(Domain))
            {
                throw new UserFriendlyException("Заполните доменное имя");
            }
            if (AlarmDaysCount < 1)
            {
                throw new UserFriendlyException("Количество дней для красной проверки должно быть больше нуля");
            }
            if (WarningDaysCount < 1)
            {
                throw new UserFriendlyException("Количество дней для желтой проверки должно быть больше нуля");
            }
            if (AlarmDaysCount > WarningDaysCount)
            {
                throw new UserFriendlyException("Количество дней для красной проверки не может быть больше количества дней желтой проверки");
            }
        }
    }
}