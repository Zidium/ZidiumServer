using System;
using System.ComponentModel.DataAnnotations;
using Zidium.Common;
using Zidium.Core;
using Zidium.Core.AccountsDb;
using Zidium.Storage;
using Zidium.UserAccount.Models.CheckModels;

namespace Zidium.UserAccount.Models.PingChecksModels
{
    public class EditModel : UnitTestCommonSettingsModel
    {
        [Display(Name = "Хост")]
        public string Host { get; set; }

        [Display(Name = "Таймаут, мс")]
        public int TimeoutMs { get; set; }

        public override string ComponentLabelText
        {
            get { return "Выберите компонент (хост)"; }
        }

        public override string DefaultCheckName
        {
            get { return "Проверка ping"; }
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
            get { return SystemUnitTestType.PingTestType.Id; }
        }

        public void LoadRule(Guid? id, IStorage storage)
        {
            TimeoutMs = 1000;

            if (id != null)
            {
                var rule = storage.UnitTestPingRules.GetOneOrNullByUnitTestId(id.Value);
                if (rule != null)
                {
                    Host = rule.Host;
                    TimeoutMs = rule.TimeoutMs;
                }
            }
        }

        public void SaveRule(IStorage storage)
        {
            using (var transaction = storage.BeginTransaction())
            {
                var rule = storage.UnitTestPingRules.GetOneOrNullByUnitTestId(Id.Value);
                if (rule == null)
                {
                    var ruleForAdd = new UnitTestPingRuleForAdd()
                    {
                        UnitTestId = Id.Value,
                        TimeoutMs = 5000
                    };
                    storage.UnitTestPingRules.Add(ruleForAdd);
                    rule = storage.UnitTestPingRules.GetOneOrNullByUnitTestId(Id.Value);
                }

                var ruleForUpdate = rule.GetForUpdate();
                ruleForUpdate.Host.Set(Host);
                ruleForUpdate.TimeoutMs.Set(TimeoutMs);
                storage.UnitTestPingRules.Update(ruleForUpdate);

                transaction.Commit();
            }
        }

        protected override void ValidateRule()
        {
            if (string.IsNullOrEmpty(Host))
            {
                throw new UserFriendlyException("Заполните хост");
            }
            if (TimeoutMs <= 0)
            {
                throw new UserFriendlyException("Таймаут должен быть больше нуля");
            }
        }

        public UnitTestBreadCrumbsModel UnitTestBreadCrumbs;

    }
}