using System;
using System.Threading;
using NLog;
using Zidium.Core.Common;
using Zidium.Core.Common.Helpers;
using Zidium.Core.ConfigDb;

namespace Zidium.Agent.AgentTasks.DeleteMetricHistory
{
    public class DeleteMetricHistoryProcessor
    {
        protected ILogger Logger;

        public const int MaxDeleteCount = 10000;

        public int DeletedMetricValueCount;

        public MultipleDataBaseProcessor DbProcessor { get; protected set; }

        public DeleteMetricHistoryProcessor(ILogger logger, CancellationToken cancellationToken)
        {
            Logger = logger;
            DbProcessor = new MultipleDataBaseProcessor(logger, cancellationToken)
            {
                AccountThreads = 1,
                ComponentsThreads = 1,
                DataBaseThreads = 1,
                StorageThreads = 1
            };
        }

        public void Process(Guid? accountId = null)
        {
            DeletedMetricValueCount = 0;

            DbProcessor.ForEachAccount(data =>
            {
                if ((!accountId.HasValue && data.Account.Type != AccountType.Test) || (accountId.HasValue && accountId.Value == data.Account.Id))
                    ProcessAccount(data);
            });

            if (DeletedMetricValueCount > 0)
                Logger.Info("Удалено значений метрик: " + DeletedMetricValueCount);
        }

        public void ProcessAccount(ForEachAccountData data)
        {
            var accountTariffRepository = data.AccountDbContext.GetAccountTariffRepository();
            var tariffLimit = accountTariffRepository.GetHardTariffLimit();
            var date = DateTimeHelper.TrimMs(DateTime.Now.AddDays(-tariffLimit.MetricsMaxDays));

            var metricHistoryRepository = data.AccountDbContext.GetMetricHistoryRepository();

            long deletedCount = 0;
            while (true)
            {
                DbProcessor.CancellationToken.ThrowIfCancellationRequested();

                var count = metricHistoryRepository.DeleteMetricsHistory(MaxDeleteCount, date);
                data.Logger.Trace("Удалено строк истории метрик: {0}", count);
                Interlocked.Add(ref DeletedMetricValueCount, count);

                if (count == 0)
                    break;

                deletedCount += count;
            }

            if (deletedCount > 0)
                data.Logger.Debug("Удалено значений метрик: " + deletedCount);

        }
    }
}
