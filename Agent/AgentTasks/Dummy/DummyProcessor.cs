using System;
using System.Threading;
using NLog;
using Zidium.Core.Common;

namespace Zidium.Agent.AgentTasks.Dummy
{
    public class DummyProcessor
    {
        public MultipleDataBaseProcessor DbProcessor { get; protected set; }

        public DummyProcessor(ILogger logger, CancellationToken cancellationToken)
        {
            DbProcessor = new MultipleDataBaseProcessor(logger, cancellationToken);
        }

        public void Process()
        {
            DbProcessor.ForEachAccount(data => ProcessAccount(data.Account.Id, data.Logger));
        }

        protected void ProcessAccount(Guid accountId, ILogger logger)
        {
            logger.Info($"Обработка аккаунта {accountId}");
        }
    }
}
