using System;
using System.Diagnostics;
using System.Threading;
using Microsoft.Extensions.Logging;
using Zidium.Core;
using Zidium.Core.Common.Helpers;

namespace Zidium.Agent.AgentTasks.DeleteMetricHistory
{
    public class DeleteMetricHistoryProcessor
    {
        protected ILogger Logger;

        protected CancellationToken CancellationToken;

        public const int MaxDeleteCount = 10000;

        public int DeletedMetricValueCount;

        public DeleteMetricHistoryProcessor(ILogger logger, CancellationToken cancellationToken)
        {
            Logger = logger;
            CancellationToken = cancellationToken;
        }

        public void Process()
        {
            DeletedMetricValueCount = 0;

            var stopWatch = Stopwatch.StartNew();

            var logicSettings = LogicSettingsCache.LogicSettings;
            var date = DateTimeHelper.TrimMs(DateTime.Now.AddDays(-logicSettings.MetricsMaxDays));

            var storage = DependencyInjection.GetServicePersistent<IDefaultStorageFactory>().GetStorage();
            var metricHistoryRepository = storage.MetricHistory;

            long deletedCount = 0;
            while (true)
            {
                CancellationToken.ThrowIfCancellationRequested();

                var innerStopWatch = Stopwatch.StartNew();

                var count = metricHistoryRepository.DeleteMetricsHistory(MaxDeleteCount, date);
                Interlocked.Add(ref DeletedMetricValueCount, count);

                if (count == 0)
                    break;

                innerStopWatch.Stop();
                Logger.LogDebug("Удален пакет из {0} значений метрик за {1}", count, TimeSpanHelper.Get2UnitsString(innerStopWatch.Elapsed));

                deletedCount += count;

                // чтобы не сильно нагружать SQL
                Thread.Sleep(1000);
            }

            stopWatch.Stop();

            if (deletedCount > 0)
                Logger.LogDebug($"Удалено значений метрик: {deletedCount} за {TimeSpanHelper.Get2UnitsString(stopWatch.Elapsed)}");

        }

    }
}
