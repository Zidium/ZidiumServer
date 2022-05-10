using Microsoft.Extensions.Configuration;
using Zidium.Common;
using Zidium.Core;
using Zidium.Core.Common;
using Zidium.Core.InternalLogger;
using Zidium.Storage;
using Zidium.Storage.Ef;

namespace Zidium.Agent.Single.Tests
{
    public abstract class BaseTest
    {
        static BaseTest()
        {
            var appConfiguration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", false, false)
                .AddUserSecrets(typeof(BaseTest).Assembly, true)
                .Build();

            var configuration = new Configuration(appConfiguration);
            DependencyInjection.SetServicePersistent<IDebugConfiguration>(configuration);
            DependencyInjection.SetServicePersistent<IDatabaseConfiguration>(configuration);
            DependencyInjection.SetServicePersistent<IDispatcherConfiguration>(configuration);
            DependencyInjection.SetServicePersistent<IAccessConfiguration>(configuration);
            DependencyInjection.SetServicePersistent<ILogicConfiguration>(configuration);
            DependencyInjection.SetServicePersistent<IStorageFactory>(new StorageFactory());
            DependencyInjection.SetServicePersistent<InternalLoggerComponentMapping>(new InternalLoggerComponentMapping(null));
            DependencyInjection.SetServicePersistent<ITimeService>(new TimeService());
        }
    }
}
