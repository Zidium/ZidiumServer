using System.Threading;
using NLog;
using Zidium.Core.Api;
using Zidium.Storage;

namespace Zidium.Core.Common
{
    /// <summary>
    /// Данные для цикла по аккаунтам
    /// </summary>
    public class ForEachAccountData
    {
        public ILogger Logger { get; protected set; }

        public CancellationToken CancellationToken { get; protected set; }

        public AccountInfo Account { get; protected set; }

        public IStorage Storage { get; protected set; }

        public ForEachAccountData(
            ILogger logger,
            CancellationToken cancellationToken,
            AccountInfo account)
        {
            Logger = logger;
            CancellationToken = cancellationToken;
            Account = account;

            var storageFactory = DependencyInjection.GetServicePersistent<IStorageFactory>();
            Storage = storageFactory.GetStorage(account.DatabaseConnectionString);
        }

    }
}
