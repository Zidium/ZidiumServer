using System;
using System.Threading;
using NLog;
using Zidium.Api;
using Zidium.Core;
using Zidium.Core.Common;
using Zidium.Core.Common.Helpers;

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
                ComponentsThreads = 1,
                StorageThreads = 1
            };
        }

        public void Process(DateTime? date = null, Guid? accountId = null, Guid? componentId = null)
        {
            DeletedLogsCount = 0;
            DeletedPropertiesCount = 0;
            
            if (!accountId.HasValue)
                DbProcessor.ForEachComponent(data => ProcessComponent(date, data), true);
            else
                DbProcessor.ForEachAccountComponents(accountId.Value, data =>
                {
                    if (!componentId.HasValue || componentId.Value == data.Component.Id)
                        ProcessComponent(date, data);
                }, true);

            if (DeletedLogsCount > 0)
                Logger.Info("Удалено записей лога: " + DeletedLogsCount);
        }

        protected void DeleteProperties(ILogger logger, ILogRepository repository, Guid componentId, DateTime date, int maxCount)
        {
            while (true)
            {
                DbProcessor.CancellationToken.ThrowIfCancellationRequested();
                var count = repository.DeleteLogProperties(componentId, maxCount, date);
                logger.Trace("Удалено строк свойств лога: {0}", count);
                DeletedPropertiesCount += count;
                if (count == 0)
                    break;
            }
        }

        protected int DeleteLogs(ILogger logger, ILogRepository repository, Guid componentId, DateTime date, int maxCount)
        {
            var count = repository.DeleteLogs(componentId, maxCount, date);
            logger.Trace("Удалено строк лога: {0}", count);
            Interlocked.Add(ref DeletedLogsCount, count);

            return count;
        }

        public void ProcessComponent(DateTime? date, ForEachComponentData data)
        {
            var accountTariffRepository = data.AccountDbContext.GetAccountTariffRepository();
            var tarifLimit = accountTariffRepository.GetHardTariffLimit();
            var date2 = date ?? DateTimeHelper.TrimMs(DateTime.Now.AddDays(-tarifLimit.LogMaxDays));

            var logRepository = data.AccountDbContext.GetLogRepository();

            long count = 0;
            while (true)
            {
                DbProcessor.CancellationToken.ThrowIfCancellationRequested();

                DeleteProperties(
                    data.Logger,
                    logRepository,
                    data.Component.Id,
                    date2,
                    MaxDeleteCount);

                var result = DeleteLogs(
                    data.Logger,
                    logRepository,
                    data.Component.Id,
                    date2,
                    MaxDeleteCount);

                count += result;

                if (result == 0)
                    break;
            }

            if (count > 0)
                data.Logger.Debug("Удалено логов: " + count + " по компоненту " + data.Component.Id);
        }
    }
}
