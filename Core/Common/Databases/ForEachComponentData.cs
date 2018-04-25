using System;
using System.Threading;
using NLog;
using Zidium.Core.AccountsDb;
using Zidium.Core.Api;

namespace Zidium.Core.Common
{
    /// <summary>
    /// Данные для обработки одного компонента в цикле
    /// </summary>
    public class ForEachComponentData : IDisposable
    {
        public ILogger Logger { get; protected set; }

        public CancellationToken CancellationToken { get; protected set; }

        public AccountInfo Account { get; protected set; }

        public AccountDbContext AccountDbContext { get; protected set; }

        public Component Component { get; protected set; }

        public ForEachComponentData(
            ILogger logger,
            CancellationToken cancellationToken,
            AccountInfo account, 
            Component component)
        {
            Logger = logger;
            CancellationToken = cancellationToken;
            Account = account;
            AccountDbContext = AccountDbContext.CreateFromDatabaseId(account.AccountDatabaseId);
            Component = component;
        }

        public void Dispose()
        {
            AccountDbContext.Dispose();
        }
    }
}
