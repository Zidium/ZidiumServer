using System;

namespace Zidium.Core.AccountsDb
{
    /// <summary>
    /// Настройка проверки через команду ping
    /// </summary>
    public class UnitTestPingRule
    {
        public Guid UnitTestId { get; set; }

        public virtual UnitTest UnitTest { get; set; }

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
