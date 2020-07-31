using System;
using Zidium.Storage;

namespace Zidium.Core.ConfigDb
{
    internal class LoginService : ILoginService
    {
        public LoginService(IAccountService accountService)
        {
            _accountService = accountService;
        }

        private readonly IAccountService _accountService;

        public LoginInfo[] GetAllByLogin(string login)
        {
            var accountStorageFactory = DependencyInjection.GetServicePersistent<IAccountStorageFactory>();
            var accountStorage = accountStorageFactory.GetStorageByAccountId(_accountService.GetSystemAccount().Id);
            var user = accountStorage.Users.GetOneOrNullByLogin(login);

            if (user == null)
                return new LoginInfo[0];

            return new[] { GetLoginInfo(user) };
        }

        public LoginInfo GetOneById(Guid id)
        {
            var accountStorageFactory = DependencyInjection.GetServicePersistent<IAccountStorageFactory>();
            var accountStorage = accountStorageFactory.GetStorageByAccountId(_accountService.GetSystemAccount().Id);
            var user = accountStorage.Users.GetOneById(id);
            return GetLoginInfo(user);
        }

        public LoginInfo GetOneOrNullById(Guid id)
        {
            var accountStorageFactory = DependencyInjection.GetServicePersistent<IAccountStorageFactory>();
            var accountStorage = accountStorageFactory.GetStorageByAccountId(_accountService.GetSystemAccount().Id);
            var user = accountStorage.Users.GetOneOrNullById(id);
            return GetLoginInfo(user);
        }

        public LoginInfo GetOneOrNull(Guid accountId, string login)
        {
            var accountStorageFactory = DependencyInjection.GetServicePersistent<IAccountStorageFactory>();
            var accountStorage = accountStorageFactory.GetStorageByAccountId(_accountService.GetSystemAccount().Id);
            var user = accountStorage.Users.GetOneOrNullByLogin(login);
            return GetLoginInfo(user);
        }

        public void Delete(Guid id)
        {
        }

        public string MasterPassword()
        {
            return null;
        }

        private LoginInfo GetLoginInfo(UserForRead user)
        {
            if (user == null)
                return null;

            return new LoginInfo()
            {
                Id = user.Id,
                AccountId = _accountService.GetSystemAccount().Id,
                Login = user.Login
            };
        }

        public void Add(Guid id, Guid accountId, string login)
        {
        }

        public void UpdateLogin(Guid id, string login)
        {
        }

        public void UpdateLastEntryDate(Guid id, DateTime date)
        {
        }

        public void UpdateUserAgentTag(Guid id, Guid userAgentTag)
        {
        }
    }
}
