using System;

namespace Zidium.Storage
{
    /// <summary>
    /// Настройка проверки через команду ping
    /// </summary>
    public class UnitTestPingRuleForUpdate
    {
        public UnitTestPingRuleForUpdate(Guid unitTestId)
        {
            UnitTestId = unitTestId;
            Host = new ChangeTracker<string>();
            TimeoutMs = new ChangeTracker<int>();
            LastRunErrorCode = new ChangeTracker<PingErrorCode?>();
        }

        public Guid UnitTestId { get; }

        public ChangeTracker<string> Host { get; }

        /// <summary>
        /// Таймаут
        /// </summary>
        public ChangeTracker<int> TimeoutMs { get; }

        /// <summary>
        /// Код ошибки последнего выполнения
        /// </summary>
        public ChangeTracker<PingErrorCode?> LastRunErrorCode { get; }

    }
}