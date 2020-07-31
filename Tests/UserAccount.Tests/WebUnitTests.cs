using System;
using System.Linq;
using System.Web.Mvc;
using Zidium.Core.Api;
using Xunit;
using Zidium.TestTools;
using Zidium.UserAccount.Controllers;
using Zidium.UserAccount.Models;

namespace Zidium.UserAccount.Tests
{
    public class WebUnitTests
    {
        [Fact]
        public void AddTest()
        {
            var account = TestHelper.GetTestAccount();
            var user = TestHelper.GetAccountAdminUser(account.Id);
            var component = account.CreateTestApplicationComponent();
            var unitTestType = TestHelper.CreateTestUnitTestType(account.Id);

            // Добавим новую проверку
            UnitTestAddModel model;
            using (var controller = new UnitTestsController(account.Id, user.Id))
            {
                var result = (ViewResultBase)controller.Add(component.Id);
                model = (UnitTestAddModel)result.Model;
            }
            Assert.Equal(component.Id, model.ComponentId);

            model.DisplayName = "Новая проверка " + DateTime.Now.Ticks;
            model.UnitTestTypeId = unitTestType.Id;
            
            using (var controller = new UnitTestsController(account.Id, user.Id))
            {
                controller.Add(model);
            }

            // Проверим данные созданной проверки
            using (var accountContext = TestHelper.GetAccountDbContext(account.Id))
            {
                var unitTest = accountContext.UnitTests.SingleOrDefault(t => t.DisplayName == model.DisplayName);
                Assert.NotNull(unitTest);
                Assert.Equal(model.UnitTestTypeId, unitTest.TypeId);
                Assert.Equal(model.ComponentId, unitTest.ComponentId);
                Assert.Equal(model.DisplayName, unitTest.DisplayName);
            }
        }

        [Fact]
        public void DeleteTest()
        {
            var account = TestHelper.GetTestAccount();
            var user = TestHelper.GetAccountAdminUser(account.Id);
            var component = account.CreateTestApplicationComponent();
            var unitTestType = TestHelper.CreateTestUnitTestType(account.Id);
            var service = DispatcherHelper.GetDispatcherService();

            // Создадим проверку
            var request = new GetOrCreateUnitTestRequest()
            {
                Token = account.GetCoreToken(),
                Data = new GetOrCreateUnitTestRequestData()
                {
                    DisplayName = "Новый юнит-тест " + DateTime.Now.Ticks,
                    SystemName = "Test.UnitTestType " + DateTime.Now.Ticks,
                    ComponentId = component.Id,
                    UnitTestTypeId = unitTestType.Id
                }
            };
            var response = service.GetOrCreateUnitTest(request);
            Assert.True(response.Success);
            var unitTestId = response.Data.Id;

            // Проверим, что проверка есть в списке
            using (var controller = new UnitTestsController(account.Id, user.Id))
            {
                var result = (ViewResultBase)controller.Index(componentId: component.Id);
                var listModel = (UnitTestsListModel)result.Model;
                var unitTestItems = listModel.UnitTestTypes.FirstOrDefault(t => t.UnitTestType.Id == unitTestType.Id);
                Assert.NotNull(unitTestItems);
                var unitTestItem = unitTestItems.UnitTests.FirstOrDefault(t => t.Id == unitTestId);
                Assert.NotNull(unitTestItem);
            }

            // Удалим проверку
            using (var controller = new UnitTestsController(account.Id, user.Id))
            {
                controller.Delete(unitTestId, "fake");
            }
            account.SaveAllCaches();

            // Проверим, что проверки нет в списке
            using (var controller = new UnitTestsController(account.Id, user.Id))
            {
                var result = (ViewResultBase)controller.Index(componentId: component.Id);
                var listModel = (UnitTestsListModel)result.Model;
                var unitTestItems = listModel.UnitTestTypes.FirstOrDefault(t => t.UnitTestType.Id == unitTestType.Id);
                Assert.Null(unitTestItems);
            }
        }

    }
}
