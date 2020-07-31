using System;
using Zidium.Core.ConfigDb;
using Zidium.Storage;

namespace Zidium.Core
{
    public class LocalAccountStorageFactory : IAccountStorageFactory
    {
        public LocalAccountStorageFactory()
        {
            _configDbServicesFactory = DependencyInjection.GetServicePersistent<IConfigDbServicesFactory>();
            _storageFactory = DependencyInjection.GetServicePersistent<IStorageFactory>();
        }

        private readonly IConfigDbServicesFactory _configDbServicesFactory;
        private readonly IStorageFactory _storageFactory;

        public IStorage GetStorageByAccountId(Guid accountId)
        {
            var account = _configDbServicesFactory.GetAccountService().GetOneById(accountId);
            var database = _configDbServicesFactory.GetDatabaseService().GetOneById(account.AccountDatabaseId);
            return _storageFactory.GetStorage(database.ConnectionString);
        }

        public IStorage GetStorageByDatabaseId(Guid databaseId)
        {
            var database = _configDbServicesFactory.GetDatabaseService().GetOneById(databaseId);
            return _storageFactory.GetStorage(database.ConnectionString);
        }
    }
}
