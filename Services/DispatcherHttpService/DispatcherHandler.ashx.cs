using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Web;
using NLog;
using Zidium.Api;
using Zidium.Core;
using Zidium.Core.Api;
using Zidium.Core.Caching;
using Zidium.Core.Common;
using Zidium.Core.Common.Helpers;
using Zidium.Core.ConfigDb;

namespace Zidium.DispatcherHttpService
{
    public class DispatcherHandler : WebHandlerBase
    {
        private static IComponentControl ComponentControl { get; set; }

        private static ComponentSelfTest SelfTest { get; set; }

        public static void Init(IComponentControl componentControl)
        {
            if (componentControl == null)
            {
                throw new ArgumentNullException("componentControl");
            }
            ComponentControl = componentControl;

            // запустим тест самопроверки
            SelfTest = new ComponentSelfTest(ComponentControl);
            SelfTest.AddUnitTest("CheckComponentControl", CheckComponentControl);
            SelfTest.AddUnitTest("CheckConfigDbContext", CheckConfigDbContext);
            SelfTest.AddUnitTest("AllCaches.Events.LastSaveException", () => CheckException(AllCaches.Events.LastSaveException));
            SelfTest.AddUnitTest("AllCaches.StatusDatas.LastSaveException", () => CheckException(AllCaches.StatusDatas.LastSaveException));
            SelfTest.AddUnitTest("AllCaches.UnitTests.LastSaveException", () => CheckException(AllCaches.UnitTests.LastSaveException));

            SelfTest.StartTimer(TimeSpan.FromMinutes(1));
            _saveCountersTimer = new Timer(SaveCounters, _saveCountersTimer, TimeSpan.FromMinutes(1), TimeSpan.FromMilliseconds(-1));

            //DeadLockHunter = new DeadLockHunter(componentControl);
        }

        private static void CheckComponentControl()
        {
            if (ComponentControl.IsFake())
            {
                throw new Exception("Component control is fake");
            }
        }

        private static void CheckConfigDbContext()
        {
            var configDbServicesFactory = DependencyInjection.GetServicePersistent<IConfigDbServicesFactory>();
            configDbServicesFactory.GetAccountService().GetSystemAccount();
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

        private string GetExceptionTempString(ExceptionTempInfo exception)
        {
            if (exception == null)
            {
                return "";
            }
            return exception.Date + " " + exception.Exception.ToString();
        }

        protected override string GetDebugInfo()
        {
            var output = new StringBuilder();

            output.AppendLine("DispatcherWrapper.Uptime = " + DispatcherWrapper.UptimeTimer.Elapsed);
            output.AppendLine("------------");

            if (ComponentControl.IsFake() == false)
            {
                output.AppendLine("Api.EventManager.GetQueueSize(): " + DataSizeHelper.GetSizeText(ComponentControl.Client.EventManager.GetQueueSize()));
                output.AppendLine("Api.WebLogManager.GetQueueSize(): " + DataSizeHelper.GetSizeText(ComponentControl.Client.WebLogManager.GetQueueSize()));
            }
            output.AppendLine("------------");
            output.AppendLine("");

            foreach (var cache in AllCaches.All)
            {
                output.AppendLine("--- " + cache.GetType() + " ---");
                output.AppendLine("Count: " + cache.Count + " / " + cache.MaxCount);
                output.AppendLine("Size: " + DataSizeHelper.GetSizeText(cache.GetSize()));
                output.AppendLine("Changed: " + cache.GetChangedCount());
                output.AppendLine("LastSaveChangesDate: " + cache.GetLastSaveChangesDate());
                output.AppendLine("AddCacheCount: " + cache.AddCacheCount);
                output.AppendLine("AddDataBaseCount: " + cache.AddDataBaseCount);
                output.AppendLine("UpdateCacheCount: " + cache.UpdateCacheCount);
                output.AppendLine("UpdateDataBaseCount: " + cache.UpdateDataBaseCount);
                output.AppendLine("Generation: " + cache.Generation);
                output.AppendLine("LastSaveException: " + GetExceptionTempString(cache.LastSaveException));

                output.AppendLine("");
            }

            return output.ToString();
        }

        private static void CheckException(ExceptionTempInfo exception)
        {
            if (exception == null || exception.Exception == null)
            {
                return;
            }
            var duration = DateTime.Now - exception.Date;
            if (duration < TimeSpan.FromMinutes(10))
            {
                throw exception.Exception;
            }
        }

        protected override string GetTestInfo()
        {
            var result = SelfTest.Validate();
            return result.Log;
        }

        public override void ProcessRequest(HttpContext httpContext, out string action)
        {
            var timer = new Stopwatch();
            timer.Start();

            base.ProcessRequest(httpContext, out action);

            timer.Stop();

            // Считаем статистику только по корректным названиям action
            if (!string.IsNullOrEmpty(action))
            {
                UpdateCounters((long)timer.Elapsed.TotalMilliseconds, action);
            }
        }

        public static string GetVersion()
        {
            return typeof(DispatcherHandler).Assembly.GetName().Version.ToString();
        }

        #region Метрики

        protected static Stopwatch CounterTimer = new Stopwatch();

        protected static Int64 RequestsCount = 0;

        protected static Int64 RequestsDuration = 0;

        private static Dictionary<string, ActionStats> ActionsStats = new Dictionary<string, ActionStats>();

        private static long _addCacheCountLast = 0;
        private static long _updateCacheCountLast = 0;

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

                // самый долгий цикл
                var updateStats = AllCaches.Events.GetUpdateStatsAndReset();

                // отправляем метрики
                var actualInterval = TimeSpan.FromMinutes(10);
                control.SendMetrics(new List<SendMetricData>()
                {
                    new SendMetricData()
                    {
                        ActualInterval = actualInterval,
                        Name = "ChangedCount",
                        Value = changed
                    },
                    new SendMetricData()
                    {
                        ActualInterval = actualInterval,
                        Name = "AddCacheByInterval",
                        Value = add
                    },
                    new SendMetricData()
                    {
                        ActualInterval = actualInterval,
                        Name = "UpdateCacheByInterval",
                        Value = update
                    },
                    new SendMetricData()
                    {
                        ActualInterval = actualInterval,
                        Name = "MaxUpdateCount",
                        Value = updateStats.MaxCount
                    },
                    new SendMetricData()
                    {
                        ActualInterval = actualInterval,
                        Name = "MaxUpdateDurationSec",
                        Value = updateStats.MaxDuration
                    }
                });
            }
        }

        private static void SendMemoryMetrics()
        {
            var actualInterval = TimeSpan.FromDays(365);

            var workingSetSize = Environment.WorkingSet / 1024 / 1024;
            ComponentControl.SendMetric("Memory Working Set, Mb", workingSetSize, actualInterval);

            var managedSize = GC.GetTotalMemory(false) / 1024 / 1024;
            ComponentControl.SendMetric("Memory Managed, Mb", managedSize, actualInterval);
        }

        protected static object CounterLockObject = new object();

        protected static void UpdateCounters(long ms, string action)
        {
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
                LogManager.GetCurrentClassLogger().Fatal(exception);
            }
            finally
            {
                DispatcherService.Wrapper.DeadLockHunter.Remove(threadId);
                _saveCountersTimer = new Timer(SaveCounters, _saveCountersTimer, TimeSpan.FromMinutes(1), TimeSpan.FromMilliseconds(-1));
            }
        }

        private static Timer _saveCountersTimer;

        #endregion

        protected override MethodInfo GetMethodInfo(string action)
        {
            return typeof(IDispatcherService).GetMethod(action);
        }

        protected override object GetRealHandler(string ip, HttpContext httpContext)
        {
            return DispatcherService.Wrapper;
        }

        static DispatcherHandler()
        {
            CounterTimer.Start();
        }

    }
}