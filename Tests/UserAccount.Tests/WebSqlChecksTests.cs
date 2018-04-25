using System;
using System.Linq;
using System.Web.Mvc;
using Xunit;
using Zidium.Core.AccountsDb;
using Zidium.Core.Common;
using Zidium.TestTools;
using Zidium.UserAccount.Controllers;
using Zidium.UserAccount.Models.SqlChecksModels;

namespace Zidium.UserAccount.Tests
{
    public class WebSqlChecksTests
    {
        [Fact]
        public void EditSimpleTest()
        {
            var account = TestHelper.GetTestAccount();
            var user = TestHelper.CreateTestUser(account.Id);
            EditSimpleModel model;
            Guid unitTestId;

            // Добавим проверку
            using (var controller = new SqlChecksController(account.Id, user.Id))
            {
                var result = (ViewResultBase)controller.EditSimple();
                model = (EditSimpleModel)result.Model;
            }

            model.Name = Guid.NewGuid().ToString();
            model.ConnectionString = "Data Source=Server;Initial Catalog=Database;User Id=User;Password=Password";
            model.Query = Guid.NewGuid().ToString();
            model.Period = TimeSpan.FromMinutes(10);

            using (var controller = new SqlChecksController(account.Id, user.Id))
            {
                controller.EditSimple(model);
            }

            // Посмотрим, как создалась проверка и правило
            using (var accountContext = AccountDbContext.CreateFromAccountId(account.Id))
            {
                var unitTestRepository = accountContext.GetUnitTestRepository();
                var unitTest = unitTestRepository.QueryAll()
                    .FirstOrDefault(t => t.DisplayName == "Sql-запрос: " + model.Name);

                Assert.NotNull(unitTest);
                Assert.Equal(SystemUnitTestTypes.SqlTestType.Id, unitTest.TypeId);
                Assert.Equal((int)model.Period.TotalSeconds, unitTest.PeriodSeconds);
                Assert.Equal(ObjectColor.Gray, unitTest.NoSignalColor);
                Assert.Equal(model.ConnectionString, unitTest.SqlRule.ConnectionString);
                Assert.Equal(model.Query, unitTest.SqlRule.Query);
                Assert.Equal("Sql-запрос: " + model.Name, unitTest.DisplayName);
                Assert.Equal(model.Provider, unitTest.SqlRule.Provider);
                Assert.True(unitTest.SimpleMode);

                unitTestId = unitTest.Id;
            }

            // Отредактируем проверку
            using (var controller = new SqlChecksController(account.Id, user.Id))
            {
                var result = (ViewResultBase)controller.EditSimple(unitTestId);
                model = (EditSimpleModel)result.Model;
            }

            model.Name = Guid.NewGuid().ToString();
            model.ConnectionString = "Data Source=Server2;Initial Catalog=Database2;User Id=User2;Password=Password2";
            model.Query = Guid.NewGuid().ToString();
            model.Period = TimeSpan.FromMinutes(20);
            model.Provider = DatabaseProviderType.Oracle;

            using (var accountContext = AccountDbContext.CreateFromAccountId(account.Id))
            {
                using (var controller = new SqlChecksController(account.Id, user.Id))
                {
                    controller.EditSimple(model);

                    // Посмотрим, как изменилась проверка и правило
                    var unitTestRepository = accountContext.GetUnitTestRepository();
                    var unitTest = unitTestRepository.GetByIdOrNull(unitTestId);

                    Assert.NotNull(unitTest);
                    Assert.Equal(SystemUnitTestTypes.SqlTestType.Id, unitTest.TypeId);
                    Assert.Equal((int)model.Period.TotalSeconds, unitTest.PeriodSeconds);
                    Assert.Equal(model.ConnectionString, unitTest.SqlRule.ConnectionString);
                    Assert.Equal(model.Query, unitTest.SqlRule.Query);
                    Assert.Equal("Sql-запрос: " + model.Name, unitTest.DisplayName);
                    Assert.Equal(model.Provider, unitTest.SqlRule.Provider);

                    // Проверим, что компонент НЕ переименовался
                    Assert.NotEqual(controller.ComponentDisplayName(model), unitTest.Component.DisplayName);
                    //Assert.Equal(controller.ComponentSystemName(model), unitTest.Component.SystemName);

                    // Отключим проверку, чтобы она не мешалась другим тестам
                    unitTest.Enable = false;
                    accountContext.SaveChanges();
                }
            }
        }

    }
}
