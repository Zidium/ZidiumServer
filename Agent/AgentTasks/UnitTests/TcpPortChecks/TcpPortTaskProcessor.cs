using System;
using System.Threading;
using NLog;
using Zidium.Agent.AgentTasks.UnitTests.TcpPortChecks;
using Zidium.Core.AccountsDb;
using Zidium.Core.Api;

namespace Zidium.Agent.AgentTasks
{
    public class TcpPortTaskProcessor : UnitTestProcessorBase
    {
        public TcpPortTaskProcessor(ILogger logger, CancellationToken cancellationToken)
            : base(logger, cancellationToken)
        {
        }
       
        protected override Guid GetUnitTestTypeId()
        {
            return SystemUnitTestTypes.TcpPortTestType.Id;
        }

        protected override UnitTestExecutionInfo GetResult(Guid accountId,
            AccountDbContext accountDbContext,
            UnitTest unitTest,
            ILogger logger,
            string accountName,
            CancellationToken token)
        {
            if (unitTest == null)
            {
                throw new ArgumentNullException("unitTest");
            }

            var rule = unitTest.TcpPortRule;
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
                exception.Data.Add("UnitTestName", rule.UnitTest.DisplayName);
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
                        Message = "Порт закрыт",
                        Result = rule.Opened ? UnitTestResult.Alarm : UnitTestResult.Success,
                        IsNetworkProblem = true // чтобы выполнить 2-ую попытку
                    };
            }

            // открыт
            if (result.Code == TcpPortCheckCode.Opened)
            {
                return new UnitTestExecutionInfo()
                {
                    Message = "Порт открыт",
                    Result = rule.Opened ? UnitTestResult.Success : UnitTestResult.Alarm
                };
            }

            // ошибки
            var executionInfo = new UnitTestExecutionInfo()
            {
                Message = result.Message,
                Result = UnitTestResult.Alarm
            };
            if (result.Code == TcpPortCheckCode.NoIp)
            {
                executionInfo.IsNetworkProblem = true;
            }
            return executionInfo;
        }
    }
}
