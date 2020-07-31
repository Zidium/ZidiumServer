using System;

namespace Zidium.Storage
{
    /// <summary>
    /// Настройка проверки tcp-порта
    /// </summary>
    public class UnitTestTcpPortRuleForRead
    {
        public UnitTestTcpPortRuleForRead(
            Guid unitTestId,
            string host,
            int timeoutMs,
            int port,
            bool opened,
            int? lastRunErrorCode)
        {
            UnitTestId = unitTestId;
            Host = host;
            TimeoutMs = timeoutMs;
            Port = port;
            Opened = opened;
            LastRunErrorCode = lastRunErrorCode;
        }

        public Guid UnitTestId { get; }

        public string Host { get; }

        /// <summary>
        /// Таймаут
        /// </summary>
        public int TimeoutMs { get; }

        public int Port { get; }

        public bool Opened { get; }

        /// <summary>
        /// Код ошибки последнего выполнения
        /// </summary>
        public int? LastRunErrorCode { get; }

        public UnitTestTcpPortRuleForUpdate GetForUpdate()
        {
            return new UnitTestTcpPortRuleForUpdate(UnitTestId);
        }

    }
}
