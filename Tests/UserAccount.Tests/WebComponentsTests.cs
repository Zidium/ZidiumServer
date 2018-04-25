using System;
using System.Linq;
using System.Web.Mvc;
using Xunit;
using Zidium.Core.AccountsDb;
using Zidium.TestTools;
using Zidium.UserAccount.Controllers;
using Zidium.UserAccount.Models;

namespace Zidium.UserAccount.Tests
{
    public class WebComponentsTests
    {
        [Fact]
        public void AddTest()
        {
            var account = TestHelper.GetTestAccount();
            var user = TestHelper.GetAccountAdminUser(account.Id);
            var componentTypeId = TestHelper.CreateRandomComponentTypeId(account.Id);

            ComponentAddModel model;
            using (var controller = new ComponentsController(account.Id, user.Id))
            {
                var result = (ViewResultBase)controller.Add();
                model = (ComponentAddModel)result.Model;
            }

            model.SystemName = Guid.NewGuid().ToString();
            model.DisplayName = Guid.NewGuid().ToString();
            model.ComponentTypeId = componentTypeId;
            model.Version = "1.2.3.4";

            using (var controller = new ComponentsController(account.Id, user.Id))
            {
                controller.Add(model);
            }

            var accountContext = AccountDbContext.CreateFromAccountId(account.Id);
            var component = accountContext.Components.Single(x=>x.SystemName == model.SystemName);

            Assert.NotNull(component);
            Assert.Equal(model.SystemName, component.SystemName);
            Assert.Equal(model.DisplayName, component.DisplayName);
            Assert.Equal(model.ComponentTypeId, component.ComponentTypeId);
            Assert.Equal(model.Version, component.Version);
        }

        [Fact]
        public void DeleteTest()
        {
            var account = TestHelper.GetTestAccount();
            var user = TestHelper.GetAccountAdminUser(account.Id);

            // Создадим компонент
            var componentInfo = account.CreateTestApplicationComponent();

            // Удалим его
            using (var controller = new ComponentsController(account.Id, user.Id))
            {
                controller.Delete(componentInfo.Id, "fake");
            }

            // Проверим, что компонент удалился
            var accountContext = AccountDbContext.CreateFromAccountId(account.Id);
            var repository = accountContext.GetComponentRepository();
            var component = repository.GetByIdOrNull(componentInfo.Id);
            Assert.NotNull(component);
            Assert.True(component.IsDeleted);
        }
    }
}
