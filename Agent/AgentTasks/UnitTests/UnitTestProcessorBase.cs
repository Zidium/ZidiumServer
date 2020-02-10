using System;
using System.Linq;
using System.Threading;
using NLog;
using Zidium.Core.AccountsDb;
using Zidium.Core.Api;
using Zidium.Core.Api.Dispatcher;
using Zidium.Core.Common;
using Zidium.Core.Common.TimeService;
using Zidium.Core.ConfigDb;

namespace Zidium.Agent.AgentTasks
{
    public abstract class UnitTestProcessorBase
    {
        public MultipleDataBaseProcessor DbProcessor { get; protected set; }

        public ITimeService TimeService { get; set; }

        protected ILogger Logger;

        /// <summary>
        /// Количество проверок, которые удалось вычисляить.
        /// Сюда входят и проверки у которых статус alarm
        /// </summary>
        public int SuccessCount { get; set; }

        /// <summary>
        /// Количество проверок, которые не удалось проверить из-за ошибки
        /// </summary>
        public int ErrorCount { get; set; }

        public int AllCount
        {
            get { return ErrorCount + SuccessCount; }
        }

        protected UnitTestProcessorBase(ILogger logger, CancellationToken cancellationToken, ITimeService timeService)
        {
            Logger = logger;
            DbProcessor = new MultipleDataBaseProcessor(logger, cancellationToken);
            TimeService = timeService;
        }

        public void ProcessAll()
        {
            ResetCounters();

            DbProcessor.ForEachAccount(x =>
            {
                if (x.Account.Type != AccountType.Test)
                {
                    ProcessAccount(
                      x.Account.Id,
                      x.AccountDbContext,
                      x.Logger,
                      x.Account.DisplayName,
                      x.CancellationToken);
                }
            });
        }

        public void ProcessAccount(Guid accountId, Guid unitTestId)
        {
            ResetCounters();

            DbProcessor.ForEachAccount(x =>
            {
                if (x.Account.Id == accountId)
                {
                    ProcessAccount(
                        x.Account.Id,
                        x.AccountDbContext,
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
            AccountDbContext accountDbContext,
            ILogger logger,
            string accountName,
            CancellationToken token,
            Guid? unitTestId = null)
        {
            var repository = accountDbContext.GetUnitTestRepository();
            var unitTestTypeId = GetUnitTestTypeId();
            var tests = repository.GetForProcessing(unitTestTypeId, TimeService.Now());

            if (unitTestId.HasValue)
                tests = tests.Where(t => t.Id == unitTestId.Value).ToList();

            // был баг, когда компонент был выключен, а проверка имела ParentEnable == True
            // в итоге агент каждый раз выполнял проверку, отправлял диспетчеру, а тот ее игнорировал
            // tests = tests.Where(x => x.Component.CanProcess).ToList();

            if (tests.Count == 0)
            {
                logger.Trace("В аккаунте {0} нет проверок для выполнения", accountName);
                return;
            }
            logger.Debug("В аккаунте {0} начинаем выполнять {1} проверок", accountName, tests.Count);
            // для одного аккаунта будет задействовано максимум 20 потоков на все его проверки
            var tasks = new ThreadTaskQueue(20);

            tasks.ForEach(tests, test =>
            {
                Guid testId = test.Id;
                try
                {
                    ProcessUnitTest(accountId, testId, logger, accountName, token);
                }
                catch (OperationCanceledException) { }
            });

            if (SuccessCount > 0 || ErrorCount > 0)
                logger.Info("Выполнено проверок успешно: {0}, с ошибкой: {1}", SuccessCount, ErrorCount);
        }

        protected DispatcherClient GetDispatcherClient()
        {
            return new DispatcherClient(GetType().Name);
        }

        protected abstract UnitTestExecutionInfo GetResult(Guid accountId,
            AccountDbContext accountDbContext,
            UnitTest unitTest,
            ILogger logger,
            string accountName,
            CancellationToken token);

        protected void ProcessUnitTest(Guid accountId, Guid unitTestId, ILogger logger, string accountName, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            try
            {
                logger.Debug("Выполняем проверку " + unitTestId);
                var dispatcher = GetDispatcherClient();
                using (var accountDbContext = AccountDbContext.CreateFromAccountId(accountId))
                {
                    var repository = accountDbContext.GetUnitTestRepository();
                    var unitTest = repository.GetById(unitTestId);
                    logger.Debug("Имя проверки: " + unitTest.SystemName);
                    try
                    {
                        var result = GetResult(accountId, accountDbContext, unitTest, logger, accountName, token);

                        if (result == null)
                        {
                            logger.Warn("Отмена выполнения проверки {0} в аккаунте {1} из-за внутренней проблемы", unitTestId, accountName);
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

                            result.ResultRequest.UnitTestId = unitTestId;
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
                                    var previousStatus = unitTest.Bulb.Status;
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
                            SuccessCount++;
                        }
                        else
                        {
                            // нет результата выполнения
                            if (result.NextStepProcessTime == null)
                            {
                                logger.Warn("Отмена выполнения проверки {0} в аккаунте {1} из-за внутренней проблемы", unitTestId, accountName);
                                return;
                            }

                            // результата выполнения нет, т.к. нужно выполнить следующий шаг
                            var response = dispatcher.SetUnitTestNextStepProcessTime(accountId, new SetUnitTestNextStepProcessTimeRequestData()
                            {
                                UnitTestId = unitTestId,
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
                        ErrorCount++;

                        exception.Data.Add("AccountName", accountName);
                        exception.Data.Add("UnitTestId", unitTestId);
                        exception.Data.Add("UnitTestName", unitTest.DisplayName);
                        exception.Data.Add("UnitTestType", unitTest.Type.DisplayName);
                        exception.Data.Add("ComponentName", unitTest.Component.DisplayName);
                        logger.Error(exception);
                    }
                }
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception exception)
            {
                ErrorCount++;

                exception.Data.Add("AccountName", accountName);
                exception.Data.Add("UnitTestId", unitTestId);
                logger.Error(exception);
            }
        }
    }
}
