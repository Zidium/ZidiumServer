using System;

namespace Zidium.Storage.Ef
{
    /// <summary>
    /// Настройка проверки через команду ping
    /// </summary>
    public class DbUnitTestTcpPortRule
    {
        public Guid UnitTestId { get; set; }

        public virtual DbUnitTest UnitTest { get; set; }

        public string Host { get; set; }

        /// <summary>
        /// Таймаут
        /// </summary>
        public int TimeoutMs { get; set; }

        public int Port { get; set; }

        public bool Opened { get; set; }

        /// <summary>
        /// Код ошибки последнего выполнения
        /// </summary>
        public int? LastRunErrorCode { get; set; }

    }
}
