using System;
using System.Linq;
using System.Web.Mvc;
using Xunit;
using Zidium.Core.AccountsDb;
using Zidium.Core.Api;
using Zidium.Core.Common;
using Zidium.Storage;
using Zidium.TestTools;
using Zidium.UserAccount.Controllers;
using Zidium.UserAccount.Models;
using Zidium.UserAccount.Models.Home;

namespace Zidium.UserAccount.Tests
{
    public class WebHomeTests : BaseTest
    {
        [Fact]
        public void QuickAddMobilePhoneTest()
        {
            var account = TestHelper.GetTestAccount();
            var user = TestHelper.CreateTestUser(account.Id);
            var phone = "+7 (926) 666-77-88";

            // Проверим, что у пользователя нет мобильного телефона с заданным номером
            using (var context = TestHelper.GetAccountDbContext(account.Id))
            {
                var dbUser = context.Users.Find(user.Id);
                Assert.NotNull(dbUser);
                var hasMobilePhone = dbUser.UserContacts.Any(t => t.Type == UserContactType.MobilePhone && t.Value == phone);
                Assert.False(hasMobilePhone);
            }

            // Добавим мобильный телефон
            QuickSetMobilePhoneModel model;
            using (var controller = new HomeController(account.Id, user.Id))
            {
                var result = (ViewResultBase)controller.QuickSetMobilePhone();
                model = (QuickSetMobilePhoneModel)result.Model;
            }

            model.Phone = phone;

            using (var controller = new HomeController(account.Id, user.Id))
            {
                var result = controller.QuickSetMobilePhone(model);
                Assert.IsAssignableFrom<RedirectResult>(result);
            }

            // Проверим, что телефон появился у пользователя

            using (var context = TestHelper.GetAccountDbContext(account.Id))
            {
                var dbUser = context.Users.Find(user.Id);
                Assert.NotNull(dbUser);
                var hasMobilePhone = dbUser.UserContacts.Any(t => t.Type == UserContactType.MobilePhone && t.Value == phone);
                Assert.True(hasMobilePhone);
            }

            // Проверим, что подписки по sms включены
            var client = TestHelper.GetDispatcherClient();

            var subscription = client.CreateSubscription(account.Id, new CreateSubscriptionRequestData()
            {
                Channel = SubscriptionChannel.Sms,
                UserId = user.Id,
                Object = SubscriptionObject.Default
            }).Data;

            Assert.True(subscription.IsEnabled);

        }

        [Fact]
        public void SetPasswordTest()
        {
            var account = TestHelper.GetTestAccount();
            var user = TestHelper.CreateTestUser(account.Id);
            var storage = TestHelper.GetStorage(account.Id);

            // Получим токен на смену пароля
            Guid tokenId;

            var service = new UserService(storage);
            tokenId = service.StartResetPassword(user.Id, false);

            // Укажем новый пароль
            SetPasswordModel model;
            using (var controller = new HomeController(null, null))
            {
                var result = (ViewResultBase)controller.SetPassword(tokenId, account.Id);
                model = (SetPasswordModel)result.Model;
            }

            var newPassword = PasswordHelper.GetRandomPassword(20);
            model.Password = newPassword;
            model.PasswordConfirmation = newPassword;

            using (var controller = new HomeController(null, null))
            {
                controller.SetPassword(model);
            }

            // Проверим, что можно зайти с новым паролем
            var authInfo = service.Auth(user.Login, newPassword, null);
            Assert.Equal(user.Id, authInfo.User.Id);
        }
    }
}
