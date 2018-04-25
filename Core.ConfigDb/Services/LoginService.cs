using System;
using System.Linq;
using Zidium.Core.AccountsDb;

namespace Zidium.Core.ConfigDb
{
    internal class LoginService : ILoginService
    {
        public LoginInfo[] GetAllByLogin(string login)
        {
            using (var context = AccountDbContext.CreateFromConnectionString(DatabaseService.ConnectionString))
            {
                var userRepository = context.GetUserRepository();
                var users = userRepository.QueryAll().Where(t => t.Login == login).ToArray();
                return users.Select(GetLoginInfo).ToArray();
            }
        }

        public LoginInfo GetOneById(Guid id)
        {
            using (var context = AccountDbContext.CreateFromConnectionString(DatabaseService.ConnectionString))
            {
                var userRepository = context.GetUserRepository();
                var user = userRepository.GetById(id);
                return GetLoginInfo(user);
            }
        }

        public LoginInfo GetOneOrNullById(Guid id)
        {
            using (var context = AccountDbContext.CreateFromConnectionString(DatabaseService.ConnectionString))
            {
                var userRepository = context.GetUserRepository();
                var user = userRepository.GetByIdOrNull(id);
                return GetLoginInfo(user);
            }
        }

        public LoginInfo GetOneOrNull(Guid accountId, string login)
        {
            using (var context = AccountDbContext.CreateFromConnectionString(DatabaseService.ConnectionString))
            {
                var userRepository = context.GetUserRepository();
                var user = userRepository.GetOneOrNullByLogin(login);
                return GetLoginInfo(user);
            }
        }

        public LoginInfo Add(Guid id, Guid accountId, string login)
        {
            return new LoginInfo()
            {
                Id = id,
                Account = new AccountService().GetSystemAccount(),
                Login = login
            };
        }

        public LoginInfo UpdateLogin(Guid id, string login)
        {
            return new LoginInfo()
            {
                Id = id,
                Account = new AccountService().GetSystemAccount(),
                Login = login
            };
        }

        public LoginInfo UpdateLastEntryDate(Guid id, DateTime date)
        {
            return null;
        }

        public LoginInfo UpdateUserAgentTag(Guid id, Guid userAgentTag)
        {
            return null;
        }

        public void Delete(Guid id)
        {
        }

        public string MasterPassword()
        {
            return null;
        }

        private LoginInfo GetLoginInfo(User user)
        {
            if (user == null)
                return null;
            return new LoginInfo()
            {
                Id = user.Id,
                Account = new AccountService().GetSystemAccount(),
                Login = user.Login
            };
        }
    }
}
