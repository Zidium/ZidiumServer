using System;

namespace Zidium.Storage
{
    public class UnitTestTcpPortRuleForAdd
    {
        public Guid UnitTestId;

        public string Host;

        /// <summary>
        /// Таймаут
        /// </summary>
        public int TimeoutMs;

        public int Port;

        public bool Opened;

        /// <summary>
        /// Код ошибки последнего выполнения
        /// </summary>
        public int? LastRunErrorCode;
    }
}
