using System;

namespace Zidium.Storage.Ef
{
    /// <summary>
    /// Настройка проверки через команду ping
    /// </summary>
    public class DbUnitTestPingRule
    {
        public Guid UnitTestId { get; set; }

        public virtual DbUnitTest UnitTest { get; set; }

        public string Host { get; set; }

        /// <summary>
        /// Таймаут
        /// </summary>
        public int TimeoutMs { get; set; }

        /// <summary>
        /// Код ошибки последнего выполнения
        /// </summary>
        public PingErrorCode? LastRunErrorCode { get; set; }

    }
}
