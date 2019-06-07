using System;
using System.Diagnostics;
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
            data.AccountDbContext.Database.CommandTimeout = 0;
            var stopWatch = Stopwatch.StartNew();

            var accountTariffRepository = data.AccountDbContext.GetAccountTariffRepository();
            var tariffLimit = accountTariffRepository.GetHardTariffLimit();
            var date = DateTimeHelper.TrimMs(DateTime.Now.AddDays(-tariffLimit.MetricsMaxDays));

            var metricHistoryRepository = data.AccountDbContext.GetMetricHistoryRepository();

            long deletedCount = 0;
            while (true)
            {
                DbProcessor.CancellationToken.ThrowIfCancellationRequested();

                var innerStopWatch = Stopwatch.StartNew();

                var count = metricHistoryRepository.DeleteMetricsHistory(MaxDeleteCount, date);
                data.Logger.Trace("Удалено строк истории метрик: {0}", count);
                Interlocked.Add(ref DeletedMetricValueCount, count);

                if (count == 0)
                    break;

                innerStopWatch.Stop();
                data.Logger.Debug("Удален пакет из {0} значений метрик за {1}", count, TimeSpanHelper.Get2UnitsString(innerStopWatch.Elapsed));

                deletedCount += count;

                // чтобы не сильно нагружать SQL
                Thread.Sleep(1000);
            }

            stopWatch.Stop();

            if (deletedCount > 0)
                data.Logger.Debug($"Удалено значений метрик: {deletedCount} в аккаунте {data.Account.SystemName} за {TimeSpanHelper.Get2UnitsString(stopWatch.Elapsed)}");

        }
    }
}
