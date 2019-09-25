using System;
using System.Linq;
using System.Web.Mvc;
using Xunit;
using Zidium.Core.AccountsDb;
using Zidium.Core.Common;
using Zidium.TestTools;
using Zidium.UserAccount.Controllers;
using Zidium.UserAccount.Models.TcpPortChecksModels;

namespace Zidium.UserAccount.Tests
{
    public class WebTcpPortChecksTests
    {
        [Fact]
        public void EditSimpleTest()
        {
            var account = TestHelper.GetTestAccount();
            var user = TestHelper.CreateTestUser(account.Id);
            EditSimpleModel model;
            Guid unitTestId;

            // Добавим проверку
            using (var controller = new TcpPortChecksController(account.Id, user.Id))
            {
                var result = (ViewResultBase)controller.EditSimple();
                model = (EditSimpleModel)result.Model;
            }

            model.Host = Guid.NewGuid().ToString();
            model.Period = TimeSpan.FromMinutes(10);
            model.Port = 80;
            model.Opened = true;

            using (var controller = new TcpPortChecksController(account.Id, user.Id))
            {
                controller.EditSimple(model);
            }

            // Посмотрим, как создалась проверка и правило
            using (var accountContext = AccountDbContext.CreateFromAccountId(account.Id))
            {
                var unitTestRepository = accountContext.GetUnitTestRepository();
                var unitTest = unitTestRepository.QueryAll()
                    .FirstOrDefault(t => t.TcpPortRule != null && t.TcpPortRule.Host == model.Host);

                Assert.NotNull(unitTest);
                Assert.Equal(SystemUnitTestTypes.TcpPortTestType.Id, unitTest.TypeId);
                Assert.Equal((int)TimeSpan.FromMinutes(10).TotalSeconds, unitTest.PeriodSeconds);
                Assert.Equal(ObjectColor.Gray, unitTest.NoSignalColor);
                Assert.True(unitTest.SimpleMode);
                Assert.Equal(2, unitTest.AttempMax);
                Assert.Equal(model.Host, unitTest.TcpPortRule.Host);
                Assert.Equal(model.Port, unitTest.TcpPortRule.Port);
                Assert.Equal(model.Opened, unitTest.TcpPortRule.Opened);

                unitTestId = unitTest.Id;
            }

            // Отредактируем проверку
            using (var controller = new TcpPortChecksController(account.Id, user.Id))
            {
                var result = (ViewResultBase)controller.EditSimple(unitTestId);
                model = (EditSimpleModel)result.Model;
            }

            model.Host = Guid.NewGuid().ToString();
            model.Period = TimeSpan.FromMinutes(20);
            model.Port = 81;
            model.Opened = false;

            using (var accountContext = AccountDbContext.CreateFromAccountId(account.Id))
            {
                using (var controller = new TcpPortChecksController(account.Id, user.Id))
                {
                    controller.EditSimple(model);

                    // Посмотрим, как изменилась проверка и правило
                    var unitTestRepository = accountContext.GetUnitTestRepository();
                    var unitTest = unitTestRepository.GetByIdOrNull(unitTestId);

                    Assert.NotNull(unitTest);
                    Assert.Equal(SystemUnitTestTypes.TcpPortTestType.Id, unitTest.TypeId);
                    Assert.Equal((int)TimeSpan.FromMinutes(20).TotalSeconds, unitTest.PeriodSeconds);

                    Assert.Equal(model.Host, unitTest.TcpPortRule.Host);
                    Assert.Equal(model.Port, unitTest.TcpPortRule.Port);
                    Assert.Equal(model.Opened, unitTest.TcpPortRule.Opened);

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
