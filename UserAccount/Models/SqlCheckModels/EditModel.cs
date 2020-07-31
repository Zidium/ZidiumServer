using System;
using System.ComponentModel.DataAnnotations;
using Zidium.Common;
using Zidium.Core;
using Zidium.Core.AccountsDb;
using Zidium.Storage;
using Zidium.UserAccount.Models.CheckModels;

namespace Zidium.UserAccount.Models.SqlChecksModels
{
    public class EditModel : UnitTestCommonSettingsModel
    {
        [Required(ErrorMessage = "Пожалуйста, выберите провайдера")]
        [Display(Name = "Провайдер")]
        public SqlRuleDatabaseProviderType Provider { get; set; }

        [Display(Name = "Строка соединения")]
        public string ConnectionString { get; set; }

        [Display(Name = "Таймаут выполнения запроса, мс")]
        public int CommandTimeoutMs { get; set; }

        [Display(Name = "Таймаут открытия соединения, мс")]
        public int OpenConnectionTimeoutMs { get; set; }

        [Display(Name = "Sql запрос")]
        public string Query { get; set; }

        public override string ComponentLabelText
        {
            get { return "Выберите компонент (базу данных)"; }
        }

        public override string DefaultCheckName
        {
            get { return "Проверка базы данных"; }
        }

        public override UserFolder NewComponentFolder
        {
            get { return UserFolder.DataBases; }
        }

        public override Guid NewComponentTypeId
        {
            get { return SystemComponentType.Database.Id; }
        }

        public override Guid UnitTestTypeId
        {
            get { return SystemUnitTestType.SqlTestType.Id; }
        }

        public void LoadRule(Guid? id, IStorage storage)
        {
            OpenConnectionTimeoutMs = 3000;
            CommandTimeoutMs = 30000;
            Provider = SqlRuleDatabaseProviderType.MsSql;
            if (id != null)
            {
                var rule = storage.UnitTestSqlRules.GetOneOrNullByUnitTestId(id.Value);
                if (rule != null)
                {
                    CommandTimeoutMs = rule.CommandTimeoutMs;
                    ConnectionString = rule.ConnectionString;
                    OpenConnectionTimeoutMs = rule.OpenConnectionTimeoutMs;
                    Query = rule.Query;
                    Provider = rule.Provider;
                }
            }
        }

        public void SaveRule(IStorage storage)
        {
            using (var transaction = storage.BeginTransaction())
            {
                var rule = storage.UnitTestSqlRules.GetOneOrNullByUnitTestId(Id.Value);
                if (rule == null)
                {
                    var ruleForAdd = new UnitTestSqlRuleForAdd()
                    {
                        UnitTestId = Id.Value,
                        Provider = SqlRuleDatabaseProviderType.MsSql,
                        OpenConnectionTimeoutMs = 3000,
                        CommandTimeoutMs = 30000
                    };
                    storage.UnitTestSqlRules.Add(ruleForAdd);
                    rule = storage.UnitTestSqlRules.GetOneOrNullByUnitTestId(Id.Value);
                }

                var ruleForUpdate = rule.GetForUpdate();
                ruleForUpdate.ConnectionString.Set(ConnectionString);
                ruleForUpdate.Query.Set(Query);
                ruleForUpdate.Provider.Set(Provider);
                ruleForUpdate.CommandTimeoutMs.Set(CommandTimeoutMs);
                ruleForUpdate.OpenConnectionTimeoutMs.Set(OpenConnectionTimeoutMs);
                storage.UnitTestSqlRules.Update(ruleForUpdate);

                transaction.Commit();
            }
        }

        protected override void ValidateRule()
        {
            if (string.IsNullOrEmpty(Query))
            {
                throw new UserFriendlyException("Заполните запрос");
            }
            if (string.IsNullOrEmpty(ConnectionString))
            {
                throw new UserFriendlyException("Заполните строку соединения");
            }
            if (OpenConnectionTimeoutMs < 1000)
            {
                throw new UserFriendlyException("Таймаут открытия соединения должен быть >= 1000");
            }
            if (CommandTimeoutMs < 1000)
            {
                throw new UserFriendlyException("Таймаут выполнения запроса должен быть >= 1000");
            }
        }

        public UnitTestBreadCrumbsModel UnitTestBreadCrumbs;
    }
}