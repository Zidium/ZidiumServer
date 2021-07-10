using Zidium.Common;
using Zidium.Storage;

namespace Zidium.Core
{
    public class DefaultStorageFactory : IDefaultStorageFactory
    {
        public IStorage GetStorage()
        {
            var configuration = DependencyInjection.GetServicePersistent<IDatabaseConfiguration>();
            var storageFactory = DependencyInjection.GetServicePersistent<IStorageFactory>();
            return storageFactory.GetStorage(configuration.ConnectionString);
        }
    }
}
