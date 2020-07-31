using System;
using Zidium.Core.Api;
using Zidium.Storage;

namespace Zidium.Core
{
    public class RemoteAccountStorageFactory : IAccountStorageFactory
    {
        public RemoteAccountStorageFactory()
        {
            _storageFactory = DependencyInjection.GetServicePersistent<IStorageFactory>();
        }

        private readonly IStorageFactory _storageFactory;

        public IStorage GetStorageByAccountId(Guid accountId)
        {
            var client = DispatcherHelper.GetDispatcherClient();
            var account = client.GetAccountById(new GetAccountByIdRequestData() { Id = accountId }).Data;
            var database = client.GetDatabaseById(new GetDatabaseByIdRequestData() { Id = account.AccountDatabaseId }).Data;
            return _storageFactory.GetStorage(database.ConnectionString);
        }

        public IStorage GetStorageByDatabaseId(Guid databaseId)
        {
            var client = DispatcherHelper.GetDispatcherClient();
            var database = client.GetDatabaseById(new GetDatabaseByIdRequestData() { Id = databaseId }).Data;
            return _storageFactory.GetStorage(database.ConnectionString);
        }
    }
}
