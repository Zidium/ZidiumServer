using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Zidium.Api;
using Zidium.Api.Dto;
using Zidium.Core;
using Zidium.Core.Api;
using Zidium.Core.Caching;
using Zidium.Storage;

namespace Zidium.Dispatcher
{
    public class DispatcherWebHandlerMiddleware : WebHandlerMiddlewareBase
    {
        public DispatcherWebHandlerMiddleware(RequestDelegate next, ILogger<DispatcherWebHandlerMiddleware> logger) : base(logger)
        {
        }

        public async Task Invoke(HttpContext httpContext)
        {
            var timer = new Stopwatch();
            timer.Start();

            var action = await InvokeInternal(httpContext);

            timer.Stop();

            // Считаем статистику только по корректным названиям action
            if (!string.IsNullOrEmpty(action))
            {
                UpdateCounters((long)timer.Elapsed.TotalMilliseconds, action);
            }
        }

        private static IComponentControl ComponentControl { get; set; }

        public static void Init(IComponentControl componentControl)
        {
            if (componentControl == null)
            {
                throw new ArgumentNullException("componentControl");
            }
            ComponentControl = componentControl;

            _saveCountersTimer = new Timer(SaveCounters, _saveCountersTimer, saveCountersTimerInterval, Timeout.InfiniteTimeSpan);
        }


        private static IComponentControl _actionStatsFolder;

        private static IComponentControl ActionStatsFolder
        {
            get
            {
                if (_actionStatsFolder == null)
                {
                    _actionStatsFolder = ComponentControl.GetOrCreateChildFolderControl("Requests");
                }
                return _actionStatsFolder;
            }
        }

        protected override int GetErrorCode()
        {
            return ResponseCode.ServerError;
        }

        public static string GetVersion()
        {
            return typeof(DispatcherWebHandlerMiddleware).Assembly.GetName().Version.ToString();
        }

        #region Метрики

        protected static long RequestsCount = 0;

        protected static long RequestsDuration = 0;

        private static Dictionary<string, ActionStats> ActionsStats = new Dictionary<string, ActionStats>();

        private static long _addCacheCountLast = 0;
        private static long _updateCacheCountLast = 0;

        private static TimeSpan metricActualInterval = TimeSpan.FromDays(365);
        private static TimeSpan saveCountersTimerInterval = TimeSpan.FromMinutes(5);

        private static void SendCacheMetrics()
        {
            var control = AllCaches.Events.ComponentControl;
            if (control != null && !control.IsFake())
            {
                var changed = AllCaches.Events.GetChangedCount();

                // добавилось 
                var oldAdd = _addCacheCountLast;
                _addCacheCountLast = AllCaches.Events.AddCacheCount;
                var add = _addCacheCountLast - oldAdd;

                // изменилось
                var oldUpdate = _updateCacheCountLast;
                _updateCacheCountLast = AllCaches.Events.UpdateCacheCount;
                var update = _updateCacheCountLast - oldUpdate;

                // статистика
                var updateStats = AllCaches.Events.GetUpdateStatsAndReset();

                // отправляем метрики
                control.SendMetrics(new List<SendMetricData>()
                {
                    new SendMetricData()
                    {
                        ActualInterval = metricActualInterval,
                        Name = "ChangedCount",
                        Value = changed
                    },
                    new SendMetricData()
                    {
                        ActualInterval = metricActualInterval,
                        Name = "AddCacheByInterval",
                        Value = add
                    },
                    new SendMetricData()
                    {
                        ActualInterval = metricActualInterval,
                        Name = "UpdateCacheByInterval",
                        Value = update
                    },
                    new SendMetricData()
                    {
                        ActualInterval = metricActualInterval,
                        Name = "Count",
                        Value = updateStats.Count
                    },
                    new SendMetricData()
                    {
                        ActualInterval = metricActualInterval,
                        Name = "AvgDurationSec",
                        Value = updateStats.Iterations > 0 ? (int) updateStats.Duration / updateStats.Iterations : 0
                    },
                    new SendMetricData()
                    {
                        ActualInterval = metricActualInterval,
                        Name = "AvgSpeedInSec",
                        Value = updateStats.Iterations > 0 ? (int) updateStats.Speed / updateStats.Iterations : 0
                    },
                    new SendMetricData()
                    {
                        ActualInterval = metricActualInterval,
                        Name = "MaxUpdateCount",
                        Value = updateStats.MaxCount
                    },
                    new SendMetricData()
                    {
                        ActualInterval = metricActualInterval,
                        Name = "MaxUpdateDurationSec",
                        Value = updateStats.MaxDuration
                    }
                });
            }
        }

        private static void SendMemoryMetrics()
        {
            var workingSetSize = Environment.WorkingSet / 1024 / 1024;
            ComponentControl.SendMetric("Memory Working Set, Mb", workingSetSize, metricActualInterval);

            var managedSize = GC.GetTotalMemory(false) / 1024 / 1024;
            ComponentControl.SendMetric("Memory Managed, Mb", managedSize, metricActualInterval);
        }

        private static void SendDatabaseSize()
        {
            try
            {
                var storageFactory = DependencyInjection.GetServicePersistent<IStorageFactory>();
                var storage = storageFactory.GetStorage();
                var size = storage.GetDatabaseSize() / 1024 / 1024;
                ComponentControl.SendMetric("Database size, Mb", size, metricActualInterval);
            }
            catch (Exception e)
            {
                var logger = DependencyInjection.GetLogger<DispatcherWebHandlerMiddleware>();
                logger.LogError(e, e.Message);
            }
        }

        protected static object CounterLockObject = new object();

        protected static void UpdateCounters(long ms, string action)
        {
            // TODO nolock stats
            lock (CounterLockObject)
            {
                RequestsCount++;
                RequestsDuration += ms;

                ActionsStats.TryGetValue(action, out var actionsStats);
                if (actionsStats == null)
                {
                    actionsStats = new ActionStats()
                    {
                        Action = action
                    };
                    ActionsStats.Add(action, actionsStats);
                }

                actionsStats.Count++;
                actionsStats.Duration += ms;
            }
        }

        internal class ActionStats
        {
            public IComponentControl Component;
            public string Action;
            public long Duration;
            public long Count;
        }

        private static void SaveCounters(object obj)
        {
            var threadId = DispatcherService.Wrapper.DeadLockHunter.Add("_SaveCounters_");

            try
            {
                SendCacheMetrics();
                SendMemoryMetrics();
                SendDatabaseSize();

                var avgRequestDuration = RequestsDuration / (RequestsCount > 0 ? RequestsCount : 1);
                var selfPercent = (RequestsDuration - InvokeDuration) * 100 / (RequestsDuration > 0 ? RequestsDuration : 1);
                var avgInvokeDuration = InvokeDuration / (RequestsCount > 0 ? RequestsCount : 1);

                var actualInterval = TimeSpan.FromDays(365);
                ComponentControl.SendMetric("Запросов в минуту", RequestsCount, actualInterval);
                ComponentControl.SendMetric("Мс на запрос", avgRequestDuration, actualInterval);
                ComponentControl.SendMetric("Процент собственных действий", selfPercent, actualInterval);
                ComponentControl.SendMetric("Мс на обработку", avgInvokeDuration, actualInterval);

                var stats = ActionsStats.Values.ToArray();
                foreach (var stat in stats)
                {
                    if (stat.Component == null)
                    {
                        stat.Component = ActionStatsFolder.GetOrCreateChildComponentControl("ActionStats", stat.Action);
                        stat.Component.IsFake();
                    }

                    var avgActionDuration = stat.Duration / (stat.Count > 0 ? stat.Count : 1);
                    stat.Component.SendMetric("Мс на запрос", avgActionDuration, actualInterval);
                    stat.Component.SendMetric("Запросов в минуту", stat.Count, actualInterval);
                }

                // TODO Collect storage stats
                // ComponentControl.SendMetric("Contexts.Account.Active", AccountDbContext.ActiveCount, actualInterval);
                // ComponentControl.SendMetric("Contexts.Account.Max", AccountDbContext.MaxActiveCount, actualInterval);

                lock (CounterLockObject)
                {
                    RequestsCount = 0;
                    RequestsDuration = 0;

                    foreach (var stat in stats)
                    {
                        stat.Count = 0;
                        stat.Duration = 0;
                    }
                }

                Interlocked.Exchange(ref InvokeDuration, 0);
            }
            catch (Exception exception)
            {
                var logger = DependencyInjection.GetLogger<DispatcherWebHandlerMiddleware>();
                logger.LogCritical(exception, exception.Message);
            }
            finally
            {
                DispatcherService.Wrapper.DeadLockHunter.Remove(threadId);
                _saveCountersTimer = new Timer(SaveCounters, _saveCountersTimer, saveCountersTimerInterval, Timeout.InfiniteTimeSpan);
            }
        }

        private static Timer _saveCountersTimer;

        #endregion

        protected override MethodInfo GetMethodInfo(string action)
        {
            // TODO Cache reflection results
            return typeof(DispatcherWrapper).GetMethod(action, BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy | BindingFlags.IgnoreCase);
        }

        protected override object GetRealHandler()
        {
            return DispatcherService.Wrapper;
        }
    }
}
