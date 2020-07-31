using System;
using System.Linq;
using System.Web.Mvc;
using Xunit;
using Zidium.Storage;
using Zidium.TestTools;
using Zidium.UserAccount.Controllers;
using Zidium.UserAccount.Models;

namespace Zidium.UserAccount.Tests
{
    public class WebUnitTestTypesTests
    {
        [Fact]
        public void ListTest()
        {
            var account = TestHelper.GetTestAccount();
            var user = TestHelper.GetAccountAdminUser(account.Id);
            var unitTestType = TestHelper.CreateTestUnitTestType(account.Id);
            account.SaveAllCaches();

            // Проверим список без фильтров
            using (var controller = new UnitTestTypesController(account.Id, user.Id))
            {
                var result = (ViewResultBase)controller.Index(unitTestType.Id.ToString());
                var model = (UnitTestTypeListModel)result.Model;
                Assert.True(model.Items.Any(t => t.Id == unitTestType.Id));
            }

            // Проверим фильтр по системному имени
            using (var controller = new UnitTestTypesController(account.Id, user.Id))
            {
                var result = (ViewResultBase)controller.Index(unitTestType.SystemName);
                var model = (UnitTestTypeListModel)result.Model;
                Assert.True(model.Items.Any(t => t.Id == unitTestType.Id));
            }

            // Проверим фильтр по дружелюбному имени
            using (var controller = new UnitTestTypesController(account.Id, user.Id))
            {
                var result = (ViewResultBase)controller.Index(unitTestType.DisplayName);
                var model = (UnitTestTypeListModel)result.Model;
                Assert.True(model.Items.Any(t => t.Id == unitTestType.Id));
            }

        }

        [Fact]
        public void AddTest()
        {
            var account = TestHelper.GetTestAccount();
            var user = TestHelper.GetAccountAdminUser(account.Id);

            // Добавим новый тип
            UnitTestTypeEditModel model;
            using (var controller = new UnitTestTypesController(account.Id, user.Id))
            {
                var result = (ViewResultBase)controller.Add();
                model = (UnitTestTypeEditModel)result.Model;
            }
            var name = "Новый тип проверки " + DateTime.Now.Ticks;
            model.DisplayName = name;
            model.SystemName = name;
            using (var controller = new UnitTestTypesController(account.Id, user.Id))
            {
                controller.Add(model);
            }

            // Проверим данные созданного типа
            using (var controller = new UnitTestTypesController(account.Id, user.Id))
            {
                var result = (ViewResultBase)controller.Index();
                var listModel = (UnitTestTypeListModel)result.Model;
                var item = listModel.Items.SingleOrDefault(t => t.SystemName == name);
                Assert.NotNull(item);
                Assert.Equal(model.DisplayName, item.DisplayName);
                Assert.Equal(model.SystemName, item.SystemName);
            }
        }

        [Fact]
        public void ShowTest()
        {
            var account = TestHelper.GetTestAccount();
            var user = TestHelper.GetAccountAdminUser(account.Id);
            var unitTestType = TestHelper.CreateTestUnitTestType(account.Id);

            using (var controller = new UnitTestTypesController(account.Id, user.Id))
            {
                var result = (ViewResultBase)controller.Show(unitTestType.Id);
                var model = (UnitTestTypeShowModel)result.Model;
                Assert.Equal(unitTestType.Id, model.Id);
                Assert.Equal(unitTestType.DisplayName, model.DisplayName);
                Assert.Equal(unitTestType.SystemName, model.SystemName);
            }
        }

        [Fact]
        public void EditTest()
        {
            var account = TestHelper.GetTestAccount();
            var user = TestHelper.GetAccountAdminUser(account.Id);
            var unitTestType = TestHelper.CreateTestUnitTestType(account.Id);

            // Отредактируем тип
            UnitTestTypeEditModel model;
            using (var controller = new UnitTestTypesController(account.Id, user.Id))
            {
                var result = (ViewResultBase)controller.Edit(unitTestType.Id);
                model = (UnitTestTypeEditModel)result.Model;
            }
            var name = "Новый юнит-тест " + Guid.NewGuid();
            model.DisplayName = name;
            model.SystemName = name;
            model.ActualTime = TimeSpan.FromMinutes(10);
            model.NoSignalColor.RedChecked = true;
            using (var controller = new UnitTestTypesController(account.Id, user.Id))
            {
                controller.Edit(model);
            }

            // Проверим данные изменённого типа
            using (var controller = new UnitTestTypesController(account.Id, user.Id))
            {
                var result = (ViewResultBase)controller.Index();
                var listModel = (UnitTestTypeListModel)result.Model;
                var item = listModel.Items.SingleOrDefault(t => t.Id == unitTestType.Id);
                Assert.NotNull(item);
                Assert.Equal(model.DisplayName, item.DisplayName);
                Assert.Equal(model.SystemName, item.SystemName);
                Assert.NotNull(model.ActualTime);
                Assert.Equal(10, model.ActualTime.Value.TotalMinutes);

                var noSignalColor = model.NoSignalColor.GetSelectedOne();
                Assert.NotNull(noSignalColor);
                Assert.Equal(ObjectColor.Red, noSignalColor.Value);
            }
        }

        [Fact]
        public void DeleteTest()
        {
            var account = TestHelper.GetTestAccount();
            var user = TestHelper.GetAccountAdminUser(account.Id);
            var unitTestType = TestHelper.CreateTestUnitTestType(account.Id);

            // Проверим, что тип есть в списке
            using (var controller = new UnitTestTypesController(account.Id, user.Id))
            {
                var result = (ViewResultBase)controller.Index(unitTestType.Id.ToString());
                var listModel = (UnitTestTypeListModel)result.Model;
                Assert.True(listModel.Items.Any(t => t.Id == unitTestType.Id));
            }

            // Удалим его
            DeleteConfirmationModel model;
            using (var controller = new UnitTestTypesController(account.Id, user.Id))
            {
                var result = (ViewResultBase)controller.Delete(unitTestType.Id);
                model = (DeleteConfirmationModel)result.Model;
            }
            model.ReturnUrl = "/";
            using (var controller = new UnitTestTypesController(account.Id, user.Id))
            {
                controller.Delete(model);
            }

            // Проверим, что типа нет в списке
            using (var controller = new UnitTestTypesController(account.Id, user.Id))
            {
                var result = (ViewResultBase)controller.Index(unitTestType.Id.ToString());
                var listModel = (UnitTestTypeListModel)result.Model;
                Assert.False(listModel.Items.Any(t => t.Id == unitTestType.Id));
            }
        }

    }
}
