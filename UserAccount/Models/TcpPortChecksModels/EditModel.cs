using System;
using System.ComponentModel.DataAnnotations;
using Zidium.Core;
using Zidium.Core.AccountsDb;
using Zidium.UserAccount.Models.CheckModels;

namespace Zidium.UserAccount.Models.TcpPortChecksModels
{
    public class EditModel : UnitTestCommonSettingsModel
    {
        [Display(Name = "Хост")]
        public string Host { get; set; }

        [Display(Name = "Порт")]
        public int? Port { get; set; }

        [Display(Name = "Порт должен быть открыт")]
        public bool Opened { get; set; }

        public EditModel()
        {
            Opened = true;
        }

        public override string ComponentLabelText
        {
            get { return "Выберите компонент (хост)"; }
        }

        public override string DefaultCheckName
        {
            get { return "Проверка tcp порта"; }
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
            get { return SystemUnitTestTypes.TcpPortTestType.Id; }
        }

        public void LoadRule()
        {
            if (UnitTest != null)
            {
                var rule = UnitTest.TcpPortRule;
                if (rule != null)
                {
                    Host = rule.Host;
                    Port = rule.Port;
                    Opened = rule.Opened;
                }
            }
        }

        public void SaveRule()
        {
            if (UnitTest.TcpPortRule == null)
            {
                var newRule = new UnitTestTcpPortRule();
                newRule.UnitTestId = UnitTest.Id;
                newRule.UnitTest = UnitTest;
                UnitTest.TcpPortRule = newRule;
            }
            var rule = UnitTest.TcpPortRule;
            rule.Host = Host;
            rule.Port = Port.Value;
            rule.Opened = Opened;
        }

        protected override void ValidateRule()
        {
            if (string.IsNullOrEmpty(Host))
            {
                throw new UserFriendlyException("Заполните хост");
            }
            if (Port==null)
            {
                throw new UserFriendlyException("Заполните порт");
            }
            if (Port < 0 || Port > 65535)
            {
                throw new UserFriendlyException("Порт имеет значения от 0 до 65535");
            }
        }
    }
}