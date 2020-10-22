using System;
using Zidium.Common;
using Zidium.Core.Api;
using Zidium.Core.Api.Accounts.ChangeApiKey;
using Zidium.Core.Common.Helpers;

namespace Zidium.Core.ConfigDb
{
    public class AccountService : IAccountService
    {
        public AccountInfo[] GetAccounts(GetAccountsRequestData data)
        {
            return new[] { _accountInfo };
        }

        public AccountInfo GetOneOrNullBySystemName(string name)
        {
            return _accountInfo;
        }

        public AccountInfo GetOneOrNullById(Guid accountId)
        {
            return _accountInfo;
        }

        public AccountInfo GetOneById(Guid accountId)
        {
            return _accountInfo;
        }

        public AccountInfo GetSystemAccount()
        {
            return _accountInfo;
        }

        public string ChangeApiKey(ChangeApiKeyRequestData data)
        {
            return AccountSecretKey;
        }

        public void Update(UpdateAccountRequestData data)
        {
        }

        public static readonly Guid AccountId = new Guid("11111111-1111-1111-1111-111111111111");

        private static AccountInfo _accountInfo = new AccountInfo()
        {
            Id = AccountId,
            AccountDatabaseId = new Guid("22222222-2222-2222-2222-222222222222"),
            CreationDate = new DateTime(2017, 01, 01),
            DisplayName = "Main account",
            LastOverLimitDate = null,
            OverlimitEmailDate = null,
            RootId = new Guid("33333333-3333-3333-3333-333333333333"),
            SecretKey = AccountSecretKey,
            Status = AccountStatus.Active,
            SystemName = SystemAccountHelper.SystemAccountName,
            Type = AccountType.System,
            UserAgentTag = null,
            DatabaseConnectionString = DependencyInjection.GetServicePersistent<IDatabaseConfiguration>().ConnectionString
        };

        public static string AccountSecretKey
        {
            get
            {
                if (_accountSecretKey == null)
                {
                    _accountSecretKey = DependencyInjection.GetServicePersistent<IConfigDbConfiguration>().AccountSecretKey;
                    if (_accountSecretKey == null)
                        _accountSecretKey = Guid.NewGuid().ToString();
                }

                return _accountSecretKey;
            }
        }

        private static string _accountSecretKey;
    }
}
