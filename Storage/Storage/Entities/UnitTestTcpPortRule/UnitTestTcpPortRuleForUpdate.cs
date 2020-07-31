using System;

namespace Zidium.Storage
{
    /// <summary>
    /// Настройка проверки tcp-порта
    /// </summary>
    public class UnitTestTcpPortRuleForUpdate
    {
        public UnitTestTcpPortRuleForUpdate(Guid unitTestId)
        {
            UnitTestId = unitTestId;
            Host = new ChangeTracker<string>();
            TimeoutMs = new ChangeTracker<int>();
            Port = new ChangeTracker<int>();
            Opened = new ChangeTracker<bool>();
            LastRunErrorCode = new ChangeTracker<int?>();
        }

        public Guid UnitTestId { get; }

        public ChangeTracker<string> Host { get; }

        /// <summary>
        /// Таймаут
        /// </summary>
        public ChangeTracker<int> TimeoutMs { get; }

        public ChangeTracker<int> Port { get; }

        public ChangeTracker<bool> Opened { get; }

        /// <summary>
        /// Код ошибки последнего выполнения
        /// </summary>
        public ChangeTracker<int?> LastRunErrorCode { get; }

    }
}