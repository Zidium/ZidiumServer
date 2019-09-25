using System;
using System.Diagnostics;
using System.Text;
using System.Threading;
using NLog;
using Zidium.Core.AccountsDb;
using Zidium.Core.Api;
using Zidium.Core.Common;
using Zidium.Core.Common.Helpers;
using Zidium.Core.ConfigDb;

namespace Zidium.Agent.AgentTasks.DeleteEvents
{
    public abstract class DeleteEventsProcessorBase
    {
        protected ILogger Logger;

        public const int MaxDeleteCount = 1000;

        public int DeletedEventsCount;

        public int DeletedPropertiesCount;

        public MultipleDataBaseProcessor DbProcessor { get; protected set; }

        protected object ReportLockObject = new object();

        protected StringBuilder Report = new StringBuilder();

        protected DeleteEventsProcessorBase(ILogger logger, CancellationToken cancellationToken)
        {
            Logger = logger;
            DbProcessor = new MultipleDataBaseProcessor(logger, cancellationToken)
            {
                AccountThreads = 1,
                ComponentsThreads = 1,
                DataBaseThreads = 1
            };
        }

        public void Process(Guid? accountId = null)
        {
            DeletedEventsCount = 0;
            DeletedPropertiesCount = 0;

            lock (ReportLockObject)
            {
                Report.Clear();
            }

            DbProcessor.ForEachAccount(data =>
            {
                if ((!accountId.HasValue && data.Account.Type != AccountType.Test) || (accountId.HasValue && accountId.Value == data.Account.Id))
                    ProcessAccount(data);
            });

            if (DeletedEventsCount > 0)
            {
                Logger.Info("Удалено событий, всего: " + DeletedEventsCount);
                lock (ReportLockObject)
                {
                    Logger.Info(Report.ToString());
                }
            }
        }

        protected void DeleteParameters(ILogger logger, IEventRepository repository, Guid accountId, EventCategory[] categories, DateTime date, int maxCount)
        {
            DbProcessor.CancellationToken.ThrowIfCancellationRequested();
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var count = repository.DeleteEventParameters(categories, maxCount, date);
            stopwatch.Stop();
            logger.Trace("Удалено строк свойств событий: {0} за {1}", count, TimeSpanHelper.Get2UnitsString(stopwatch.Elapsed));
            Interlocked.Add(ref DeletedPropertiesCount, count);
        }

        protected void DeleteEventStatuses(ILogger logger, IEventRepository repository, Guid accountId, EventCategory[] categories, DateTime date, int maxCount)
        {
            DbProcessor.CancellationToken.ThrowIfCancellationRequested();
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var count = repository.DeleteEventStatuses(categories, maxCount, date);
            stopwatch.Stop();
            logger.Trace("Удалено статусов событий: {0} за {1}", count, TimeSpanHelper.Get2UnitsString(stopwatch.Elapsed));
        }

        protected void UpdateMetricsHistory(ILogger logger, IEventRepository repository, Guid accountId, EventCategory[] categories, DateTime date, int maxCount)
        {
            DbProcessor.CancellationToken.ThrowIfCancellationRequested();
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var count = repository.UpdateMetricsHistory(categories, maxCount, date);
            stopwatch.Stop();
            logger.Trace("Обновлено строк истории метрик: {0} за {1}", count, TimeSpanHelper.Get2UnitsString(stopwatch.Elapsed));
        }

        protected void DeleteNotifications(ILogger logger, IEventRepository repository, Guid accountId, EventCategory[] categories, DateTime date, int maxCount)
        {
            DbProcessor.CancellationToken.ThrowIfCancellationRequested();
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var count = repository.DeleteNotifications(categories, maxCount, date);
            stopwatch.Stop();
            logger.Trace("Удалено строк уведомлений: {0} за {1}", count, TimeSpanHelper.Get2UnitsString(stopwatch.Elapsed));
        }

        protected void DeleteArchivedStatuses(ILogger logger, IEventRepository repository, Guid accountId, EventCategory[] categories, DateTime date, int maxCount)
        {
            DbProcessor.CancellationToken.ThrowIfCancellationRequested();
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var count = repository.DeleteEventArchivedStatuses(categories, maxCount, date);
            stopwatch.Stop();
            logger.Trace("Удалено архивных статусов: {0} за {1}", count, TimeSpanHelper.Get2UnitsString(stopwatch.Elapsed));
        }

        protected int DeleteEvents(ILogger logger, IEventRepository repository, Guid accountId, EventCategory[] categories, DateTime date, int maxCount)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var count = repository.DeleteEvents(categories, maxCount, date);
            stopwatch.Stop();
            logger.Trace("Удалено строк событий: {0} за {1}", count, TimeSpanHelper.Get2UnitsString(stopwatch.Elapsed));
            Interlocked.Add(ref DeletedEventsCount, count);
            return count;
        }

        public abstract int GetMaxDaysFromTariffLimit(TariffLimit tariffLimit);

        public abstract EventCategory[] GetCategories();

        public void ProcessAccount(ForEachAccountData data)
        {
            data.AccountDbContext.Database.CommandTimeout = 0;
            var categories = GetCategories();

            var accountTariffRepository = data.AccountDbContext.GetAccountTariffRepository();
            var tariffLimit = accountTariffRepository.GetHardTariffLimit();
            var date = DateTimeHelper.TrimMs(DateTime.Now.AddDays(-GetMaxDaysFromTariffLimit(tariffLimit)));
            data.Logger.Trace("Аккаунт: {0}, Максимальная дата актуальности: {1}", data.Account.SystemName, date);

            var eventRepository = data.AccountDbContext.GetEventRepository();
            var remaining = eventRepository.GetEventsCountForDeletion(categories, date);

            Int64 count = 0;
            var stopwatch = Stopwatch.StartNew();

            while (true)
            {
                DbProcessor.CancellationToken.ThrowIfCancellationRequested();

                var innerStopWatch = new Stopwatch();
                innerStopWatch.Start();

                if (remaining > 0)
                    data.Logger.Debug("Осталось удалить событий: " + remaining);

                DeleteParameters(
                    data.Logger,
                    eventRepository,
                    data.Account.Id,
                    categories,
                    date,
                    MaxDeleteCount);

                DeleteEventStatuses(
                    data.Logger,
                    eventRepository,
                    data.Account.Id,
                    categories,
                    date,
                    MaxDeleteCount);

                UpdateMetricsHistory(
                    data.Logger,
                    eventRepository,
                    data.Account.Id,
                    categories,
                    date,
                    MaxDeleteCount);

                DeleteNotifications(
                    data.Logger,
                    eventRepository,
                    data.Account.Id,
                    categories,
                    date,
                    MaxDeleteCount);

                DeleteArchivedStatuses(
                    data.Logger,
                    eventRepository,
                    data.Account.Id,
                    categories,
                    date,
                    MaxDeleteCount);

                var result = DeleteEvents(
                    data.Logger,
                    eventRepository,
                    data.Account.Id,
                    categories,
                    date,
                    MaxDeleteCount);

                count += result;
                remaining -= result;

                innerStopWatch.Stop();
                if (result > 0)
                    data.Logger.Debug("Удален пакет из {0} событий за {1}", result, TimeSpanHelper.Get2UnitsString(innerStopWatch.Elapsed));

                if (result == 0)
                    break;

                // чтобы не сильно нагружать SQL
                Thread.Sleep(1000);
            }

            stopwatch.Stop();

            if (count > 0)
            {
                data.Logger.Debug($"Удалено событий: {count} в аккаунте {data.Account.SystemName} за {TimeSpanHelper.Get2UnitsString(stopwatch.Elapsed)}");
                AddToReport(string.Format("Удалено {0} событий в аккаунте {1} за {2}", count, data.Account.SystemName, TimeSpanHelper.Get2UnitsString(stopwatch.Elapsed)));
            }

        }

        protected void AddToReport(string s)
        {
            lock (ReportLockObject)
            {
                Report.AppendLine(s);
            }
        }
    }
}
