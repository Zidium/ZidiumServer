using System;
using System.ComponentModel.DataAnnotations;
using Zidium.Core;
using Zidium.Core.AccountsDb;
using Zidium.Core.Common;
using Zidium.UserAccount.Models.CheckModels;

namespace Zidium.UserAccount.Models.SqlChecksModels
{
    public class EditModel : UnitTestCommonSettingsModel
    {
        [Required(ErrorMessage = "Пожалуйста, выберите провайдера")]
        [Display(Name = "Провайдер")]
        public DatabaseProviderType Provider { get; set; }

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
            get { return SystemComponentTypes.DataBase.Id; }
        }

        public override Guid UnitTestTypeId
        {
            get { return SystemUnitTestTypes.SqlTestType.Id; }
        }

        public void LoadRule()
        {
            OpenConnectionTimeoutMs = 3000;
            CommandTimeoutMs = 30000;
            Provider = DatabaseProviderType.MsSql;
            if (UnitTest != null)
            {
                var rule = UnitTest.SqlRule;
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

        public void SaveRule()
        {
            if (UnitTest.SqlRule == null)
            {
                var newRule = new UnitTestSqlRule();
                newRule.UnitTestId = UnitTest.Id;
                newRule.UnitTest = UnitTest;
                UnitTest.SqlRule = newRule;
            }
            var rule = UnitTest.SqlRule;
            rule.ConnectionString = ConnectionString;
            rule.OpenConnectionTimeoutMs = OpenConnectionTimeoutMs;
            rule.Provider = Provider;
            rule.Query = Query;
            rule.CommandTimeoutMs = CommandTimeoutMs;
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
    }
}