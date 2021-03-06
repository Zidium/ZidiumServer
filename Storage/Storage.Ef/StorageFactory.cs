﻿namespace Zidium.Storage.Ef
{
    public class StorageFactory : IStorageFactory
    {
        public IStorage GetStorage(string connectionString)
        {
            return new Storage(connectionString);
        }

        public static void DisableMigrations()
        {
            AccountDbContext.DisableMigrations();
        }

    }
}
