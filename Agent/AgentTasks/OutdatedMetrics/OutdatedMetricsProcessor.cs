using System;
using System.Threading;
using NLog;
using Zidium.Core.Common;

namespace Zidium.Agent.AgentTasks.OutdatedMetrics
{
    public class OutdatedMetricsProcessor
    {
        protected ILogger Logger;

        public MultipleDataBaseProcessor DbProcessor { get; protected set; }

        public int Count;

        public OutdatedMetricsProcessor(ILogger logger, CancellationToken cancellationToken)
        {
            Logger = logger;
            DbProcessor = new MultipleDataBaseProcessor(logger, cancellationToken);
        }

        public void Process()
        {
            Count = 0;
            DbProcessor.ForEachAccount(ProcessAccount);
            if (Count > 0)
                Logger.Info("Обновлено {0} статусов метрик", Count);
        }

        public void ProcessAccount(Guid accountId)
        {
            Count = 0;
            DbProcessor.ForEachAccount(data =>
            {
                if (data.Account.Id == accountId)
                {
                    ProcessAccount(data);
                }
            });
        }

        protected void ProcessAccount(ForEachAccountData data)
        {
            var dispatcher = AgentHelper.GetDispatcherClient();
            while (true)
            {
                data.CancellationToken.ThrowIfCancellationRequested();

                const int maxCount = 100;
                var response = dispatcher.CalculateMetrics(data.Account.Id, maxCount);
                if (response.Success)
                {
                    var updateCount = response.Data.UpdateCount;
                    Interlocked.Add(ref Count, updateCount);
                    if (updateCount == 0)
                    {
                        data.Logger.Trace("Обновлено статусов метрик: " + updateCount);
                        return;
                    }
                    data.Logger.Debug("Обновлено статусов метрик: " + updateCount);
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
