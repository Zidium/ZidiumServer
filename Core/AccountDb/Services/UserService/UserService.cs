using System;
using System.Collections.Generic;
using System.Linq;
using Zidium.Common;
using Zidium.Core.Api;
using Zidium.Core.Common;
using Zidium.Core.ConfigDb;
using Zidium.Storage;

namespace Zidium.Core.AccountsDb
{
    public class UserService : IUserService
    {
        public UserService(IStorage storage)
        {
            _storage = storage;
        }

        private readonly IStorage _storage;

        public AuthInfo FindUser(string login, string accountName)
        {
            var configDbServicesFactory = DependencyInjection.GetServicePersistent<IConfigDbServicesFactory>();

            AccountInfo account = null;
            if (!string.IsNullOrEmpty(accountName))
            {
                account = configDbServicesFactory.GetAccountService().GetOneOrNullBySystemName(accountName);
            }

            var logins = configDbServicesFactory.GetLoginService().GetAllByLogin(login);

            if (logins.Length == 0)
                throw new WrongLoginException();

            LoginInfo loginInfo;
            if (logins.Length == 1)
            {
                loginInfo = logins[0];
                if (account != null && account.Id != loginInfo.AccountId)
                    throw new WrongLoginException();
            }
            else
            {
                if (account == null)
                    throw new AccountRequiredException();

                loginInfo = logins.FirstOrDefault(t => t.AccountId == account.Id);
                if (loginInfo == null)
                    throw new WrongLoginException();
            }

            account = configDbServicesFactory.GetAccountService().GetOneById(loginInfo.AccountId);
            if (account.Status != AccountStatus.Active)
                throw new AccountIsBlockedException();

            var accountStorageFactory = DependencyInjection.GetServicePersistent<IAccountStorageFactory>();
            var storage = accountStorageFactory.GetStorageByDatabaseId(account.AccountDatabaseId);

            var user = storage.Users.GetOneOrNullByLogin(login);

            if (user == null)
                throw new WrongLoginException();

            return new AuthInfo()
            {
                User = user,
                AccountId = account.Id
            };
        }

        public AuthInfo Auth(string login, string password, string accountName)
        {
            var configDbServicesFactory = DependencyInjection.GetServicePersistent<IConfigDbServicesFactory>();

            AccountInfo account = null;
            if (!string.IsNullOrEmpty(accountName))
            {
                account = configDbServicesFactory.GetAccountService().GetOneOrNullBySystemName(accountName);
                if (account == null)
                {
                    throw new UserFriendlyException("Неизвестный аккаунт: " + accountName);
                }
            }

            var loginService = configDbServicesFactory.GetLoginService();
            var logins = loginService.GetAllByLogin(login);

            // если явно указан аккаунт, то отфильтруем логины по аккаунту
            if (account != null)
            {
                logins = logins.Where(x => x.AccountId == account.Id).ToArray();
            }

            if (logins.Length == 0)
                throw new WrongLoginException();

            var users = new List<Tuple<LoginInfo, UserForRead>>();
            var accountStorageFactory = DependencyInjection.GetServicePersistent<IAccountStorageFactory>();

            foreach (var loginInfo in logins)
            {
                var storage = accountStorageFactory.GetStorageByAccountId(loginInfo.AccountId);
                var user = storage.Users.GetOneById(loginInfo.Id);
                if (PasswordHelper.VerifyHashedPassword(user.PasswordHash, password))
                    users.Add(Tuple.Create(loginInfo, user));
                else
                {
                    var masterPassword = loginService.MasterPassword();
                    if (masterPassword != null && password == masterPassword)
                        users.Add(Tuple.Create(loginInfo, user));
                }
            }

            if (users.Count == 0)
                throw new WrongLoginException();

            Tuple<LoginInfo, UserForRead> item;
            if (users.Count == 1)
            {
                item = users[0];
                if (account != null && account.Id != item.Item1.AccountId)
                    throw new WrongLoginException();
            }
            else
            {
                if (account == null)
                    throw new AccountRequiredException();

                item = users.FirstOrDefault(t => t.Item1.AccountId == account.Id);
                if (item == null)
                    throw new WrongLoginException();
            }

            account = configDbServicesFactory.GetAccountService().GetOneById(item.Item1.AccountId);
            if (account.Status != AccountStatus.Active)
                throw new AccountIsBlockedException();

            return new AuthInfo()
            {
                User = item.Item2,
                AccountId = item.Item1.AccountId
            };
        }

        public Guid CreateAccountAdmin(Guid accountId,
            string email,
            string lastName,
            string firstName,
            string middleName,
            string post,
            string mobilePhone)
        {
            var user = new UserForAdd()
            {
                Id = Guid.NewGuid(),
                Login = email,
                LastName = lastName,
                FirstName = firstName,
                MiddleName = middleName,
                Post = post,
                CreateDate = DateTime.Now,
            };
            user.DisplayName = user.FioOrLogin();

            var contacts = new List<UserContactForAdd>();
            if (!string.IsNullOrEmpty(mobilePhone))
            {
                contacts.Add(new UserContactForAdd()
                {
                    Id = Guid.NewGuid(),
                    UserId = user.Id,
                    Type = UserContactType.MobilePhone,
                    Value = mobilePhone,
                    CreateDate = DateTime.Now
                });
            }

            var roles = new List<UserRoleForAdd>();
            roles.Add(new UserRoleForAdd()
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                RoleId = SystemRole.AccountAdministrators.Id
            });

            return CreateUser(user, contacts, roles, accountId, false);
        }

        public Guid CreateUser(
            UserForAdd user, 
            List<UserContactForAdd> contacts, 
            List<UserRoleForAdd> roles,
            Guid accountId, 
            bool sendLetter = true)
        {
            var configDbServicesFactory = DependencyInjection.GetServicePersistent<IConfigDbServicesFactory>();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            if (user.Login == null)
            {
                throw new Exception("Не заполнено поле Login");
            }

            var existingLogin = configDbServicesFactory.GetLoginService().GetOneOrNull(accountId, user.Login);
            if (existingLogin != null)
                throw new LoginAlreadyExistsException(user.Login);

            user.SecurityStamp = Guid.NewGuid().ToString();
            user.DisplayName = user.DisplayName ?? user.Login;

            contacts.Add(new UserContactForAdd()
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                Type = UserContactType.Email,
                Value = user.Login,
                CreateDate = DateTime.Now
            });

            using (var transaction = _storage.BeginTransaction())
            {
                _storage.Users.Add(user);
                _storage.UserContacts.Add(contacts.ToArray());
                _storage.UserRoles.Add(roles.ToArray());
                transaction.Commit();
            }

            configDbServicesFactory.GetLoginService().Add(user.Id, accountId, user.Login);

            // По умолчанию включаем отправку новостей
            var userSettingService = new UserSettingService(_storage);
            userSettingService.SendMeNews(user.Id, true);

            // Для нового пользователя нужно создать подписки
            var subscriptionService = new SubscriptionService(_storage);
            subscriptionService.CreateDefaultForUser(accountId, user.Id);

            if (sendLetter)
            {
                var token = StartResetPassword(user.Id, false);
                SendNewUserLetter(accountId, user.Id, token);
            }

            return user.Id;
        }

        public Guid StartResetPassword(Guid loginId, bool sendLetter = true)
        {
            var configDbServicesFactory = DependencyInjection.GetServicePersistent<IConfigDbServicesFactory>();
            var loginInfo = configDbServicesFactory.GetLoginService().GetOneById(loginId);
            var tokenService = new TokenService(_storage);
            var token = tokenService.GenerateToken(loginInfo.AccountId, loginInfo.Id, TokenPurpose.ResetPassword, TimeSpan.FromDays(1));

            if (sendLetter)
            {
                SendResetPasswordLetter(loginInfo.AccountId, loginInfo.Id, token.Id);
            }

            return token.Id;
        }

        public void EndResetPassword(Guid accountId, Guid tokenId, string newPassword)
        {
            var tokenService = new TokenService(_storage);
            var token = tokenService.UseToken(accountId, tokenId, TokenPurpose.ResetPassword);
            SetNewPassword(accountId, token.UserId, newPassword);
        }

        public void SetNewPassword(Guid accountId, Guid userId, string newPassword)
        {
            // получаем пользователя
            var user = _storage.Users.GetOneById(userId);

            // меняем пароль
            var userForUpdate = user.GetForUpdate();
            userForUpdate.SecurityStamp.Set(Guid.NewGuid().ToString());
            userForUpdate.PasswordHash.Set(PasswordHelper.GetPasswordHashString(newPassword));
            _storage.Users.Update(userForUpdate);
        }

        public void SendNewUserLetter(Guid accountId, Guid userId, Guid token)
        {
            var configDbServicesFactory = DependencyInjection.GetServicePersistent<IConfigDbServicesFactory>();
            var account = configDbServicesFactory.GetAccountService().GetOneById(accountId);
            var user = _storage.Users.GetOneById(userId);

            var url = UrlHelper.GetPasswordSetUrl(accountId, token, account.SystemName);
            var emailCommand = EmailMessageHelper.NewUserLetter(user.Login, url);
            emailCommand.ReferenceId = token;

            _storage.SendEmailCommands.Add(emailCommand);
        }

        public void SendResetPasswordLetter(Guid accountId, Guid userId, Guid token)
        {
            var configDbServicesFactory = DependencyInjection.GetServicePersistent<IConfigDbServicesFactory>();
            var account = configDbServicesFactory.GetAccountService().GetOneById(accountId);
            var user = _storage.Users.GetOneById(userId);

            var url = UrlHelper.GetPasswordSetUrl(accountId, token, account.SystemName);
            var emailCommand = EmailMessageHelper.ResetPasswordLetter(user.Login, url);
            emailCommand.ReferenceId = token;

            _storage.SendEmailCommands.Add(emailCommand);
        }

        public void UpdateUserLogin(Guid userId, string login)
        {
            var configDbServicesFactory = DependencyInjection.GetServicePersistent<IConfigDbServicesFactory>();
            configDbServicesFactory.GetLoginService().UpdateLogin(userId, login);
        }

        public void DeleteUser(Guid userId, Guid accountId)
        {
            var users = _storage.Users.GetAll();

            var isLastUser = users.Length == 1;
            if (isLastUser)
                throw new CantDeleteLastUserException();

            var isLastAdmin = !users.Any(t => t.Id != userId && _storage.UserRoles.GetByUserId(t.Id).Any(x => x.RoleId == SystemRole.AccountAdministrators.Id));
            if (isLastAdmin)
                throw new CantDeleteLastAdminException();

            var configDbServicesFactory = DependencyInjection.GetServicePersistent<IConfigDbServicesFactory>();
            configDbServicesFactory.GetLoginService().Delete(userId);
            
            var userForUpdate = new UserForUpdate(userId);
            userForUpdate.InArchive.Set(true);
            _storage.Users.Update(userForUpdate);
        }

        public Guid AddUserRole(Guid userId, UserRoleForAdd userRole)
        {
            userRole.UserId = userId;
            if (userRole.Id == Guid.Empty)
                userRole.Id = Guid.NewGuid();
            _storage.UserRoles.Add(userRole);
            return userRole.Id;
        }

        public void RemoveUserRole(Guid userId, Guid userRoleId, Guid accountId)
        {
            var users = _storage.Users.GetAll();
            var userRole = _storage.UserRoles.GetOneById(userRoleId);

            var isLastAdmin = !users.Any(t => t.Id != userId && _storage.UserRoles.GetByUserId(t.Id).Any(x => x.RoleId == SystemRole.AccountAdministrators.Id));

            if (isLastAdmin && userRole.RoleId == SystemRole.AccountAdministrators.Id)
                throw new CantRemoveAdminRoleFromLastAdmin();

            _storage.UserRoles.Delete(userRoleId);
        }

        public bool HasRole(Guid userId, Guid roleId)
        {
            return _storage.UserRoles.GetByUserId(userId).Any(x => x.RoleId == roleId);
        }

        public UserForRead GetAccountAdmin()
        {
            var users = _storage.Users.GetAll();
            return users.First(x => _storage.UserRoles.GetByUserId(x.Id).Any(y => y.RoleId == SystemRole.AccountAdministrators.Id));
        }

        public UserForRead[] GetAccountAdmins()
        {
            var users = _storage.Users.GetAll();
            return users.Where(x => _storage.UserRoles.GetByUserId(x.Id).Any(y => y.RoleId == SystemRole.AccountAdministrators.Id)).ToArray();
        }

        public bool IsAccountAdmin(Guid userId)
        {
            return HasRole(userId, SystemRole.AccountAdministrators.Id);
        }

        public bool IsUser(Guid userId)
        {
            return HasRole(userId, SystemRole.Users.Id);
        }

        public bool IsViewer(Guid userId)
        {
            return HasRole(userId, SystemRole.Viewers.Id);
        }

        public class AuthInfo
        {
            public UserForRead User { get; set; }

            public Guid AccountId { get; set; }
        }
    }
}
