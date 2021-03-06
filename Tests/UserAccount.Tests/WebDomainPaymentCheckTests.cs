﻿using System;
using System.Linq;
using System.Web.Mvc;
using Xunit;
using Zidium.Core.AccountsDb;
using Zidium.Storage;
using Zidium.TestTools;
using Zidium.UserAccount.Controllers;
using Zidium.UserAccount.Models.DomainNamePaymentPeriodCheckModels;

namespace Zidium.UserAccount.Tests
{
    public class WebDomainPaymentCheckTests : BaseTest
    {
        [Fact]
        public void EditSimpleTest()
        {
            var account = TestHelper.GetTestAccount();
            var user = TestHelper.CreateTestUser(account.Id);
            EditSimpleModel model;
            Guid unitTestId;

            // Добавим проверку
            using (var controller = new DomainNamePaymentPeriodChecksController(account.Id, user.Id))
            {
                var result = (ViewResultBase)controller.EditSimple();
                model = (EditSimpleModel)result.Model;
            }

            model.Domain = Guid.NewGuid().ToString();

            using (var controller = new DomainNamePaymentPeriodChecksController(account.Id, user.Id))
            {
                controller.EditSimple(model);
            }

            // Посмотрим, как создалась проверка и правило
            using (var accountContext = TestHelper.GetAccountDbContext(account.Id))
            {
                var unitTest = accountContext.UnitTests
                    .FirstOrDefault(t => t.DomainNamePaymentPeriodRule != null && t.DomainNamePaymentPeriodRule.Domain == model.Domain);

                Assert.NotNull(unitTest);
                Assert.Equal(unitTest.ErrorColor, UnitTestResult.Alarm);
                Assert.Equal(unitTest.NoSignalColor, ObjectColor.Gray);
                Assert.Null(unitTest.ActualTimeSecs);
                Assert.Equal(SystemUnitTestType.DomainNameTestType.Id, unitTest.TypeId);
                Assert.Equal((int)TimeSpan.FromDays(1).TotalSeconds, unitTest.PeriodSeconds);
                Assert.Equal(30, unitTest.DomainNamePaymentPeriodRule.WarningDaysCount);
                Assert.Equal(14, unitTest.DomainNamePaymentPeriodRule.AlarmDaysCount);
                Assert.True(unitTest.SimpleMode);
                Assert.Equal(2, unitTest.AttempMax);

                unitTestId = unitTest.Id;
            }

            // Отредактируем проверку
            using (var controller = new DomainNamePaymentPeriodChecksController(account.Id, user.Id))
            {
                var result = (ViewResultBase)controller.EditSimple(unitTestId);
                model = (EditSimpleModel)result.Model;
            }

            model.Domain = Guid.NewGuid().ToString();

            using (var accountContext = TestHelper.GetAccountDbContext(account.Id))
            {
                using (var controller = new DomainNamePaymentPeriodChecksController(account.Id, user.Id))
                {
                    controller.EditSimple(model);

                    // Посмотрим, как изменилась проверка и правило
                    var unitTest = accountContext.UnitTests.Find(unitTestId);

                    Assert.NotNull(unitTest);
                    Assert.Equal(SystemUnitTestType.DomainNameTestType.Id, unitTest.TypeId);
                    Assert.Equal((int)TimeSpan.FromDays(1).TotalSeconds, unitTest.PeriodSeconds);
                    Assert.Equal(model.Domain, unitTest.DomainNamePaymentPeriodRule.Domain);

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
