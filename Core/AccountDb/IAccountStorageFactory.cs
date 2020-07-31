using System;
using Zidium.Storage;

namespace Zidium.Core
{
    public interface IAccountStorageFactory
    {
        IStorage GetStorageByAccountId(Guid accountId);

        IStorage GetStorageByDatabaseId(Guid databaseId);
    }
}
