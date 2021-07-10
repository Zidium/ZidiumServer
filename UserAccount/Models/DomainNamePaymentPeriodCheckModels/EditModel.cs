using System;
using System.ComponentModel.DataAnnotations;
using Zidium.Common;
using Zidium.Core;
using Zidium.Core.AccountsDb;
using Zidium.Storage;
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

        public UnitTestBreadCrumbsModel UnitTestBreadCrumbs { get; set; }

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
            get { return SystemComponentType.Others.Id; }
        }

        public override Guid UnitTestTypeId
        {
            get { return SystemUnitTestType.DomainNameTestType.Id; }
        }

        public void LoadRule(Guid? id, IStorage storage)
        {
            AlarmDaysCount = 14;
            WarningDaysCount = 30;

            if (id != null)
            {
                var rule = storage.DomainNamePaymentPeriodRules.GetOneOrNullByUnitTestId(id.Value);
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

        public void SaveRule(Guid id, IStorage storage)
        {
            using (var transaction = storage.BeginTransaction())
            {
                var rule = storage.DomainNamePaymentPeriodRules.GetOneOrNullByUnitTestId(id);
                if (rule == null)
                {
                    var newRule = new UnitTestDomainNamePaymentPeriodRuleForAdd();
                    newRule.UnitTestId = id;
                    storage.DomainNamePaymentPeriodRules.Add(newRule);
                }

                var ruleForUpdate = new UnitTestDomainNamePaymentPeriodRuleForUpdate(id);
                ruleForUpdate.Domain.Set(Domain);
                ruleForUpdate.AlarmDaysCount.Set(AlarmDaysCount);
                ruleForUpdate.WarningDaysCount.Set(WarningDaysCount);
                storage.DomainNamePaymentPeriodRules.Update(ruleForUpdate);

                transaction.Commit();
            }
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