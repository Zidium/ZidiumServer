using Zidium.Common;
using Zidium.Core.ConfigDb;
using Zidium.Core;
using Zidium.Storage.Ef;
using Zidium.Storage;
using Zidium.TestTools;
using Zidium;

namespace ApiTests_1._0
{
    public abstract class BaseTest
    {
        static BaseTest()
        {
            // Тесты не должны накатывать миграции или создавать базы
            StorageFactory.DisableMigrations();

            var configuration = new Configuration();
            DependencyInjection.SetServicePersistent<IDebugConfiguration>(configuration);
            DependencyInjection.SetServicePersistent<IDatabaseConfiguration>(configuration);
            DependencyInjection.SetServicePersistent<IDispatcherConfiguration>(configuration);
            DependencyInjection.SetServicePersistent<ITestsConfiguration>(configuration);

            Initialization.SetServices();
            DependencyInjection.SetServicePersistent<IStorageFactory>(new StorageFactory());
            DependencyInjection.SetServicePersistent<IAccountStorageFactory>(new LocalAccountStorageFactory());
        }
    }
}
