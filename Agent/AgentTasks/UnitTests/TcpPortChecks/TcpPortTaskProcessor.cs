using System;
using System.Threading;
using NLog;
using Zidium.Agent.AgentTasks.UnitTests.TcpPortChecks;
using Zidium.Core.AccountsDb;
using Zidium.Core.Api;
using Zidium.Core.Common;
using Zidium.Storage;

namespace Zidium.Agent.AgentTasks
{
    public class TcpPortTaskProcessor : UnitTestProcessorBase
    {
        public TcpPortTaskProcessor(ILogger logger, CancellationToken cancellationToken, ITimeService timeService)
            : base(logger, cancellationToken, timeService)
        {
        }
       
        protected override Guid GetUnitTestTypeId()
        {
            return SystemUnitTestType.TcpPortTestType.Id;
        }

        protected override UnitTestExecutionInfo GetResult(Guid accountId,
            IStorage storage,
            UnitTestForRead unitTest,
            ILogger logger,
            string accountName,
            CancellationToken token)
        {
            if (unitTest == null)
            {
                throw new ArgumentNullException("unitTest");
            }

            var rule = storage.UnitTestTcpPortRules.GetOneOrNullByUnitTestId(unitTest.Id);
            if (rule == null)
            {
                return null; // Правило ещё не сохранено
            }

            token.ThrowIfCancellationRequested();

            TcpPortCheckOutputData result = null;
            try
            {
                var processor = new TcpPortCheckProcessor();
                var inputData = new TcpPortCheckInputData()
                {
                    Host = rule.Host,
                    Port = rule.Port
                };
                result = processor.Process(inputData);
            }
            catch (Exception exception)
            {
                exception.Data.Add("AccountId", accountId);
                exception.Data.Add("UnitTestId", rule.UnitTestId);
                exception.Data.Add("UnitTestName", unitTest.DisplayName);
                exception.Data.Add("Address", rule.Host);
                exception.Data.Add("Port", rule.Port);
                logger.Error(exception);

                // ошибка агента, не обновляем данные проверки
                return null;
            }

            if (result == null)
                return null;

            // закрыт
            if (result.Code == TcpPortCheckCode.Closed)
            {
                    return new UnitTestExecutionInfo()
                    {
                        ResultRequest = new SendUnitTestResultRequestData()
                        {
                            Message = "Порт закрыт",
                            Result = rule.Opened ? UnitTestResult.Alarm : UnitTestResult.Success
                        },
                        IsNetworkProblem = true // чтобы выполнить 2-ую попытку
                    };
            }

            // открыт
            if (result.Code == TcpPortCheckCode.Opened)
            {
                return new UnitTestExecutionInfo()
                {
                    ResultRequest = new SendUnitTestResultRequestData()
                    {
                        Message = "Порт открыт",
                        Result = rule.Opened ? UnitTestResult.Success : UnitTestResult.Alarm
                    }
                };
            }

            // ошибки
            var executionInfo = new UnitTestExecutionInfo()
            {
                ResultRequest = new SendUnitTestResultRequestData()
                {
                    Message = result.Message,
                    Result = UnitTestResult.Alarm
                }
            };
            if (result.Code == TcpPortCheckCode.NoIp)
            {
                executionInfo.IsNetworkProblem = true;
            }
            return executionInfo;
        }
    }
}
