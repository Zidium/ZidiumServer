using System;
using System.Diagnostics;
using System.Threading;
using Microsoft.Extensions.Logging;
using Zidium.Api;
using Zidium.Api.Dto;
using Zidium.Core;
using Zidium.Core.Common;
using Zidium.Core.InternalLogger;

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

        protected ITimeService TimeService { get; set; }

        protected abstract AgentTaskResult Do();

        public string Name
        {
            get { return GetType().Name; }
        }

        protected ILogger Logger;

        protected IAgentConfiguration AgentConfiguration;

        protected AgentTaskBase()
        {
            TimeService = DependencyInjection.GetServicePersistent<ITimeService>();
            ExecutionPeriod = TimeSpan.FromMinutes(10);
            WaitOnErrorTime = TimeSpan.FromMinutes(1);

            AgentConfiguration = DependencyInjection.GetServicePersistent<IAgentConfiguration>();
            _maximumOfflineInterval = TimeSpan.Parse(AgentConfiguration.MaximumOfflineInterval);

            var debugConfiguration = DependencyInjection.GetServicePersistent<IDebugConfiguration>();
            var typeControl = Client.Instance.GetOrCreateComponentTypeControl(!debugConfiguration.DebugMode ? "AgentTask" : DebugHelper.DebugComponentType);
            _componentControl = Client.Instance.GetDefaultComponentControl().GetOrCreateChildComponentControl(typeControl, Name);

            var mapping = DependencyInjection.GetServicePersistent<InternalLoggerComponentMapping>();
            mapping.MapLoggerToComponent(Name, _componentControl.Info.Id);

            Logger = DependencyInjection.GetLogger(Name);
        }

        protected IUnitTestControl MainUnitTest;

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
            Logger.LogDebug("Запуск рабочего потока задачи");
            Logger.LogInformation($"Period: {ExecutionPeriod}");
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
                        Logger.LogError(exception, exception.Message);

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

            Logger.LogDebug("Рабочий поток задачи остановлен");
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
                if (!WorkThread.Join(TimeSpan.FromSeconds(60))) // ждем максимум 60 сек
                    WorkThread.Interrupt();
                WorkThread = null;
            }
        }

    }
}
