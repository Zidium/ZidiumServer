using System;
using System.Threading;
using NLog;
using Zidium.Core.Common;
using Zidium.Core.Common.Helpers;

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

        public void Process(Guid? accountId = null, Guid? componentId = null)
        {
            DeletedMetricValueCount = 0;

            if (!accountId.HasValue)
                DbProcessor.ForEachComponent(data => ProcessComponent(data), true);
            else
                DbProcessor.ForEachAccountComponents(accountId.Value, data =>
                {
                    if (!componentId.HasValue || componentId.Value == data.Component.Id)
                        ProcessComponent(data);
                }, true);

            if (DeletedMetricValueCount > 0)
                Logger.Info("Удалено значений метрик: " + DeletedMetricValueCount);
        }

        public void ProcessComponent(ForEachComponentData data)
        {
            var accountTariffRepository = data.AccountDbContext.GetAccountTariffRepository();
            var tariffLimit = accountTariffRepository.GetHardTariffLimit();
            var date = DateTimeHelper.TrimMs(DateTime.Now.AddDays(-tariffLimit.MetricsMaxDays));

            var metricHistoryRepository = data.AccountDbContext.GetMetricHistoryRepository();

            long deletedCount = 0;
            while (true)
            {
                DbProcessor.CancellationToken.ThrowIfCancellationRequested();

                var count = metricHistoryRepository.DeleteMetricsHistory(data.Component.Id, MaxDeleteCount, date);
                data.Logger.Trace("Удалено значений метрик: {0}", count);
                Interlocked.Add(ref DeletedMetricValueCount, count);

                if (count == 0)
                    break;

                deletedCount += count;
            }

            if (deletedCount > 0)
                data.Logger.Debug("Удалено значений метрик: " + deletedCount + " по компоненту " + data.Component.Id);

        }
    }
}
