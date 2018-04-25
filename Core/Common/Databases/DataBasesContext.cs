using System;
using System.Collections.Generic;
using Zidium.Core.AccountsDb;
using Zidium.Core.Api;
using Zidium.Core.ConfigDb;

namespace Zidium.Core.Common
{
    public class DatabasesContext : IDisposable
    {
        public bool IsInternalDispatcherContext { get; set; }

        public string ConfigDbConnectionString;

        private Dictionary<Guid, AccountDbContext> _accountDbContexts = new Dictionary<Guid, AccountDbContext>();

        public AccountDbContext GetAccountDbContext(Guid accountId)
        {
            if (IsInternalDispatcherContext)
            {
                var account = ConfigDbServicesHelper.GetAccountService().GetOneById(accountId);
                return GetAccountDbContextByDataBaseId(account.AccountDatabaseId);

            }
            else
            {
                var client = DispatcherHelper.GetDispatcherClient();
                var account = client.GetAccountById(new GetAccountByIdRequestData() { Id = accountId }).Data;
                return GetAccountDbContextByDataBaseId(account.AccountDatabaseId);
            }
        }

        public AccountDbContext GetAccountDbContextByDataBaseId(Guid databaseId)
        {
            AccountDbContext result;
            if (_accountDbContexts.TryGetValue(databaseId, out result))
            {
                return result;
            }
            if (IsInternalDispatcherContext)
            {
                var database = ConfigDbServicesHelper.GetDatabaseService().GetOneById(databaseId);
                result = AccountDbContext.CreateFromConnectionString(database.ConnectionString);
            }
            else
            {
                result = AccountDbContext.CreateFromDatabaseId(databaseId);
            }
            _accountDbContexts.Add(databaseId, result);
            return result;
        }

        public void SaveChanges()
        {
            foreach (var context in _accountDbContexts.Values)
            {
                context.SaveChanges();
            }
        }

        public void Dispose()
        {
            foreach (var context in _accountDbContexts.Values)
            {
                context.Dispose();
            }
        }

    }
}
