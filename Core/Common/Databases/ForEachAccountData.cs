using System;
using System.Threading;
using NLog;
using Zidium.Core.AccountsDb;
using Zidium.Core.Api;

namespace Zidium.Core.Common
{
    /// <summary>
    /// Данные для цикла по аккаунтам
    /// </summary>
    public class ForEachAccountData : IDisposable
    {
        public ILogger Logger { get; protected set; }

        public CancellationToken CancellationToken { get; protected set; }

        public AccountInfo Account { get; protected set; }

        public AccountDbContext AccountDbContext { get; protected set; }

        public ForEachAccountData(
            ILogger logger,
            CancellationToken cancellationToken,
            AccountInfo account)
        {
            Logger = logger;
            CancellationToken = cancellationToken;
            AccountDbContext = AccountDbContext.CreateFromDatabaseId(account.AccountDatabaseId);
            Account = account;
        }

        public void Dispose()
        {
            AccountDbContext.Dispose();
        }
    }
}
