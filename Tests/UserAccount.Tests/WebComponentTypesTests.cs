using System;
using System.Web.Mvc;
using Xunit;
using Zidium.Core.AccountsDb;
using Zidium.TestTools;
using Zidium.UserAccount.Controllers;
using Zidium.UserAccount.Models;

namespace Zidium.UserAccount.Tests
{
    public class WebComponentTypesTests
    {
        [Fact]
        public void AddTest()
        {
            var account = TestHelper.GetTestAccount();
            var user = TestHelper.GetAccountAdminUser(account.Id);

            ComponentTypeEditModel model;
            using (var controller = new ComponentTypesController(account.Id, user.Id))
            {
                var result = (ViewResultBase)controller.Add();
                model = (ComponentTypeEditModel)result.Model;
            }

            model.SystemName = Guid.NewGuid().ToString();
            model.DisplayName = Guid.NewGuid().ToString();

            using (var controller = new ComponentTypesController(account.Id, user.Id))
            {
                controller.Add(model);
            }

            var accountContext = AccountDbContext.CreateFromAccountId(account.Id);
            var repository = accountContext.GetComponentTypeRepository();
            var componentType = repository.GetOneOrNullBySystemName(model.SystemName);

            Assert.NotNull(componentType);
            Assert.False(componentType.IsSystem);
            Assert.Equal(model.SystemName, componentType.SystemName);
            Assert.Equal(model.DisplayName, componentType.DisplayName);
        }

        [Fact]
        public void DeleteTest()
        {
            var account = TestHelper.GetTestAccount();
            var user = TestHelper.GetAccountAdminUser(account.Id);

            // Создадим тип компонента
            var componentTypeInfo = TestHelper.CreateRandomComponentType(account.Id);

            // Удалим его
            DeleteConfirmationModel model;
            using (var controller = new ComponentTypesController(account.Id, user.Id))
            {
                var result = (ViewResultBase)controller.Delete(componentTypeInfo.Id);
                model = (DeleteConfirmationModel)result.Model;
            }
            model.ReturnUrl = "/";
            using (var controller = new ComponentTypesController(account.Id, user.Id))
            {
                controller.Delete(model);
            }

            // Проверим, что компонент удалился
            var accountContext = AccountDbContext.CreateFromAccountId(account.Id);
            var repository = accountContext.GetComponentTypeRepository();
            var componentType = repository.GetByIdOrNull(componentTypeInfo.Id);
            Assert.NotNull(componentType);
            Assert.True(componentType.IsDeleted);

        }

    }
}
