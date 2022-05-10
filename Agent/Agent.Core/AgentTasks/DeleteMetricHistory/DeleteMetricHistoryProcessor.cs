using System;
using System.Diagnostics;
using System.Threading;
using Microsoft.Extensions.Logging;
using Zidium.Core.Common;
using Zidium.Core.Common.Helpers;
using Zidium.Storage;

namespace Zidium.Agent.AgentTasks.DeleteMetricHistory
{
    public class DeleteMetricHistoryProcessor
    {
        protected ILogger Logger;

        protected CancellationToken CancellationToken;

        public const int MaxDeleteCount = 10000;

        public int DeletedMetricValueCount;

        protected readonly ITimeService TimeService;

        public DeleteMetricHistoryProcessor(ILogger logger, CancellationToken cancellationToken)
        {
            Logger = logger;
            CancellationToken = cancellationToken;
            TimeService = DependencyInjection.GetServicePersistent<ITimeService>();
        }

        public void Process()
        {
            DeletedMetricValueCount = 0;

            var stopWatch = Stopwatch.StartNew();

            var logicSettings = LogicSettingsCache.LogicSettings;
            var date = DateTimeHelper.TrimMs(TimeService.Now().AddDays(-logicSettings.MetricsMaxDays));

            var storage = DependencyInjection.GetServicePersistent<IStorageFactory>().GetStorage();
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
            }

            stopWatch.Stop();

            if (deletedCount > 0)
                Logger.LogInformation($"Удалено {deletedCount} значений метрик за {TimeSpanHelper.Get2UnitsString(stopWatch.Elapsed)}");
            else
                Logger.LogDebug("Нет значений метрик для удаления");
        }

    }
}
