﻿using System;
using System.Collections.Generic;
using Xunit;
using Zidium.Common;
using Zidium.Core.AccountsDb;
using Zidium.Core.Common;
using Zidium.Storage;
using Zidium.TestTools;

namespace Zidium.Core.Tests.Services
{
    public class UserServiceTests : BaseTest
    {
        [Fact]
        public void ValidUserAuthTest()
        {
            var password = PasswordHelper.GetRandomPassword(10);
            var user = TestHelper.CreateTestUser(password);
            var storage = TestHelper.GetStorage();
            var timeService = DependencyInjection.GetServicePersistent<ITimeService>();

            // Проверим, что можно зайти под созданным пользователем
            var service = new UserService(storage, timeService);
            var authInfo = service.Auth(user.Login, password);
            Assert.Equal(user.Id, authInfo.User.Id);
        }

        [Fact]
        public void WrongUserAuthTest()
        {
            var password = PasswordHelper.GetRandomPassword(10);
            var user = TestHelper.CreateTestUser(password);
            var storage = TestHelper.GetStorage();
            var timeService = DependencyInjection.GetServicePersistent<ITimeService>();

            // Проверим, что нельзя зайти под созданным пользователем с неправильным паролем
            var service = new UserService(storage, timeService);
            Assert.ThrowsAny<WrongLoginException>(() =>
            {
                service.Auth(user.Login, "-");
            });

            // Проверим, что нельзя зайти под несуществующим пользователем
            Assert.ThrowsAny<WrongLoginException>(() =>
            {
                service.Auth("-", password);
            });
        }

        [Fact]
        public void DeletedUserAuthTest()
        {
            var password = PasswordHelper.GetRandomPassword(10);
            var user = TestHelper.CreateTestUser(password);
            var storage = TestHelper.GetStorage();
            var timeService = DependencyInjection.GetServicePersistent<ITimeService>();

            // Удалим пользователя
            var service = new UserService(storage, timeService);
            service.DeleteUser(user.Id);

            // Проверим, что нельзя зайти под удалённым пользователем
            Assert.ThrowsAny<WrongLoginException>(() =>
            {
                service.Auth(user.Login, password);
            });
        }

        [Fact]
        public void RestorePasswordTest()
        {
            var oldPassword = PasswordHelper.GetRandomPassword(10);
            var newPassword = PasswordHelper.GetRandomPassword(20);
            var user = TestHelper.CreateTestUser(oldPassword);
            var storage = TestHelper.GetStorage();
            var timeService = DependencyInjection.GetServicePersistent<ITimeService>();

            var service = new UserService(storage, timeService);

            // Инициируем смену пароля
            var token = service.StartResetPassword(user.Id, false);

            // Завершим смену пароля
            service.EndResetPassword(token, newPassword);

            // Проверим, что нельзя зайти со старым паролем
            Assert.ThrowsAny<WrongLoginException>(() =>
            {
                service.Auth(user.Login, oldPassword);
            });

            // Проверим, что можно зайти с новым паролем
            var authInfo = service.Auth(user.Login, newPassword);
            Assert.Equal(user.Id, authInfo.User.Id);
        }

        [Fact]
        public void NotValidTokenTest()
        {
            var oldPassword = PasswordHelper.GetRandomPassword(10);
            var newPassword = PasswordHelper.GetRandomPassword(20);
            var user = TestHelper.CreateTestUser(oldPassword);
            var storage = TestHelper.GetStorage();
            var timeService = DependencyInjection.GetServicePersistent<ITimeService>();

            // Проверим, что нельзя поменять пароль по несуществующему токену
            var service = new UserService(storage, timeService);
            Assert.ThrowsAny<TokenNotValidException>(() => service.EndResetPassword(Ulid.NewUlid(), newPassword));

            // Проверим, что можно зайти со старым паролем
            var authInfo = service.Auth(user.Login, oldPassword);
            Assert.Equal(user.Id, authInfo.User.Id);

        }

        [Fact]
        public void MasterPasswordTest()
        {
            var password = PasswordHelper.GetRandomPassword(10);
            var user = TestHelper.CreateTestUser(password);
            var storage = TestHelper.GetStorage();
            var timeService = DependencyInjection.GetServicePersistent<ITimeService>();

            // Проверим, что можно зайти с мастер-паролем
            var logicConfiguration = DependencyInjection.GetServicePersistent<ILogicConfiguration>();
            var masterPassword = logicConfiguration.MasterPassword;
            if (masterPassword == null)
                return;

            var service = new UserService(storage, timeService);
            var authInfo = service.Auth(user.Login, masterPassword);
            Assert.Equal(user.Id, authInfo.User.Id);
        }

        [Fact]
        public void AddSameUsersInOneAccountTest()
        {
            // Проверим, что нельзя создать двух пользователей с одинаковым логином            
            var storage = TestHelper.GetStorage();
            var timeService = DependencyInjection.GetServicePersistent<ITimeService>();
            var login = "user_" + Ulid.NewUlid();

            var userForAdd = new UserForAdd()
            {
                Id = Ulid.NewUlid(),
                CreateDate = DateTime.Now,
                Login = login
            };

            var service = new UserService(storage, timeService);
            service.CreateUser(userForAdd, new List<UserContactForAdd>(), new List<UserRoleForAdd>(), false);

            var userForAdd2 = new UserForAdd()
            {
                Id = Ulid.NewUlid(),
                CreateDate = DateTime.Now,
                Login = login
            };

            Assert.ThrowsAny<LoginAlreadyExistsException>(() =>
            {
                service.CreateUser(userForAdd2, new List<UserContactForAdd>(), new List<UserRoleForAdd>(), false);
            });
        }

        [Fact]
        public void CreateUserTest()
        {
            var password = PasswordHelper.GetRandomPassword(10);
            var user = TestHelper.CreateTestUser(password);
            var storage = TestHelper.GetStorage();

            Assert.NotNull(user);

            // Проверим, что у нового пользователя по умолчанию включена отправка новостей
            var userSettingService = new UserSettingService(storage);
            var sendMeNews = userSettingService.SendMeNews(user.Id);
            Assert.True(sendMeNews);
        }

    }
}
