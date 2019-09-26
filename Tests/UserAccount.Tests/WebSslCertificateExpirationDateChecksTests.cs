using System;
using System.Linq;
using System.Web.Mvc;
using Xunit;
using Zidium.Core.AccountsDb;
using Zidium.Core.Common;
using Zidium.TestTools;
using Zidium.UserAccount.Controllers;
using Zidium.UserAccount.Models.SslCertificateExpirationDateChecksModels;

namespace Zidium.UserAccount.Tests
{
    public class WebSslCertificateExpirationDateChecksTests
    {
        [Fact]
        public void EditSimpleTest()
        {
            var account = TestHelper.GetTestAccount();
            var user = TestHelper.CreateTestUser(account.Id);
            EditSimpleModel model;
            Guid unitTestId;

            // Добавим проверку
            using (var controller = new SslCertificateExpirationDateChecksController(account.Id, user.Id))
            {
                var result = (ViewResultBase)controller.EditSimple();
                model = (EditSimpleModel)result.Model;
            }

            model.Url = Guid.NewGuid().ToString();

            using (var controller = new SslCertificateExpirationDateChecksController(account.Id, user.Id))
            {
                controller.EditSimple(model);
            }

            // Посмотрим, как создалась проверка и правило
            using (var accountContext = AccountDbContext.CreateFromAccountId(account.Id))
            {
                var unitTestRepository = accountContext.GetUnitTestRepository();
                var unitTest = unitTestRepository.QueryAll()
                    .FirstOrDefault(t => t.SslCertificateExpirationDateRule != null && t.SslCertificateExpirationDateRule.Url == model.Url);

                Assert.NotNull(unitTest);
                Assert.Equal(SystemUnitTestTypes.SslTestType.Id, unitTest.TypeId);
                Assert.Equal((int)TimeSpan.FromDays(1).TotalSeconds, unitTest.PeriodSeconds);
                Assert.Equal(ObjectColor.Gray, unitTest.NoSignalColor);
                Assert.Equal(30, unitTest.SslCertificateExpirationDateRule.WarningDaysCount);
                Assert.Equal(14, unitTest.SslCertificateExpirationDateRule.AlarmDaysCount);
                Assert.True(unitTest.SimpleMode);
                Assert.Equal(2, unitTest.AttempMax);

                unitTestId = unitTest.Id;
            }

            // Отредактируем проверку
            using (var controller = new SslCertificateExpirationDateChecksController(account.Id, user.Id))
            {
                var result = (ViewResultBase)controller.EditSimple(unitTestId);
                model = (EditSimpleModel)result.Model;
            }

            model.Url = Guid.NewGuid().ToString();

            using (var accountContext = AccountDbContext.CreateFromAccountId(account.Id))
            {
                using (var controller = new SslCertificateExpirationDateChecksController(account.Id, user.Id))
                {
                    controller.EditSimple(model);

                    // Посмотрим, как изменилась проверка и правило
                    var unitTestRepository = accountContext.GetUnitTestRepository();
                    var unitTest = unitTestRepository.GetByIdOrNull(unitTestId);

                    Assert.NotNull(unitTest);
                    Assert.Equal(SystemUnitTestTypes.SslTestType.Id, unitTest.TypeId);
                    Assert.Equal((int)TimeSpan.FromDays(1).TotalSeconds, unitTest.PeriodSeconds);
                    Assert.Equal(model.Url, unitTest.SslCertificateExpirationDateRule.Url);

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
