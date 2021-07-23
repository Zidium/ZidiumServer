using System;
using System.Diagnostics;
using System.Threading;
using Microsoft.Extensions.Logging;
using Zidium.Core;
using Zidium.Core.Common.Helpers;
using Zidium.Storage;

namespace Zidium.Agent.AgentTasks.DeleteLogs
{
    public class DeleteLogsProcessor
    {
        protected ILogger Logger;

        protected CancellationToken CancellationToken;

        public const int MaxDeleteCount = 10000;

        public int DeletedLogsCount;

        public int DeletedPropertiesCount { get; protected set; }

        public DeleteLogsProcessor(ILogger logger, CancellationToken cancellationToken)
        {
            Logger = logger;
            CancellationToken = cancellationToken;
        }

        public void Process()
        {
            var stopWatch = Stopwatch.StartNew();

            DeletedLogsCount = 0;
            DeletedPropertiesCount = 0;

            var logicSettings = LogicSettingsCache.LogicSettings;
            var date = DateTimeHelper.TrimMs(DateTime.Now.AddDays(-logicSettings.LogMaxDays));

            var storage = DependencyInjection.GetServicePersistent<IDefaultStorageFactory>().GetStorage();
            var logRepository = storage.Logs;

            long count = 0;
            while (true)
            {
                CancellationToken.ThrowIfCancellationRequested();

                var innerStopWatch = Stopwatch.StartNew();

                DeleteProperties(
                    Logger,
                    logRepository,
                    date,
                    MaxDeleteCount);

                var result = DeleteLogs(
                    Logger,
                    logRepository,
                    date,
                    MaxDeleteCount);

                innerStopWatch.Stop();
                if (result > 0)
                    Logger.LogDebug("Удален пакет из {0} записей лога за {1}", result, TimeSpanHelper.Get2UnitsString(innerStopWatch.Elapsed));

                count += result;

                if (result == 0)
                    break;
            }

            stopWatch.Stop();

            if (count > 0)
                Logger.LogInformation($"Удалено {count} логов за {TimeSpanHelper.Get2UnitsString(stopWatch.Elapsed)}");
            else
                Logger.LogDebug("Нет логов для удаления");
        }

        protected void DeleteProperties(ILogger logger, ILogRepository repository, DateTime date, int maxCount)
        {
            while (true)
            {
                CancellationToken.ThrowIfCancellationRequested();
                var count = repository.DeleteLogProperties(maxCount, date);
                logger.LogTrace("Удалено строк свойств лога: {0}", count);
                DeletedPropertiesCount += count;
                if (count == 0)
                    break;
            }
        }

        protected int DeleteLogs(ILogger logger, ILogRepository repository, DateTime date, int maxCount)
        {
            var count = repository.DeleteLogs(maxCount, date);
            logger.LogTrace("Удалено строк лога: {0}", count);
            Interlocked.Add(ref DeletedLogsCount, count);

            return count;
        }

    }

}
