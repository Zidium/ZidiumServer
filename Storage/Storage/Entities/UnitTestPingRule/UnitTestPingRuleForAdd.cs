using System;

namespace Zidium.Storage
{
    public class UnitTestPingRuleForAdd
    {
        public Guid UnitTestId;

        public string Host;

        /// <summary>
        /// Таймаут
        /// </summary>
        public int TimeoutMs;

        /// <summary>
        /// Код ошибки последнего выполнения
        /// </summary>
        public PingErrorCode? LastRunErrorCode;

    }
}
