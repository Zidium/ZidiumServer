using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using NLog;
using Zidium.Core;
using Zidium.Core.Api;
using Zidium.Core.Api.Dispatcher;
using Zidium.Core.Common;
using Zidium.Core.Common.Helpers;
using Zidium.Core.ConfigDb;
using Zidium.Storage;

namespace Zidium.Agent.AgentTasks
{
    public abstract class UnitTestProcessorBase
    {
        public MultipleDataBaseProcessor DbProcessor { get; protected set; }

        public ITimeService TimeService { get; set; }

        protected ILogger Logger;

        /// <summary>
        /// Количество проверок, которые удалось вычислить.
        /// Сюда входят и проверки у которых статус alarm
        /// </summary>
        public int SuccessCount;

        /// <summary>
        /// Количество проверок, которые не удалось проверить из-за ошибки
        /// </summary>
        public int ErrorCount;

        public int AllCount
        {
            get { return ErrorCount + SuccessCount; }
        }

        protected int MaxParallelTasks = 20;

        protected int? MaxParallelAccounts = null;

        protected UnitTestProcessorBase(ILogger logger, CancellationToken cancellationToken, ITimeService timeService)
        {
            Logger = logger;
            DbProcessor = new MultipleDataBaseProcessor(logger, cancellationToken);
            TimeService = timeService;
        }

        public void ProcessAll()
        {
            ResetCounters();

            if (MaxParallelAccounts.HasValue)
                DbProcessor.AccountThreads = MaxParallelAccounts.Value;

            DbProcessor.ForEachAccount(x =>
            {
                if (x.Account.Type != AccountType.Test)
                {
                    ProcessAccount(
                      x.Account.Id,
                      x.Storage,
                      x.Logger,
                      x.Account.DisplayName,
                      x.CancellationToken);
                }
            });
        }

        public void ProcessAccount(Guid accountId, Guid unitTestId)
        {
            ResetCounters();

            if (MaxParallelAccounts.HasValue)
                DbProcessor.AccountThreads = MaxParallelAccounts.Value;

            DbProcessor.ForEachAccount(x =>
            {
                if (x.Account.Id == accountId)
                {
                    ProcessAccount(
                        x.Account.Id,
                        x.Storage,
                        x.Logger,
                        x.Account.DisplayName,
                        x.CancellationToken,
                        unitTestId);
                }
            });
        }

        protected void ResetCounters()
        {
            ErrorCount = 0;
            SuccessCount = 0;
        }

        protected abstract Guid GetUnitTestTypeId();

        protected void ProcessAccount(
            Guid accountId,
            IStorage storage,
            ILogger logger,
            string accountName,
            CancellationToken token,
            Guid? unitTestId = null)
        {
            var stopwatch = Stopwatch.StartNew();

            var unitTestTypeId = GetUnitTestTypeId();
            var tests = storage.UnitTests.GetForProcessing(unitTestTypeId, TimeService.Now());

            if (unitTestId.HasValue)
                tests = tests.Where(t => t.Id == unitTestId.Value).ToArray();

            // был баг, когда компонент был выключен, а проверка имела ParentEnable == True
            // в итоге агент каждый раз выполнял проверку, отправлял диспетчеру, а тот ее игнорировал
            // tests = tests.Where(x => x.Component.CanProcess).ToList();

            if (tests.Length == 0)
            {
                if (logger.IsTraceEnabled)
                    logger.Trace("В аккаунте {0} нет проверок для выполнения", accountName);
                return;
            }
            if (logger.IsDebugEnabled)
                logger.Debug("В аккаунте {0} начинаем выполнять {1} проверок", accountName, tests.Length);

            // для одного аккаунта будет задействовано максимум 20 потоков на все его проверки
            var tasks = new ThreadTaskQueue(MaxParallelTasks);

            int successCount = 0;
            int errorCount = 0;
            tasks.ForEach(tests, test =>
            {
                try
                {
                    ProcessUnitTest(accountId, test, logger, accountName, token, ref successCount, ref errorCount);
                }
                catch (OperationCanceledException) { }
            });

            stopwatch.Stop();

            if (successCount > 0 || errorCount > 0)
                logger.Info("Выполнено проверок успешно: {0}, с ошибкой: {1}, за {2}", successCount, errorCount, TimeSpanHelper.Get2UnitsString(stopwatch.Elapsed));

            Interlocked.Add(ref SuccessCount, successCount);
            Interlocked.Add(ref ErrorCount, errorCount);
        }

        protected DispatcherClient GetDispatcherClient()
        {
            return new DispatcherClient(GetType().Name);
        }

        protected abstract UnitTestExecutionInfo GetResult(Guid accountId,
            IStorage storage,
            UnitTestForRead unitTest,
            ILogger logger,
            string accountName,
            CancellationToken token);

        protected void ProcessUnitTest(
            Guid accountId, 
            UnitTestForRead unitTest, 
            ILogger logger, 
            string accountName, 
            CancellationToken token,
            ref int successCount,
            ref int errorCount)
        {
            token.ThrowIfCancellationRequested();

            try
            {
                if (logger.IsInfoEnabled)
                {
                    logger.Info($"Выполняем проверку {unitTest.Id}; попытка {unitTest.AttempCount}");
                }

                var dispatcher = GetDispatcherClient();
                if (logger.IsDebugEnabled)
                    logger.Debug("Имя проверки: " + unitTest.SystemName);
                try
                {
                    // Получаем отдельный экземпляр storage, потому что проверки выполняются параллельно
                    var accountStorageFactory = DependencyInjection.GetServicePersistent<IAccountStorageFactory>();
                    var storage = accountStorageFactory.GetStorageByAccountId(accountId);

                    var result = GetResult(accountId, storage, unitTest, logger, accountName, token);

                    if (result == null)
                    {
                        logger.Warn("Отмена выполнения проверки {0} в аккаунте {1} из-за внутренней проблемы", unitTest.Id, accountName);
                        return;
                    }

                    // если есть результат выполнения
                    if (result.ResultRequest != null)
                    {
                        // переопределим цвет проверки
                        if (result.ResultRequest.Result == UnitTestResult.Alarm && unitTest.ErrorColor.HasValue)
                        {
                            result.ResultRequest.Result = unitTest.ErrorColor.Value;
                        }

                        result.ResultRequest.UnitTestId = unitTest.Id;
                        if (result.ResultRequest.ActualIntervalSeconds == null)
                        {
                            result.ResultRequest.ActualIntervalSeconds = unitTest.PeriodSeconds * 2;
                        }

                        // Если попытка провалилась из-за проблем с сетью, но количество неуспешных попыток не превышено,
                        // то отправим повторно предыдущий результат
                        // Если попытка успешная, то сбросим счётчик
                        var attempCount = unitTest.AttempCount + 1;
                        var needNextAttemp = false;
                        if (result.IsNetworkProblem)
                        {
                            if (attempCount < unitTest.AttempMax)
                            {
                                var bulb = storage.Bulbs.GetOneById(unitTest.StatusDataId);
                                var previousStatus = bulb.Status;
                                if (previousStatus == MonitoringStatus.Alarm)
                                    result.ResultRequest.Result = UnitTestResult.Alarm;
                                else if (previousStatus == MonitoringStatus.Warning)
                                    result.ResultRequest.Result = UnitTestResult.Warning;
                                else if (previousStatus == MonitoringStatus.Success)
                                    result.ResultRequest.Result = UnitTestResult.Success;
                                else if (previousStatus == MonitoringStatus.Unknown)
                                    result.ResultRequest.Result = UnitTestResult.Unknown;
                                result.ResultRequest.Message = "Ошибка сети? " + result.ResultRequest.Message;
                                needNextAttemp = true;
                            }
                        }
                        else
                        {
                            attempCount = 0;
                        }

                        result.ResultRequest.AttempCount = attempCount;


                        // Если попытки не закончились, то запланируем следующую через минуту, а не через штатный интервал
                        if (needNextAttemp && result.ResultRequest.NextExecutionTime == null)
                        {
                            result.ResultRequest.NextExecutionTime = TimeService.Now().AddMinutes(1);
                        }

                        var response = dispatcher.SendUnitTestResult(accountId, result.ResultRequest);
                        response.Check();

                        logger.Info(unitTest.Id + " => " + (result.ResultRequest.Result ?? UnitTestResult.Unknown));
                        Interlocked.Increment(ref successCount);
                    }
                    else
                    {
                        // нет результата выполнения
                        if (result.NextStepProcessTime == null)
                        {
                            logger.Warn("Отмена выполнения проверки {0} в аккаунте {1} из-за внутренней проблемы", unitTest.Id, accountName);
                            return;
                        }

                        // результата выполнения нет, т.к. нужно выполнить следующий шаг
                        var response = dispatcher.SetUnitTestNextStepProcessTime(accountId, new SetUnitTestNextStepProcessTimeRequestData()
                        {
                            UnitTestId = unitTest.Id,
                            NextStepProcessTime = result.NextStepProcessTime
                        });
                        response.Check();
                    }
                }
                catch (OperationCanceledException)
                {
                    throw;
                }
                catch (Exception exception)
                {
                    Interlocked.Increment(ref errorCount);

                    exception.Data.Add("AccountName", accountName);
                    exception.Data.Add("UnitTestId", unitTest.Id);
                    exception.Data.Add("UnitTestName", unitTest.DisplayName);
                    logger.Error(exception);
                }

            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception exception)
            {
                Interlocked.Increment(ref errorCount);

                exception.Data.Add("AccountName", accountName);
                exception.Data.Add("UnitTestId", unitTest.Id);
                logger.Error(exception);
            }
        }
    }
}
