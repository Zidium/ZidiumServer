using System;
using System.Linq;
using System.Threading;
using System.Web.Mvc;
using NLog;
using Zidium.Core.AccountsDb;
using Xunit;
using Zidium.Agent.AgentTasks.HttpRequests;
using Zidium.Core.Common;
using Zidium.Core.Common.Helpers;
using Zidium.TestTools;
using Zidium.UserAccount.Controllers;
using Zidium.UserAccount.Models.HttpRequestCheckModels;

namespace Zidium.UserAccount.Tests
{
    public class WebHttpRequestUnitTests
    {
        /// <summary>
        /// Был дефект, что после сохранения проверки, она начинала постоянно выполняться, как будет период равен 1 секунде
        /// </summary>
        [Fact]
        public void HttpRequestUnitTestAddTest()
        {
            var account = TestHelper.GetTestAccount();
            var user = TestHelper.GetAccountAdminUser(account.Id);
            var component = account.CreateRandomComponentControl();

            // Добавим новый юнит-тест
            EditModel model;
            using (var controller = new HttpRequestCheckController(account.Id, user.Id))
            {
                var result = (ViewResultBase) controller.Edit(null, component.Info.Id);
                model = (EditModel) result.Model;
            }
            Assert.Equal(component.Info.Id, model.ComponentId);

            model.CheckName = "Test_" + Guid.NewGuid();
            model.Method = HttpRequestMethod.Get;
            model.Body = "Body";
            model.ResponseCode = 200;
            model.Url = "http://recursion.ru";

            using (var controller = new HttpRequestCheckController(account.Id, user.Id))
            {
                controller.Edit(model);
            }
            account.SaveAllCaches();

            // Проверим данные созданного юнит-теста
            Guid unittestId;
            using (var accountContext = AccountDbContext.CreateFromAccountId(account.Id))
            {
                var repository = accountContext.GetUnitTestRepository();
                var unitTest = repository.QueryAll().SingleOrDefault(t => t.DisplayName == model.CheckName);
                Assert.NotNull(unitTest);
                Assert.Equal(SystemUnitTestTypes.HttpUnitTestType.Id, unitTest.TypeId);
                Assert.Equal(model.ComponentId, unitTest.ComponentId);
                Assert.Equal(model.CheckName, unitTest.DisplayName);
                Assert.Equal(model.Period, TimeSpanHelper.FromSeconds(unitTest.PeriodSeconds));
                Assert.Equal(ObjectColor.Gray, unitTest.NoSignalColor);
                Assert.Equal(2, unitTest.AttempMax);
                var httpUnitTest = unitTest.HttpRequestUnitTest;
                Assert.NotNull(httpUnitTest);
                var rule = unitTest.HttpRequestUnitTest.Rules.Single();
                Assert.NotNull(rule);
                Assert.NotNull(rule.Url);
                Assert.Equal("Body", rule.Body);
                unittestId = unitTest.Id;
            }

            // 1-ый запуск проверок
            // проверим, что агент НЕ будет выполнять проверку постоянно (был такой дефект, как будто период равен нулю)
            var processor = new HttpRequestsProcessor(LogManager.GetCurrentClassLogger(), new CancellationToken());
            processor.ProcessAccount(account.Id, unittestId);

            Assert.Equal(0, processor.ErrorCount);
            Assert.Equal(1, processor.SuccessCount);
            account.SaveAllCaches();

            // 2-ый запуск проверок
            processor = new HttpRequestsProcessor(LogManager.GetCurrentClassLogger(), new CancellationToken());
            processor.ProcessAccount(account.Id, unittestId);

            // проверка уже выполнена, время следующего выполнения еще НЕ настало
            Assert.Equal(0, processor.ErrorCount);
            Assert.Equal(0, processor.SuccessCount);
        }
    }
}
