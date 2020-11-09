using System;
using System.Collections.Generic;
using Xunit;
using Zidium.Core.AccountsDb;
using Zidium.Core.Common;
using Zidium.Core.ConfigDb;
using Zidium.Storage;
using Zidium.TestTools;

namespace Zidium.Core.Tests.Services
{
    public class UserServiceTests : BaseTest
    {
        [Fact]
        public void ValidUserAuthTest()
        {
            var account = TestHelper.GetTestAccount();
            var password = PasswordHelper.GetRandomPassword(10);
            var user = TestHelper.CreateTestUser(account.Id, password);
            var storage = TestHelper.GetStorage(account.Id);

            // Проверим, что можно зайти под созданным пользователем
            var service = new UserService(storage);
            var authInfo = service.Auth(user.Login, password, null);
            Assert.Equal(user.Id, authInfo.User.Id);
        }

        [Fact]
        public void WrongUserAuthTest()
        {
            var account = TestHelper.GetTestAccount();
            var password = PasswordHelper.GetRandomPassword(10);
            var user = TestHelper.CreateTestUser(account.Id, password);
            var storage = TestHelper.GetStorage(account.Id);

            // Проверим, что нельзя зайти под созданным пользователем с неправильным паролем
            var service = new UserService(storage);
            Assert.ThrowsAny<WrongLoginException>(() =>
            {
                service.Auth(user.Login, "-", null);
            });

            // Проверим, что нельзя зайти под несуществующим пользователем
            Assert.ThrowsAny<WrongLoginException>(() =>
            {
                service.Auth("-", password, null);
            });
        }

        [Fact]
        public void DeletedUserAuthTest()
        {
            var account = TestHelper.GetTestAccount();
            var password = PasswordHelper.GetRandomPassword(10);
            var user = TestHelper.CreateTestUser(account.Id, password);
            var storage = TestHelper.GetStorage(account.Id);

            // Удалим пользователя
            var service = new UserService(storage);
            service.DeleteUser(user.Id, account.Id);

            // Проверим, что нельзя зайти под удалённым пользователем
            Assert.ThrowsAny<WrongLoginException>(() =>
            {
                service.Auth(user.Login, password, null);
            });
        }

        [Fact]
        public void RestorePasswordTest()
        {
            var account = TestHelper.GetTestAccount();
            var oldPassword = PasswordHelper.GetRandomPassword(10);
            var newPassword = PasswordHelper.GetRandomPassword(20);
            var user = TestHelper.CreateTestUser(account.Id, oldPassword);
            var storage = TestHelper.GetStorage(account.Id);

            var service = new UserService(storage);

            // Инициируем смену пароля
            var token = service.StartResetPassword(user.Id, false);

            // Завершим смену пароля
            service.EndResetPassword(account.Id, token, newPassword);

            // Проверим, что нельзя зайти со старым паролем
            Assert.ThrowsAny<WrongLoginException>(() =>
            {
                service.Auth(user.Login, oldPassword, null);
            });

            // Проверим, что можно зайти с новым паролем
            var authInfo = service.Auth(user.Login, newPassword, null);
            Assert.Equal(user.Id, authInfo.User.Id);
        }

        [Fact]
        public void NotValidTokenTest()
        {
            var account = TestHelper.GetTestAccount();
            var oldPassword = PasswordHelper.GetRandomPassword(10);
            var newPassword = PasswordHelper.GetRandomPassword(20);
            var user = TestHelper.CreateTestUser(account.Id, oldPassword);
            var storage = TestHelper.GetStorage(account.Id);

            // Проверим, что нельзя поменять пароль по несуществующему токену
            var service = new UserService(storage);
            Assert.ThrowsAny<TokenNotValidException>(() => service.EndResetPassword(account.Id, Guid.NewGuid(), newPassword));

            // Проверим, что можно зайти со старым паролем
            var authInfo = service.Auth(user.Login, oldPassword, null);
            Assert.Equal(user.Id, authInfo.User.Id);

        }

        [Fact]
        public void MasterPasswordTest()
        {
            var account = TestHelper.GetTestAccount();
            var password = PasswordHelper.GetRandomPassword(10);
            var user = TestHelper.CreateTestUser(account.Id, password);
            var storage = TestHelper.GetStorage(account.Id);

            // Проверим, что можно зайти с мастер-паролем
            var configDbServicesFactory = DependencyInjection.GetServicePersistent<IConfigDbServicesFactory>();
            var masterPassword = configDbServicesFactory.GetLoginService().MasterPassword();
            if (masterPassword == null)
                return;

            var service = new UserService(storage);
            var authInfo = service.Auth(user.Login, masterPassword, null);
            Assert.Equal(user.Id, authInfo.User.Id);
        }

        [Fact]
        public void AddSameUsersInOneAccountTest()
        {
            // Проверим, что нельзя создать двух пользователей с одинаковым email в одном аккаунте             
            var account = TestHelper.GetTestAccount();
            var storage = TestHelper.GetStorage(account.Id);
            var email = Guid.NewGuid() + "@test.com";

            var userForAdd = new UserForAdd()
            {
                Id = Guid.NewGuid(),
                CreateDate = DateTime.Now,
                Login = email,
                FirstName = string.Empty,
                LastName = string.Empty
            };

            var service = new UserService(storage);
            service.CreateUser(userForAdd, new List<UserContactForAdd>(), new List<UserRoleForAdd>(), account.Id, false);

            var userForAdd2 = new UserForAdd()
            {
                Id = Guid.NewGuid(),
                CreateDate = DateTime.Now,
                Login = email,
                FirstName = string.Empty,
                LastName = string.Empty
            };

            Assert.ThrowsAny<LoginAlreadyExistsException>(() =>
            {
                service.CreateUser(userForAdd2, new List<UserContactForAdd>(), new List<UserRoleForAdd>(), account.Id, false);
            });
        }

        [Fact]
        public void CreateUserTest()
        {
            var account = TestHelper.GetTestAccount();
            var password = PasswordHelper.GetRandomPassword(10);
            var user = TestHelper.CreateTestUser(account.Id, password);
            var storage = TestHelper.GetStorage(account.Id);

            Assert.NotNull(user);

            // Проверим, что у нового пользователя по умолчанию включена отправка новостей
            var userSettingService = new UserSettingService(storage);
            var sendMeNews = userSettingService.SendMeNews(user.Id);
            Assert.True(sendMeNews);
        }

    }
}
