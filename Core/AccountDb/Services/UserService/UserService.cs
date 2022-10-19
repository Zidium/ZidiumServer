using System;
using System.Collections.Generic;
using System.Linq;
using Zidium.Common;
using Zidium.Core.Api.Dispatcher;
using Zidium.Core.Common;
using Zidium.Storage;

namespace Zidium.Core.AccountsDb
{
    // TODO Split to UserService and AuthService
    public class UserService : IUserService
    {
        public UserService(IStorage storage, ITimeService timeService)
        {
            _storage = storage;
            _timeService = timeService;
        }

        private readonly IStorage _storage;
        private readonly ITimeService _timeService;

        public AuthInfo FindUser(string login)
        {
            var user = _storage.Users.GetOneOrNullByLogin(login);

            if (user == null)
                return null;

            return new AuthInfo()
            {
                User = user
            };
        }

        public AuthInfo Auth(string login, string password)
        {
            var user = _storage.Users.GetOneOrNullByLogin(login);

            if (user == null)
            {
                throw new WrongLoginException();
            }

            if (PasswordHelper.VerifyHashedPassword(user.PasswordHash, password))
            {
                return new AuthInfo()
                {
                    User = user
                };
            }

            var masterPassword = new DispatcherClient("UserService").GetLogicSettings().GetDataAndCheck().MasterPassword;
            if (masterPassword != null && password == masterPassword)
            {
                return new AuthInfo()
                {
                    User = user
                };
            }

            throw new WrongLoginException();
        }

        public Guid CreateAccountAdmin(
            string login,
            string email,
            string mobilePhone)
        {
            var user = new UserForAdd()
            {
                Id = Ulid.NewUlid(),
                Login = login,
                EMail = email,
                CreateDate = _timeService.Now()
            };
            user.DisplayName = user.NameOrLogin();

            var contacts = new List<UserContactForAdd>();

            if (!string.IsNullOrEmpty(email))
            {
                contacts.Add(new UserContactForAdd()
                {
                    Id = Ulid.NewUlid(),
                    UserId = user.Id,
                    Type = UserContactType.Email,
                    Value = email,
                    CreateDate = _timeService.Now()
                });
            }

            if (!string.IsNullOrEmpty(mobilePhone))
            {
                contacts.Add(new UserContactForAdd()
                {
                    Id = Ulid.NewUlid(),
                    UserId = user.Id,
                    Type = UserContactType.MobilePhone,
                    Value = mobilePhone,
                    CreateDate = _timeService.Now()
                });
            }

            var roles = new List<UserRoleForAdd>();
            roles.Add(new UserRoleForAdd()
            {
                Id = Ulid.NewUlid(),
                UserId = user.Id,
                RoleId = SystemRole.AccountAdministrators.Id
            });

            return CreateUser(user, contacts, roles, false);
        }

        public Guid CreateUser(
            UserForAdd user,
            List<UserContactForAdd> contacts,
            List<UserRoleForAdd> roles,
            bool sendLetter = true)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            if (user.Login == null)
            {
                throw new Exception("Не заполнено поле Login");
            }

            var existingUser = _storage.Users.GetOneOrNullByLogin(user.Login);
            if (existingUser != null)
                throw new LoginAlreadyExistsException(user.Login);

            user.SecurityStamp = Ulid.NewUlid().ToString();

            using (var transaction = _storage.BeginTransaction())
            {
                _storage.Users.Add(user);
                _storage.UserContacts.Add(contacts.ToArray());
                _storage.UserRoles.Add(roles.ToArray());
                transaction.Commit();
            }

            // По умолчанию включаем отправку новостей
            var userSettingService = new UserSettingService(_storage);
            userSettingService.SendMeNews(user.Id, true);

            // Для нового пользователя нужно создать подписки
            var subscriptionService = new SubscriptionService(_storage, _timeService);
            subscriptionService.CreateDefaultForUser(user.Id);

            if (sendLetter)
            {
                var token = StartResetPassword(user.Id, false);
                SendNewUserLetter(user.Id, token);
            }

            return user.Id;
        }

        public Guid StartResetPassword(Guid userId, bool sendLetter = true)
        {
            var user = _storage.Users.GetOneById(userId);

            if (string.IsNullOrEmpty(user.Email))
                throw new UserFriendlyException("У данного пользователя нет привязанного email. Попросите администратора системы поменять ваш пароль.");

            var tokenService = new TokenService(_storage, _timeService);
            var token = tokenService.GenerateToken(user.Id, TokenPurpose.ResetPassword, TimeSpan.FromDays(1));

            if (sendLetter)
            {
                SendResetPasswordLetter(user.Id, token.Id);
            }

            return token.Id;
        }

        public void EndResetPassword(Guid tokenId, string newPassword)
        {
            var tokenService = new TokenService(_storage, _timeService);
            var token = tokenService.UseToken(tokenId, TokenPurpose.ResetPassword);
            SetNewPassword(token.UserId, newPassword);
        }

        public void SetNewPassword(Guid userId, string newPassword)
        {
            // получаем пользователя
            var user = _storage.Users.GetOneById(userId);

            // меняем пароль
            var userForUpdate = user.GetForUpdate();
            userForUpdate.SecurityStamp.Set(Ulid.NewUlid().ToString());
            userForUpdate.PasswordHash.Set(PasswordHelper.GetPasswordHashString(newPassword));
            _storage.Users.Update(userForUpdate);
        }

        public void SendNewUserLetter(Guid userId, Guid token)
        {
            var user = _storage.Users.GetOneById(userId);

            var url = UrlHelper.GetPasswordSetUrl(token);
            var emailCommand = EmailMessageHelper.NewUserLetter(user.Email, url);
            emailCommand.ReferenceId = token;
            emailCommand.CreateDate = _timeService.Now();
            emailCommand.Status = EmailStatus.InQueue;

            _storage.SendEmailCommands.Add(emailCommand);
        }

        public void SendResetPasswordLetter(Guid userId, Guid token)
        {
            var user = _storage.Users.GetOneById(userId);

            var url = UrlHelper.GetPasswordSetUrl(token);
            var emailCommand = EmailMessageHelper.ResetPasswordLetter(user.Email, url);
            emailCommand.ReferenceId = token;
            emailCommand.CreateDate = _timeService.Now();
            emailCommand.Status = EmailStatus.InQueue;

            _storage.SendEmailCommands.Add(emailCommand);
        }

        public void DeleteUser(Guid userId)
        {
            var users = _storage.Users.GetAll();

            var isLastUser = users.Length == 1;
            if (isLastUser)
                throw new CantDeleteLastUserException();

            var isLastAdmin = !users.Any(t => t.Id != userId && _storage.UserRoles.GetByUserId(t.Id).Any(x => x.RoleId == SystemRole.AccountAdministrators.Id));
            if (isLastAdmin)
                throw new CantDeleteLastAdminException();

            var userForUpdate = new UserForUpdate(userId);
            userForUpdate.InArchive.Set(true);
            _storage.Users.Update(userForUpdate);
        }

        public Guid AddUserRole(Guid userId, UserRoleForAdd userRole)
        {
            userRole.UserId = userId;
            if (userRole.Id == Guid.Empty)
                userRole.Id = Ulid.NewUlid();
            _storage.UserRoles.Add(userRole);
            return userRole.Id;
        }

        public void RemoveUserRole(Guid userId, Guid userRoleId)
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

        // TODO Unnecessary class
        public class AuthInfo
        {
            public UserForRead User { get; set; }
        }
    }
}
