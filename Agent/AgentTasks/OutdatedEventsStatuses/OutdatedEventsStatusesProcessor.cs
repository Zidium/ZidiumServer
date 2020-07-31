using System.Threading;
using NLog;
using Zidium.Core.Common;
using Zidium.Core.ConfigDb;

namespace Zidium.Agent.AgentTasks.OutdatedEventsStatuses
{
    public class OutdatedEventsStatusesProcessor
    {
        protected ILogger Logger;

        public MultipleDataBaseProcessor DbProcessor { get; protected set; }

        public int Count;

        public OutdatedEventsStatusesProcessor(ILogger logger, CancellationToken cancellationToken)
        {
            Logger = logger;
            DbProcessor = new MultipleDataBaseProcessor(logger, cancellationToken);
        }

        public void Process()
        {
            Count = 0;
            DbProcessor.ForEachAccount(data =>
            {
                if (data.Account.Type != AccountType.Test)
                    ProcessAccount(data);
            });
            if (Count > 0)
                Logger.Info("Обновлено {0} статусов событий", Count);
        }

        public void ProcessAccount(ForEachAccountData data)
        {
            var dispatcher = AgentHelper.GetDispatcherClient();
            while (true)
            {
                data.CancellationToken.ThrowIfCancellationRequested();

                const int maxCount = 100;
                var response = dispatcher.UpdateEventsStatuses(data.Account.Id, maxCount);
                if (response.Success)
                {
                    var updateCount = response.Data.UpdateCount;
                    Interlocked.Add(ref Count, updateCount);
                    if (updateCount == 0)
                    {
                        if (data.Logger.IsTraceEnabled)
                            data.Logger.Trace("Обновлено статусов событий: " + updateCount);
                        return;
                    }
                    if (data.Logger.IsDebugEnabled)
                        data.Logger.Debug("Обновлено статусов событий: " + updateCount);
                    if (updateCount < maxCount)
                    {
                        return;
                    }
                }
                else
                {
                    data.Logger.Error("Ошибка: " + response.ErrorMessage);
                    return;
                }
            }
        }

    }
}
