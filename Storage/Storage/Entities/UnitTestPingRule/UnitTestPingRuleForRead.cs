using System;

namespace Zidium.Storage
{
    /// <summary>
    /// Настройка проверки через команду ping
    /// </summary>
    public class UnitTestPingRuleForRead
    {
        public UnitTestPingRuleForRead(
            Guid unitTestId, 
            string host, 
            int timeoutMs, 
            PingErrorCode? lastRunErrorCode)
        {
            UnitTestId = unitTestId;
            Host = host;
            TimeoutMs = timeoutMs;
            LastRunErrorCode = lastRunErrorCode;
        }

        public Guid UnitTestId { get; }

        public string Host { get; }

        /// <summary>
        /// Таймаут
        /// </summary>
        public int TimeoutMs { get; }

        /// <summary>
        /// Код ошибки последнего выполнения
        /// </summary>
        public PingErrorCode? LastRunErrorCode { get; }

        public UnitTestPingRuleForUpdate GetForUpdate()
        {
            return new UnitTestPingRuleForUpdate(UnitTestId);
        }

    }
}
