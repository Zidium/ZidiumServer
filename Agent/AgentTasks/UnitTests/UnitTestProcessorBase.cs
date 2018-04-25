using System;
using System.Linq;
using System.Threading;
using NLog;
using Zidium.Core.AccountsDb;
using Zidium.Core.Api.Dispatcher;
using Zidium.Core.Common;
using Zidium.Core.Common.Helpers;
using SendUnitTestResultRequestData = Zidium.Core.Api.SendUnitTestResultRequestData;
using ThreadTaskQueue = Zidium.Core.Common.ThreadTaskQueue;
using UnitTestResult = Zidium.Core.Api.UnitTestResult;

namespace Zidium.Agent.AgentTasks
{
    public abstract class UnitTestProcessorBase
    {
        public MultipleDataBaseProcessor DbProcessor { get; protected set; }

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

        protected UnitTestProcessorBase(ILogger logger, CancellationToken cancellationToken)
        {
            Logger = logger;
            DbProcessor = new MultipleDataBaseProcessor(logger, cancellationToken);
        }

        public void ProcessAll()
        {
            ResetCounters();

            DbProcessor.ForEachAccount(x => ProcessAccount(
                x.Account.Id,
                x.AccountDbContext,
                x.Logger,
                x.Account.DisplayName,
                x.CancellationToken));
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

        protected bool IsInternetWork()
        {
            if (NetworkHelper.IsInternetWork())
            {
                return true;
            }

            Logger.Warn("Не работает интернет");

            return false;
        }

        protected void ProcessAccount(
            Guid accountId, 
            AccountDbContext accountDbContext, 
            ILogger logger, 
            string accountName, 
            CancellationToken token,
            Guid? unitTestId = null)
        {
            var now = GetDispatcherClient().GetServerTime().Data.Date;
            var repository = accountDbContext.GetUnitTestRepository();
            var unitTestTypeId = GetUnitTestTypeId();
            var tests = repository.GetForProcessing(unitTestTypeId, now);

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
                catch (OperationCanceledException)
                {

                }
            });

            if (SuccessCount > 0 || ErrorCount > 0)
                logger.Info("Выполнено проверок успешно: {0}, с ошибкой: {1}", SuccessCount, ErrorCount);
        }

        protected DispatcherClient GetDispatcherClient()
        {
            return new DispatcherClient(GetType().Name);
        }

        protected abstract SendUnitTestResultRequestData GetResult(
            Guid accountId,
            AccountDbContext accountDbContext,
            UnitTest unitTest,
            ILogger logger,
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
                        var result = GetResult(accountId, accountDbContext, unitTest, logger, token);

                        if (result == null)
                        {
                            logger.Warn("Отмена выполнения проверки {0} из-за нерабочего интернета", unitTestId);

                            return;
                        }

                        // переопределим цвет проверки
                        if (result.Result == UnitTestResult.Alarm && unitTest.ErrorColor.HasValue)
                        {
                            result.Result = unitTest.ErrorColor.Value;
                        }

                        result.UnitTestId = unitTestId;
                        if (result.ActualIntervalSeconds == null)
                        {
                            result.ActualIntervalSeconds = unitTest.PeriodSeconds * 2;
                        }

                        var response = dispatcher.SendUnitTestResult(accountId, result);
                        response.Check();

                        logger.Info(unitTest.Id + " => " + (result.Result ?? UnitTestResult.Unknown));
                        SuccessCount++;
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
