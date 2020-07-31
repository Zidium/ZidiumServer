using System;
using System.Net.NetworkInformation;
using System.Threading;
using NLog;
using Zidium.Core.AccountsDb;
using Zidium.Core.Api;
using Zidium.Core.Common;
using Zidium.Core.Common.Helpers;
using Zidium.Storage;

namespace Zidium.Agent.AgentTasks
{
    public class PingProcessor : UnitTestProcessorBase
    {
        public PingProcessor(ILogger logger, CancellationToken cancellationToken, ITimeService timeService)
            : base(logger, cancellationToken, timeService)
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

            using (var ping = new Ping())
            {
                var timeoutMs = (int) timeout.TotalMilliseconds;
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
        }

        protected override Guid GetUnitTestTypeId()
        {
            return SystemUnitTestType.PingTestType.Id;
        }

        protected override UnitTestExecutionInfo GetResult(
            Guid accountId,
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

            var rule = storage.UnitTestPingRules.GetOneOrNullByUnitTestId(unitTest.Id);
            if (rule == null)
            {
                return null; // Правило ещё не сохранено
            }

            var timeout = TimeSpan.FromMilliseconds(rule.TimeoutMs);
            PingResult result = null;

            token.ThrowIfCancellationRequested();

            try
            {
                result = Ping(rule.Host, timeout);
            }
            catch (Exception exception)
            {
                exception.Data.Add("AccountId", accountId);
                exception.Data.Add("UnitTestId", rule.UnitTestId);
                exception.Data.Add("UnitTestName", unitTest.DisplayName);
                exception.Data.Add("Address", rule.Host);
                logger.Error(exception);

                // ошибка агента, не обновляем данные проверки
                return null;
            }

            if (result == null)
                return null;

            var ruleForUpdate = rule.GetForUpdate();
            if (result.ErrorCode == PingErrorCode.Success)
            {
                ruleForUpdate.LastRunErrorCode.Set(PingErrorCode.Success);
                storage.UnitTestPingRules.Update(ruleForUpdate);

                return new UnitTestExecutionInfo()
                {
                    ResultRequest = new SendUnitTestResultRequestData()
                    {
                        Message = "Успешно",
                        Result = UnitTestResult.Success
                    }
                };
            }

            ruleForUpdate.LastRunErrorCode.Set(result.ErrorCode);
            storage.UnitTestPingRules.Update(ruleForUpdate);

            return new UnitTestExecutionInfo()
            {
                ResultRequest = new SendUnitTestResultRequestData()
                {
                    Message = result.ErrorMessage,
                    Result = UnitTestResult.Alarm,
                },
                IsNetworkProblem = true // Для пинга всегда будем использовать попытки
            };
        }
    }
}
