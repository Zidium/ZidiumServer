using System;
using System.Diagnostics;
using System.Threading;
using NLog;
using Zidium.Core;
using Zidium.Core.AccountsDb;
using Zidium.Core.Common;
using Zidium.Core.Common.Helpers;
using Zidium.Core.ConfigDb;

namespace Zidium.Agent.AgentTasks.DeleteLogs
{
    public class DeleteLogsProcessor
    {
        protected ILogger Logger;

        public const int MaxDeleteCount = 10000;

        public int DeletedLogsCount;

        public int DeletedPropertiesCount { get; protected set; }

        public MultipleDataBaseProcessor DbProcessor { get; protected set; }

        public DeleteLogsProcessor(ILogger logger, CancellationToken cancellationToken)
        {
            Logger = logger;
            DbProcessor = new MultipleDataBaseProcessor(logger, cancellationToken)
            {
                AccountThreads = 1,
                DataBaseThreads = 1,
                ComponentsThreads = 1
            };
        }

        public void Process(Guid? accountId = null)
        {
            var stopWatch = Stopwatch.StartNew();

            DeletedLogsCount = 0;
            DeletedPropertiesCount = 0;

            DbProcessor.ForEachAccount(data =>
            {
                if ((!accountId.HasValue && data.Account.Type != AccountType.Test) || (accountId.HasValue && accountId.Value == data.Account.Id))
                    ProcessAccount(data);
            });

            stopWatch.Stop();

            if (DeletedLogsCount > 0)
                Logger.Info($"Удалено записей лога во всех аккаунтах: {DeletedLogsCount} за {TimeSpanHelper.Get2UnitsString(stopWatch.Elapsed)}");
        }

        protected void DeleteProperties(ILogger logger, ILogRepository repository, Guid accountId, DateTime date, int maxCount)
        {
            while (true)
            {
                DbProcessor.CancellationToken.ThrowIfCancellationRequested();
                var count = repository.DeleteLogProperties(maxCount, date);
                logger.Trace("Удалено строк свойств лога: {0}", count);
                DeletedPropertiesCount += count;
                if (count == 0)
                    break;
            }
        }

        protected int DeleteLogs(ILogger logger, ILogRepository repository, Guid componentId, DateTime date, int maxCount)
        {
            var count = repository.DeleteLogs(maxCount, date);
            logger.Trace("Удалено строк лога: {0}", count);
            Interlocked.Add(ref DeletedLogsCount, count);

            return count;
        }

        public void ProcessAccount(ForEachAccountData data)
        {
            data.AccountDbContext.Database.CommandTimeout = 0;
            var stopWatch = Stopwatch.StartNew();

            var accountTariffRepository = data.AccountDbContext.GetAccountTariffRepository();
            var tarifLimit = accountTariffRepository.GetHardTariffLimit();
            var date = DateTimeHelper.TrimMs(DateTime.Now.AddDays(-tarifLimit.LogMaxDays));

            var logRepository = data.AccountDbContext.GetLogRepository();

            long count = 0;
            while (true)
            {
                DbProcessor.CancellationToken.ThrowIfCancellationRequested();

                var innerStopWatch = Stopwatch.StartNew();

                DeleteProperties(
                    data.Logger,
                    logRepository,
                    data.Account.Id,
                    date,
                    MaxDeleteCount);

                var result = DeleteLogs(
                    data.Logger,
                    logRepository,
                    data.Account.Id,
                    date,
                    MaxDeleteCount);

                innerStopWatch.Stop();
                if (result > 0)
                    data.Logger.Debug("Удален пакет из {0} записей лога за {1}", result, TimeSpanHelper.Get2UnitsString(innerStopWatch.Elapsed));

                count += result;

                if (result == 0)
                    break;

                // чтобы не сильно нагружать SQL
                Thread.Sleep(1000);
            }

            stopWatch.Stop();

            if (count > 0)
                data.Logger.Debug($"Удалено логов: {count} в аккаунте {data.Account.SystemName} за {TimeSpanHelper.Get2UnitsString(stopWatch.Elapsed)}");
        }
    }
}
