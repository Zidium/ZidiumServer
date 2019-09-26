using System;
using System.Linq;
using System.Web.Mvc;
using Xunit;
using Zidium.Core.AccountsDb;
using Zidium.Core.Common;
using Zidium.TestTools;
using Zidium.UserAccount.Controllers;
using Zidium.UserAccount.Models.PingChecksModels;

namespace Zidium.UserAccount.Tests
{
    public class WebPingChecksTests
    {
        [Fact]
        public void EditSimpleTest()
        {
            var account = TestHelper.GetTestAccount();
            var user = TestHelper.CreateTestUser(account.Id);
            EditSimpleModel model;
            Guid unitTestId;

            // Добавим проверку
            using (var controller = new PingChecksController(account.Id, user.Id))
            {
                var result = (ViewResultBase)controller.EditSimple();
                model = (EditSimpleModel)result.Model;
            }

            model.Host = Guid.NewGuid().ToString();
            model.Period = TimeSpan.FromMinutes(10);

            using (var controller = new PingChecksController(account.Id, user.Id))
            {
                controller.EditSimple(model);
            }

            // Посмотрим, как создалась проверка и правило
            using (var accountContext = AccountDbContext.CreateFromAccountId(account.Id))
            {
                var unitTestRepository = accountContext.GetUnitTestRepository();
                var unitTest = unitTestRepository.QueryAll()
                    .FirstOrDefault(t => t.PingRule != null && t.PingRule.Host == model.Host);

                Assert.NotNull(unitTest);
                Assert.Equal(SystemUnitTestTypes.PingTestType.Id, unitTest.TypeId);
                Assert.Equal((int)TimeSpan.FromMinutes(10).TotalSeconds, unitTest.PeriodSeconds);
                Assert.Equal(ObjectColor.Gray, unitTest.NoSignalColor);
                Assert.True(unitTest.SimpleMode);
                Assert.Equal(2, unitTest.AttempMax);

                unitTestId = unitTest.Id;
            }

            // Отредактируем проверку
            using (var controller = new PingChecksController(account.Id, user.Id))
            {
                var result = (ViewResultBase)controller.EditSimple(unitTestId);
                model = (EditSimpleModel)result.Model;
            }

            model.Host = Guid.NewGuid().ToString();
            model.Period = TimeSpan.FromMinutes(20);

            using (var accountContext = AccountDbContext.CreateFromAccountId(account.Id))
            {
                using (var controller = new PingChecksController(account.Id, user.Id))
                {
                    controller.EditSimple(model);

                    // Посмотрим, как изменилась проверка и правило
                    var unitTestRepository = accountContext.GetUnitTestRepository();
                    var unitTest = unitTestRepository.GetByIdOrNull(unitTestId);

                    Assert.NotNull(unitTest);
                    Assert.Equal(SystemUnitTestTypes.PingTestType.Id, unitTest.TypeId);
                    Assert.Equal((int)TimeSpan.FromMinutes(20).TotalSeconds, unitTest.PeriodSeconds);
                    Assert.Equal(model.Host, unitTest.PingRule.Host);

                    // Проверим, что компонент НЕ переименовался
                    Assert.NotEqual(controller.GetComponentDisplayName(model), unitTest.Component.DisplayName);
                    //Assert.Equal(controller.ComponentSystemName(model), unitTest.Component.SystemName);

                    // Отключим проверку, чтобы она не мешалась другим тестам
                    unitTest.Enable = false;
                    accountContext.SaveChanges();
                }
            }
        }
    }
}
