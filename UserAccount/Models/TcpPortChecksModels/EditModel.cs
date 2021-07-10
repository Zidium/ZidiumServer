using System;
using System.ComponentModel.DataAnnotations;
using Zidium.Common;
using Zidium.Core;
using Zidium.Core.AccountsDb;
using Zidium.Storage;
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
            get { return SystemComponentType.Others.Id; }
        }

        public override Guid UnitTestTypeId
        {
            get { return SystemUnitTestType.TcpPortTestType.Id; }
        }

        public void LoadRule(Guid? id, IStorage storage)
        {
            if (id != null)
            {
                var rule = storage.UnitTestTcpPortRules.GetOneOrNullByUnitTestId(id.Value);
                if (rule != null)
                {
                    Host = rule.Host;
                    Port = rule.Port;
                    Opened = rule.Opened;
                }
            }
        }

        public void SaveRule(IStorage storage)
        {
            using (var transaction = storage.BeginTransaction())
            {
                var rule = storage.UnitTestTcpPortRules.GetOneOrNullByUnitTestId(Id.Value);
                if (rule == null)
                {
                    var ruleForAdd = new UnitTestTcpPortRuleForAdd()
                    {
                        UnitTestId = Id.Value
                    };
                    storage.UnitTestTcpPortRules.Add(ruleForAdd);
                    rule = storage.UnitTestTcpPortRules.GetOneOrNullByUnitTestId(Id.Value);
                }

                var ruleForUpdate = rule.GetForUpdate();
                ruleForUpdate.Host.Set(Host);
                ruleForUpdate.Port.Set(Port.Value);
                ruleForUpdate.Opened.Set(Opened);
                storage.UnitTestTcpPortRules.Update(ruleForUpdate);

                transaction.Commit();
            }
        }

        protected override void ValidateRule()
        {
            if (string.IsNullOrEmpty(Host))
            {
                throw new UserFriendlyException("Заполните хост");
            }
            if (Port == null)
            {
                throw new UserFriendlyException("Заполните порт");
            }
            if (Port < 0 || Port > 65535)
            {
                throw new UserFriendlyException("Порт имеет значения от 0 до 65535");
            }
        }

        public UnitTestBreadCrumbsModel UnitTestBreadCrumbs { get; set; }

    }
}