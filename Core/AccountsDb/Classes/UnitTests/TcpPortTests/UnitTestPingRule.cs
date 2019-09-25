using System;

namespace Zidium.Core.AccountsDb
{
    /// <summary>
    /// Настройка проверки через команду ping
    /// </summary>
    public class UnitTestTcpPortRule
    {
        public Guid UnitTestId { get; set; }

        public virtual UnitTest UnitTest { get; set; }

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
