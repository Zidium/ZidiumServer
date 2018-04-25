using System;
using Xunit;
using Zidium.Core.AccountsDb;
using Zidium.Core.Common;
using Zidium.Core.ConfigDb;
using Zidium.TestTools;

namespace Zidium.Core.Tests.Services
{
    public class UserServiceTests
    {
        [Fact]
        public void ValidUserAuthTest()
        {
            var account = TestHelper.GetTestAccount();
            var password = PasswordHelper.GetRandomPassword(10);
            var user = TestHelper.CreateTestUser(account.Id, password);

            // Проверим, что можно зайти под созданным пользователем
            using (var contexts = new DatabasesContext())
            {
                var service = new UserService(contexts);
                var authInfo = service.Auth(user.Login, password, null);
                Assert.Equal(user.Id, authInfo.User.Id);
            }
        }

        [Fact]
        public void WrongUserAuthTest()
        {
            var account = TestHelper.GetTestAccount();
            var password = PasswordHelper.GetRandomPassword(10);
            var user = TestHelper.CreateTestUser(account.Id, password);

            // Проверим, что нельзя зайти под созданным пользователем с неправильным паролем
            using (var contexts = new DatabasesContext())
            {
                var service = new UserService(contexts);
                Assert.ThrowsAny<WrongLoginException>(() =>
                {
                    service.Auth(user.Login, "-", null);
                });
            }

            // Проверим, что нельзя зайти под несуществующим пользователем
            using (var contexts = new DatabasesContext())
            {
                var service = new UserService(contexts);
                Assert.ThrowsAny<WrongLoginException>(() =>
                {
                    service.Auth("-", password, null);
                });
            }
        }

        [Fact]
        public void DeletedUserAuthTest()
        {
            var account = TestHelper.GetTestAccount();
            var password = PasswordHelper.GetRandomPassword(10);
            var user = TestHelper.CreateTestUser(account.Id, password);

            // Удалим пользователя
            using (var contexts = new DatabasesContext())
            {
                var service = new UserService(contexts);
                var dbUser = service.GetById(account.Id, user.Id);
                service.DeleteUser(dbUser, account.Id);
            }

            // Проверим, что нельзя зайти под удалённым пользователем
            using (var contexts = new DatabasesContext())
            {
                var service = new UserService(contexts);
                Assert.ThrowsAny<WrongLoginException>(() =>
                {
                    service.Auth(user.Login, password, null);
                });
            }
        }

        [Fact]
        public void RestorePasswordTest()
        {
            var account = TestHelper.GetTestAccount();
            var oldPassword = PasswordHelper.GetRandomPassword(10);
            var newPassword = PasswordHelper.GetRandomPassword(20);
            var user = TestHelper.CreateTestUser(account.Id, oldPassword);

            using (var contexts = new DatabasesContext())
            {
                var service = new UserService(contexts);

                // Инициируем смену пароля
                var token = service.StartResetPassword(user.Id, false);

                // Завершим смену пароля
                service.EndResetPassword(account.Id, token, newPassword);
            }

            // Проверим, что нельзя зайти со старым паролем
            using (var contexts = new DatabasesContext())
            {
                var service = new UserService(contexts);
                Assert.ThrowsAny<WrongLoginException>(() =>
                {
                    service.Auth(user.Login, oldPassword, null);
                });
            }

            // Проверим, что можно зайти с новым паролем
            using (var contexts = new DatabasesContext())
            {
                var service = new UserService(contexts);
                var authInfo = service.Auth(user.Login, newPassword, null);
                Assert.Equal(user.Id, authInfo.User.Id);
            }
        }

        [Fact]
        public void NotValidTokenTest()
        {
            var account = TestHelper.GetTestAccount();
            var oldPassword = PasswordHelper.GetRandomPassword(10);
            var newPassword = PasswordHelper.GetRandomPassword(20);
            var user = TestHelper.CreateTestUser(account.Id, oldPassword);

            // Проверим, что нельзя поменять пароль по несуществующему токену
            using (var contexts = new DatabasesContext())
            {
                var service = new UserService(contexts);
                Assert.ThrowsAny<TokenNotValidException>(() => service.EndResetPassword(account.Id, Guid.NewGuid(), newPassword));
            }

            // Проверим, что можно зайти со старым паролем
            using (var contexts = new DatabasesContext())
            {
                var service = new UserService(contexts);
                var authInfo = service.Auth(user.Login, oldPassword, null);
                Assert.Equal(user.Id, authInfo.User.Id);
            }

        }

        [Fact]
        public void MasterPasswordTest()
        {
            var account = TestHelper.GetTestAccount();
            var password = PasswordHelper.GetRandomPassword(10);
            var user = TestHelper.CreateTestUser(account.Id, password);

            // Проверим, что можно зайти с мастер-паролем
            var masterPassword = ConfigDbServicesHelper.GetLoginService().MasterPassword();
            if (masterPassword == null)
                return;

            using (var contexts = new DatabasesContext())
            {
                var service = new UserService(contexts);
                var authInfo = service.Auth(user.Login, masterPassword, null);
                Assert.Equal(user.Id, authInfo.User.Id);
            }
        }

        [Fact]
        public void AddSameUsersInOneAccountTest()
        {
            // Проверим, что нельзя создать двух пользователей с одинаковым email в одном аккаунте

            var account = TestHelper.GetTestAccount();
            var email = Guid.NewGuid() + "@test.com";

            using (var contexts = new DatabasesContext())
            {
                var user = new User()
                {
                    Login = email,
                    FirstName = string.Empty,
                    LastName = string.Empty
                };

                var service = new UserService(contexts);
                service.CreateUser(user, account.Id, false);
            }

            using (var contexts = new DatabasesContext())
            {
                var user = new User()
                {
                    Login = email,
                    FirstName = string.Empty,
                    LastName = string.Empty
                };

                var service = new UserService(contexts);
                Assert.ThrowsAny<LoginAlreadyExistsException>(() =>
                {
                    service.CreateUser(user, account.Id, false);
                });
            }
        }

        [Fact]
        public void CreateUserTest()
        {
            var account = TestHelper.GetTestAccount();
            var password = PasswordHelper.GetRandomPassword(10);
            var user = TestHelper.CreateTestUser(account.Id, password);

            Assert.NotNull(user);

            // Проверим, что у нового пользователя по умолчанию включена отправка новостей
            using (var context = AccountDbContext.CreateFromAccountId(account.Id))
            {
                var userSettingService = context.GetUserSettingService();
                var sendMeNews = userSettingService.SendMeNews(user.Id);
                Assert.True(sendMeNews);
            }
        }

    }
}
