using Zidium.Common;
using Zidium.Core.ConfigDb;
using Zidium.Storage;
using Zidium.Storage.Ef;
using Zidium.TestTools;

namespace Zidium.Core.Tests
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
            DependencyInjection.SetServicePersistent<ICoreTestsConfiguration>(configuration);

            Initialization.SetServices();
            DependencyInjection.SetServicePersistent<IStorageFactory>(new StorageFactory());
            DependencyInjection.SetServicePersistent<IAccountStorageFactory>(new LocalAccountStorageFactory());
        }
    }
}
