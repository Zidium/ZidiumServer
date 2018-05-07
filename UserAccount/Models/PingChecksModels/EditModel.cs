using System;
using System.ComponentModel.DataAnnotations;
using Zidium.Core;
using Zidium.Core.AccountsDb;
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
            get { return SystemComponentTypes.Others.Id; }
        }

        public override Guid UnitTestTypeId
        {
            get { return SystemUnitTestTypes.PingTestType.Id; }
        }

        public void LoadRule()
        {
            TimeoutMs = 1000;

            if (UnitTest != null)
            {
                var rule = UnitTest.PingRule;
                if (rule != null)
                {
                    Host = rule.Host;
                    TimeoutMs = rule.TimeoutMs;
                }
            }
        }

        public void SaveRule()
        {
            if (UnitTest.PingRule == null)
            {
                var newRule = new UnitTestPingRule();
                newRule.UnitTestId = UnitTest.Id;
                newRule.UnitTest = UnitTest;
                UnitTest.PingRule = newRule;
            }
            var rule = UnitTest.PingRule;
            rule.Host = Host;
            rule.TimeoutMs = TimeoutMs;
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
    }
}