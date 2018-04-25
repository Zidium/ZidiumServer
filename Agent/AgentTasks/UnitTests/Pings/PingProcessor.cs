using System;
using System.Net.NetworkInformation;
using System.Threading;
using NLog;
using Zidium.Core.AccountsDb;
using Zidium.Core.AccountsDb.Classes.UnitTests.PingTests;
using Zidium.Core.Api;
using Zidium.Core.Common.Helpers;

namespace Zidium.Agent.AgentTasks
{
    public class PingProcessor : UnitTestProcessorBase
    {
        public PingProcessor(ILogger logger, CancellationToken cancellationToken)
            : base(logger, cancellationToken)
        {
        }

        public static PingResult Ping(string nameOrAddress, TimeSpan timeout)
        {
            var ip = NetworkHelper.GetIpFromHost(nameOrAddress);
            if (ip == null)
            {
                return new PingResult()
                {
                    ErrorCode = PingErrorCode.UnknownDomain,
                    ErrorMessage = "Не удалось получить IP адрес"
                };
            }
            var ping = new Ping();
            int timeoutMs = (int) timeout.TotalMilliseconds;
            var buffer = new byte[] {1, 2, 3, 4};
            var option = new PingOptions()
            {
                DontFragment = false,
                Ttl = 32
            };

            var reply = ping.Send(ip, timeoutMs, buffer, option);

            if (reply == null)
                return null;

            if (reply.Status == IPStatus.Success)
            {
                return new PingResult()
                {
                    ErrorCode = PingErrorCode.Success,
                    ErrorMessage = null
                };
            }

            return new PingResult()
            {
                ErrorCode = PingErrorCode.Timeout,
                ErrorMessage = "Ошибка: " + reply.Status
            };
        }

        protected override Guid GetUnitTestTypeId()
        {
            return SystemUnitTestTypes.PingTestType.Id;
        }

        protected override SendUnitTestResultRequestData GetResult(
            Guid accountId, 
            AccountDbContext accountDbContext,
            UnitTest unitTest, 
            ILogger logger,
            CancellationToken token)
        {
            if (unitTest == null)
            {
                throw new ArgumentNullException("unitTest");
            }

            var rule = unitTest.PingRule;
            if (rule == null)
            {
                return null; // Правило ещё не сохранено
            }

            var timeOut = TimeSpan.FromMilliseconds(rule.TimeoutMs);
            PingResult result = null;
            int attemps = 5;

            // если пинга раньше не было, то нет смысла делать много попыток
            if (unitTest.Bulb.Status != MonitoringStatus.Success)
            {
                attemps = 2;
            }

            for (int i = 0; i < attemps; i++)
            {
                token.ThrowIfCancellationRequested();

                try
                {
                    result = Ping(rule.Host, timeOut);
                }
                catch (Exception exception)
                {
                    exception.Data.Add("AccountId", accountId);
                    exception.Data.Add("UnitTestId", rule.UnitTestId);
                    exception.Data.Add("UnitTestName", rule.UnitTest.DisplayName);
                    exception.Data.Add("Address", rule.Host);
                    logger.Error(exception);

                    // ошибка агента, не обновляем данные проверки
                    return null;
                }
                if (result.ErrorCode == PingErrorCode.Success)
                {
                    break;
                }
                if (IsInternetWork() == false)
                {
                    // если интернет не работает, то не обновляем данные проверки
                    return null;
                }
            }

            if (result == null)
                return null;

            if (result.ErrorCode == PingErrorCode.Success)
            {
                return new SendUnitTestResultRequestData()
                {
                    Message = "Успешно",
                    Result = UnitTestResult.Success
                };
            }

            return new SendUnitTestResultRequestData()
            {
                Message =  result.ErrorMessage,
                Result = UnitTestResult.Alarm
            };
        }
    }
}
