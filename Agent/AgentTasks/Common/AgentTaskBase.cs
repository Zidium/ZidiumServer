using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using NLog;
using Zidium.Api;
using Zidium.Core;
using Zidium.Core.Common;

namespace Zidium.Agent.AgentTasks
{
    public abstract class AgentTaskBase
    {
        /// <summary>
        /// Периодичность выполнения
        /// </summary>
        protected TimeSpan ExecutionPeriod;

        /// <summary>
        /// Сколько ждать при ошибке
        /// </summary>
        protected TimeSpan WaitOnErrorTime;

        /// <summary>
        /// Максимально допустимый период отсутствия сигналов от задач
        /// </summary>
        private readonly TimeSpan _maximumOfflineInterval;

        private readonly IComponentControl _componentControl;

        protected CancellationToken CancellationToken;

        protected Thread WorkThread;

        protected abstract AgentTaskResult Do();

        public string Name
        {
            get { return GetType().Name; }
        }

        protected ILogger Logger;

        protected AgentTaskBase()
        {
            ExecutionPeriod = TimeSpan.FromMinutes(10);
            WaitOnErrorTime = TimeSpan.FromMinutes(1);
            _maximumOfflineInterval = ServiceConfiguration.MaximumOfflineInterval;

            var typeControl = Client.Instance.GetOrCreateComponentTypeControl(!DebugHelper.IsDebugMode ? "AgentTask" : DebugHelper.DebugComponentType);
            _componentControl = Client.Instance.GetDefaultComponentControl().GetOrCreateChildComponentControl(typeControl, Name);
            var rule = LogManager.Configuration.LoggingRules.FirstOrDefault(t => t.LoggerNamePattern == "Agent");
            var minLevel = rule != null ? rule.Levels.Min() : NLog.LogLevel.Info;
            var targetName = "AgentTask." + Name;
            LogManager.Configuration.AddTarget(targetName, new Core.InternalNLogAdapter.NLogTarget(_componentControl.Info.Id));
            LogManager.Configuration.AddRule(minLevel, NLog.LogLevel.Fatal, targetName, Name + "*");
            Logger = LogManager.GetLogger(Name);
        }

        protected IUnitTestControl MainUnitTest;

        protected IUnitTestControl ActivityUnitTest;

        protected void SetMainStatus(AgentTaskResult result, TimeSpan actualInterval)
        {
            var unitTestResult = new SendUnitTestResultData()
            {
                ActualInterval = actualInterval,
                Message = result.Message,
                Result = result.Status
            };
            MainUnitTest.SendResult(unitTestResult);
        }

        protected void DoWrapper()
        {
            MainUnitTest = _componentControl.GetOrCreateUnitTestControl("Main");
            Logger.Debug("Запуск рабочего потока задачи");
            try
            {
                while (true)
                {
                    try
                    {
                        if (IsTaskComponentEnabled())
                            ExecuteTask();

                        var nextRunDate = DateTime.Now.Add(ExecutionPeriod);
                        while (DateTime.Now < nextRunDate)
                        {
                            CancellationToken.ThrowIfCancellationRequested();
                            Thread.Sleep(1000);
                        }
                    }
                    catch (OperationCanceledException)
                    {
                        break;
                    }
                    catch (ThreadAbortException)
                    {
                        break;
                    }
                    catch (Exception exception)
                    {
                        Logger.Error(exception);

                        // Выполнено с проблемами - до следующего запуска задача агента будет жёлтой
                        var result = new AgentTaskResult(UnitTestResult.Warning, exception.Message);
                        SetMainStatus(result, ExecutionPeriod + _maximumOfflineInterval);

                        var waitForTime = DateTime.Now + WaitOnErrorTime;
                        while (DateTime.Now < waitForTime)
                        {
                            CancellationToken.ThrowIfCancellationRequested();
                            Thread.Sleep(1000);
                        }
                    }
                }
            }
            catch (OperationCanceledException) { }
            catch (ThreadAbortException) { }

            Logger.Debug("Рабочий поток задачи остановлен");
        }

        protected void ExecuteTask()
        {
            CancellationToken.ThrowIfCancellationRequested();

            var stopwatch = Stopwatch.StartNew();
            var result = Do(); // чтобы видеть полезную информацию, например: отправлено 10 писем
            stopwatch.Stop();

            _componentControl.SendMetric("Время выполнения", (int)stopwatch.Elapsed.TotalSeconds, TimeSpan.FromDays(365));

            // Выполнено успешно - до следующего запуска задача агента будет зелёной
            SetMainStatus(result, ExecutionPeriod + _maximumOfflineInterval);
        }

        protected bool IsTaskComponentEnabled()
        {
            var response = _componentControl.GetTotalState(false);
            if (!response.Success)
                return false;
            var state = response.Data;
            return state.Status != MonitoringStatus.Disabled;
        }

        public void Start(CancellationToken cancellationToken)
        {
            CancellationToken = cancellationToken;


            WorkThread = new Thread(DoWrapper)
            {
                Name = Name
            };
            WorkThread.Start();
        }

        public void WaitForStop()
        {
            if (WorkThread != null)
            {
                if (!WorkThread.Join(TimeSpan.FromSeconds(60))) //ждем максимум 60 сек
                    WorkThread.Abort();
                WorkThread = null;
            }
        }

        protected AgentTaskResult GetResult(MultipleDataBaseProcessor dbProcessor, string result)
        {
            return new AgentTaskResult(
                dbProcessor.FirstException == null ? UnitTestResult.Success : UnitTestResult.Warning,
                dbProcessor.FirstException == null ? result : dbProcessor.FirstException.Message
                );
        }
    }
}
