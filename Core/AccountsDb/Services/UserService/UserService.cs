using System;
using System.Collections.Generic;
using System.Linq;
using Zidium.Core.Api;
using Zidium.Core.Common;
using Zidium.Core.ConfigDb;
using Zidium.Core.DispatcherLayer;

namespace Zidium.Core.AccountsDb
{
    public class UserService : IUserService
    {
        protected DatabasesContext Context { get; set; }

        public UserService(DatabasesContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            Context = context;
        }

        public User FindUser(string login, string accountName)
        {
            AccountInfo account = null;
            if (!string.IsNullOrEmpty(accountName))
            {
                account = ConfigDbServicesHelper.GetAccountService().GetOneOrNullBySystemName(accountName);
            }

            var logins = ConfigDbServicesHelper.GetLoginService().GetAllByLogin(login);

            if (logins.Length == 0)
                throw new WrongLoginException();

            LoginInfo loginInfo;
            if (logins.Length == 1)
            {
                loginInfo = logins[0];
                if (account != null && account.Id != loginInfo.Account.Id)
                    throw new WrongLoginException();
            }
            else
            {
                if (account == null)
                    throw new AccountRequiredException();

                loginInfo = logins.FirstOrDefault(t => t.Account.Id == account.Id);
                if (loginInfo == null)
                    throw new WrongLoginException();
            }

            if (loginInfo.Account.Status != AccountStatus.Active)
                throw new AccountIsBlockedException();

            var accountContext = Context.GetAccountDbContext(loginInfo.Account.Id);
            var userRepository = accountContext.GetUserRepository();
            var user = userRepository.GetOneOrNullByLogin(login);

            if (user == null)
                throw new WrongLoginException();

            return user;
        }

        public AuthInfo Auth(string login, string password, string accountName)
        {
            AccountInfo account = null;
            if (!string.IsNullOrEmpty(accountName))
            {
                account = ConfigDbServicesHelper.GetAccountService().GetOneOrNullBySystemName(accountName);
                if (account == null)
                {
                    throw new UserFriendlyException("Неизвестный аккаунт: " + accountName);
                }
            }

            var loginService = ConfigDbServicesHelper.GetLoginService();
            var logins = loginService.GetAllByLogin(login);

            // если явно указан аккаунт, то отфильтруем логины по аккаунту
            if (account != null)
            {
                logins = logins.Where(x => x.Account.Id == account.Id).ToArray();
            }

            if (logins.Length == 0)
                throw new WrongLoginException();

            var users = new List<Tuple<LoginInfo, User>>();
            foreach (var loginInfo in logins)
            {
                var accountDbContext = Context.GetAccountDbContext(loginInfo.Account.Id);
                var userRepository = accountDbContext.GetUserRepository();
                var user = userRepository.GetById(loginInfo.Id);
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

            Tuple<LoginInfo, User> item;
            if (users.Count == 1)
            {
                item = users[0];
                if (account != null && account.Id != item.Item1.Account.Id)
                    throw new WrongLoginException();
            }
            else
            {
                if (account == null)
                    throw new AccountRequiredException();

                item = users.FirstOrDefault(t => t.Item1.Account.Id == account.Id);
                if (item == null)
                    throw new WrongLoginException();
            }

            if (item.Item1.Account.Status != AccountStatus.Active)
                throw new AccountIsBlockedException();

            return new AuthInfo()
            {
                User = item.Item2,
                AccountId = item.Item1.Account.Id
            };
        }

        public User CreateAccountAdmin(
            Guid accountId,
            string email,
            string lastName,
            string firstName,
            string middleName,
            string post,
            string mobilePhone)
        {
            var user = new User()
            {
                Login = email,
                LastName = lastName,
                FirstName = firstName,
                MiddleName = middleName,
                Post = post
            };
            user.DisplayName = user.FioOrLogin;

            if (!string.IsNullOrEmpty(mobilePhone))
            {
                user.UserContacts.Add(new UserContact()
                {
                    Type = UserContactType.MobilePhone,
                    Value = mobilePhone
                });
            }

            user.Roles.Add(new UserRole()
            {
                RoleId = RoleId.AccountAdministrators
            });

            return CreateUser(user, accountId, false);
        }

        public User CreateUser(User user, Guid accountId, bool sendLetter = true)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            if (user.Login == null)
            {
                throw new Exception("Не заполнено поле Login");
            }

            var existingLogin = ConfigDbServicesHelper.GetLoginService().GetOneOrNull(accountId, user.Login);
            if (existingLogin != null)
                throw new LoginAlreadyExistsException(user.Login);

            user.SecurityStamp = Guid.NewGuid().ToString();
            user.DisplayName = user.DisplayName ?? user.Login;

            user.UserContacts.Add(new UserContact()
            {
                Type = UserContactType.Email,
                Value = user.Login
            });

            foreach (var role in user.Roles)
            {
                role.User = user;
                if (role.Id == Guid.Empty)
                    role.Id = Guid.NewGuid();
            }

            foreach (var contact in user.UserContacts)
            {
                contact.User = user;
                if (contact.Id == Guid.Empty)
                {
                    contact.Id = Guid.NewGuid();
                    contact.CreateDate = DateTime.Now;
                }
            }

            var accountContext = Context.GetAccountDbContext(accountId);
            var userRepository = accountContext.GetUserRepository();
            userRepository.Add(user);

            ConfigDbServicesHelper.GetLoginService().Add(user.Id, accountId, user.Login);

            Context.SaveChanges();

            // По умолчанию включаем отправку новостей
            var userSettingService = accountContext.GetUserSettingService();
            userSettingService.SendMeNews(user.Id, true);
            Context.SaveChanges();

            // Для нового пользователя нужно создать подписки
            var subscriptionService = new SubscriptionService(Context);
            subscriptionService.CreateDefaultForUser(accountId, user.Id);
            Context.SaveChanges();

            if (sendLetter)
            {
                var token = StartResetPassword(user.Id, false);
                using (var dispatcherContext = DispatcherContext.Create())
                {
                    var userService = dispatcherContext.UserService;
                    userService.SendNewUserLetter(accountId, user.Id, token);
                }
            }

            return user;
        }

        public Guid StartResetPassword(Guid loginId, bool sendLetter = true)
        {
            var loginInfo = ConfigDbServicesHelper.GetLoginService().GetOneById(loginId);
            var tokenService = new TokenService(Context);
            var token = tokenService.GenerateToken(loginInfo.Account.Id, loginInfo.Id, TokenPurpose.ResetPassword, TimeSpan.FromDays(1));

            if (sendLetter)
            {
                SendResetPasswordLetter(loginInfo.Account.Id, loginInfo.Id, token.Id);
            }

            return token.Id;
        }

        public void EndResetPassword(Guid accountId, Guid tokenId, string newPassword)
        {
            var tokenService = new TokenService(Context);
            var token = tokenService.UseToken(accountId, tokenId, TokenPurpose.ResetPassword);
            SetNewPassword(accountId, token.UserId, newPassword);
        }

        public void SetNewPassword(Guid accountId, Guid userId, string newPassword)
        {
            // получаем пользователя
            var accountDbContext = Context.GetAccountDbContext(accountId);
            var userRepository = accountDbContext.GetUserRepository();
            var user = userRepository.GetById(userId);

            // меняем пароль
            user.SecurityStamp = Guid.NewGuid().ToString();
            user.PasswordHash = PasswordHelper.GetPasswordHashString(newPassword);
            accountDbContext.SaveChanges();
        }

        public User GetById(Guid accountId, Guid id)
        {
            var accountDbContext = Context.GetAccountDbContext(accountId);
            var userRepository = accountDbContext.GetUserRepository();
            var user = userRepository.GetById(id);
            return user;
        }

        public User GetByIdOrNull(Guid accountId, Guid id)
        {
            var accountDbContext = Context.GetAccountDbContext(accountId);
            var userRepository = accountDbContext.GetUserRepository();
            var user = userRepository.GetByIdOrNull(id);

            return user;
        }

        public void SendNewUserLetter(Guid accountId, Guid userId, Guid token)
        {
            var account = ConfigDbServicesHelper.GetAccountService().GetOneById(accountId);
            var accountDbContext = Context.GetAccountDbContext(accountId);

            var userRepository = accountDbContext.GetUserRepository();
            var user = userRepository.GetById(userId);

            var url = UrlHelper.GetPasswordSetUrl(accountId, token, account.SystemName);
            var emailCommand = EmailMessageHelper.NewUserLetter(user.Login, url);
            emailCommand.ReferenceId = token;

            var emailCommandRepository = accountDbContext.GetSendEmailCommandRepository();
            emailCommandRepository.Add(emailCommand);
        }

        public void SendResetPasswordLetter(Guid accountId, Guid userId, Guid token)
        {
            var account = ConfigDbServicesHelper.GetAccountService().GetOneById(accountId);
            var accountDbContext = Context.GetAccountDbContext(accountId);
            var userRepository = accountDbContext.GetUserRepository();
            var user = userRepository.GetById(userId);

            var url = UrlHelper.GetPasswordSetUrl(accountId, token, account.SystemName);
            var emailCommand = EmailMessageHelper.ResetPasswordLetter(user.Login, url);
            emailCommand.ReferenceId = token;

            var emailCommandRepository = accountDbContext.GetSendEmailCommandRepository();
            emailCommandRepository.Add(emailCommand);
        }

        public User UpdateUserLogin(User user)
        {
            ConfigDbServicesHelper.GetLoginService().UpdateLogin(user.Id, user.Login);
            return user;
        }

        public void DeleteUser(User user, Guid accountId)
        {
            var accountContext = Context.GetAccountDbContext(accountId);
            var userRepository = accountContext.GetUserRepository();

            var isLastUser = userRepository.QueryAll().Count() == 1;
            if (isLastUser)
                throw new CantDeleteLastUserException();

            var isLastAdmin = !userRepository.QueryAll().Any(t => t.Id != user.Id && t.Roles.Any(x => x.RoleId == RoleId.AccountAdministrators));
            if (isLastAdmin)
                throw new CantDeleteLastAdminException();

            ConfigDbServicesHelper.GetLoginService().Delete(user.Id);
            userRepository.Remove(user);
        }

        public UserRole AddUserRole(User user, UserRole userRole)
        {
            userRole.User = user;
            if (userRole.Id == Guid.Empty)
                userRole.Id = Guid.NewGuid();
            user.Roles.Add(userRole);
            return userRole;
        }

        public void RemoveUserRole(User user, UserRole userRole, Guid accountId)
        {
            var accountContext = Context.GetAccountDbContext(accountId);
            var userRepository = accountContext.GetUserRepository();
            var isLastAdmin = !userRepository.QueryAll().Any(t => t.Id != user.Id && t.Roles.Any(x => x.RoleId == RoleId.AccountAdministrators));

            if (isLastAdmin && userRole.RoleId == RoleId.AccountAdministrators)
                throw new CantRemoveAdminRoleFromLastAdmin();

            accountContext.UserRoles.Remove(userRole);
            user.Roles.Remove(userRole);
        }

        public class AuthInfo
        {
            public User User { get; set; }

            public Guid AccountId { get; set; }
        }
    }
}
