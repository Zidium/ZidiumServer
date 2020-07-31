using System.Threading;
using NLog;
using Zidium.Core.Api;
using Zidium.Storage;

namespace Zidium.Core.Common
{
    /// <summary>
    /// Данные для обработки одного компонента в цикле
    /// </summary>
    public class ForEachComponentData
    {
        public ILogger Logger { get; protected set; }

        public CancellationToken CancellationToken { get; protected set; }

        public AccountInfo Account { get; protected set; }

        public IStorage Storage { get; protected set; }

        public ComponentForRead Component { get; protected set; }

        public ForEachComponentData(
            ILogger logger,
            CancellationToken cancellationToken,
            AccountInfo account, 
            ComponentForRead component)
        {
            Logger = logger;
            CancellationToken = cancellationToken;
            Account = account;
            var accountStorageFactory = DependencyInjection.GetServicePersistent<IAccountStorageFactory>();
            Storage = accountStorageFactory.GetStorageByDatabaseId(account.AccountDatabaseId);
            Component = component;
        }

    }
}
