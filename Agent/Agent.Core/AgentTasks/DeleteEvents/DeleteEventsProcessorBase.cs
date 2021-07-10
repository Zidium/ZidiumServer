using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using NLog;
using Zidium.Api.Dto;
using Zidium.Core;
using Zidium.Core.Common.Helpers;
using Zidium.Storage;

namespace Zidium.Agent.AgentTasks.DeleteEvents
{
    public abstract class DeleteEventsProcessorBase
    {
        protected ILogger Logger;

        protected CancellationToken CancellationToken;

        public int MaxDeleteCount = 10000;

        public int DeletedEventsCount;

        public int DeletedPropertiesCount;

        protected object ReportLockObject = new object();

        protected List<string> Report = new List<string>();

        protected DeleteEventsProcessorBase(ILogger logger, CancellationToken cancellationToken)
        {
            Logger = logger;
            CancellationToken = cancellationToken;
        }

        public void Process()
        {
            DeletedEventsCount = 0;
            DeletedPropertiesCount = 0;

            lock (ReportLockObject)
            {
                Report.Clear();
            }

            var categories = GetCategories();

            var date = DateTimeHelper.TrimMs(DateTime.Now.AddDays(-GetMaxDays()));
            Logger.Trace("Максимальная дата актуальности: {0}", date);

            var storage = DependencyInjection.GetServicePersistent<IDefaultStorageFactory>().GetStorage();
            var eventRepository = storage.Events;
            var remaining = eventRepository.GetEventsCountForDeletion(categories, date);

            Int64 count = 0;
            var stopwatch = Stopwatch.StartNew();

            while (true)
            {
                CancellationToken.ThrowIfCancellationRequested();

                var innerStopWatch = new Stopwatch();
                innerStopWatch.Start();

                if (remaining > 0)
                    Logger.Debug("Осталось удалить событий: " + remaining);

                DeleteParameters(
                    Logger,
                    eventRepository,
                    categories,
                    date,
                    MaxDeleteCount);

                DeleteEventStatuses(
                    Logger,
                    eventRepository,
                    categories,
                    date,
                    MaxDeleteCount);

                UpdateMetricsHistory(
                    Logger,
                    eventRepository,
                    categories,
                    date,
                    MaxDeleteCount);

                DeleteNotifications(
                    Logger,
                    eventRepository,
                    categories,
                    date,
                    MaxDeleteCount);

                DeleteArchivedStatuses(
                    Logger,
                    eventRepository,
                    categories,
                    date,
                    MaxDeleteCount);

                var result = DeleteEvents(
                    Logger,
                    eventRepository,
                    categories,
                    date,
                    MaxDeleteCount);

                count += result;
                remaining -= result;

                innerStopWatch.Stop();
                if (result > 0)
                    Logger.Debug("Удален пакет из {0} событий за {1}", result, TimeSpanHelper.Get2UnitsString(innerStopWatch.Elapsed));

                if (result == 0)
                    break;

            }

            stopwatch.Stop();

            if (count > 0)
            {
                Logger.Debug($"Удалено событий: {count} за {TimeSpanHelper.Get2UnitsString(stopwatch.Elapsed)}");
                AddToReport(string.Format("Удалено {0} событий за {1}", count, TimeSpanHelper.Get2UnitsString(stopwatch.Elapsed)));
            }


            if (DeletedEventsCount > 0)
            {
                Logger.Info("Удалено событий, всего: " + DeletedEventsCount);
                lock (ReportLockObject)
                {
                    var reportChunks = Report
                        .Select((t, i) =>
                            new
                            {
                                Value = t,
                                Index = i
                            })
                        .GroupBy(t => t.Index / 20)
                        .Select(grp => grp.Select(x => x.Value).ToArray())
                        .ToArray();

                    foreach (var chunk in reportChunks)
                    {
                        Logger.Info(string.Join(Environment.NewLine, chunk));
                    }
                }
            }
        }

        protected void DeleteParameters(ILogger logger, IEventRepository repository, EventCategory[] categories, DateTime date, int maxCount)
        {
            CancellationToken.ThrowIfCancellationRequested();
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var count = repository.DeleteEventParameters(categories, maxCount, date);
            stopwatch.Stop();
            logger.Trace("Удалено строк свойств событий: {0} за {1}", count, TimeSpanHelper.Get2UnitsString(stopwatch.Elapsed));
            Interlocked.Add(ref DeletedPropertiesCount, count);
        }

        protected void DeleteEventStatuses(ILogger logger, IEventRepository repository, EventCategory[] categories, DateTime date, int maxCount)
        {
            CancellationToken.ThrowIfCancellationRequested();
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var count = repository.DeleteEventStatuses(categories, maxCount, date);
            stopwatch.Stop();
            logger.Trace("Удалено статусов событий: {0} за {1}", count, TimeSpanHelper.Get2UnitsString(stopwatch.Elapsed));
        }

        protected void UpdateMetricsHistory(ILogger logger, IEventRepository repository, EventCategory[] categories, DateTime date, int maxCount)
        {
            CancellationToken.ThrowIfCancellationRequested();
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var count = repository.UpdateMetricsHistory(categories, maxCount, date);
            stopwatch.Stop();
            logger.Trace("Обновлено строк истории метрик: {0} за {1}", count, TimeSpanHelper.Get2UnitsString(stopwatch.Elapsed));
        }

        protected void DeleteNotifications(ILogger logger, IEventRepository repository, EventCategory[] categories, DateTime date, int maxCount)
        {
            CancellationToken.ThrowIfCancellationRequested();
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var count = repository.DeleteNotifications(categories, maxCount, date);
            stopwatch.Stop();
            logger.Trace("Удалено строк уведомлений: {0} за {1}", count, TimeSpanHelper.Get2UnitsString(stopwatch.Elapsed));
        }

        protected void DeleteArchivedStatuses(ILogger logger, IEventRepository repository, EventCategory[] categories, DateTime date, int maxCount)
        {
            CancellationToken.ThrowIfCancellationRequested();
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var count = repository.DeleteEventArchivedStatuses(categories, maxCount, date);
            stopwatch.Stop();
            logger.Trace("Удалено архивных статусов: {0} за {1}", count, TimeSpanHelper.Get2UnitsString(stopwatch.Elapsed));
        }

        protected int DeleteEvents(ILogger logger, IEventRepository repository, EventCategory[] categories, DateTime date, int maxCount)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var count = repository.DeleteEvents(categories, maxCount, date);
            stopwatch.Stop();
            logger.Trace("Удалено строк событий: {0} за {1}", count, TimeSpanHelper.Get2UnitsString(stopwatch.Elapsed));
            Interlocked.Add(ref DeletedEventsCount, count);
            return count;
        }

        public abstract int GetMaxDays();

        public abstract EventCategory[] GetCategories();

        protected void AddToReport(string s)
        {
            lock (ReportLockObject)
            {
                Report.Add(s);
            }
        }
    }
}
