using System;

namespace Zidium.Core.AccountsDb
{
    /// <summary>
    /// Настройка логера
    /// </summary>
    public class LogConfig
    {
        public Guid ComponentId { get; set; }

        public DateTime LastUpdateDate { get; set; }

        public virtual Component Component { get; set; }

        public bool Enabled { get; set; }

        public bool IsDebugEnabled { get; set; }

        public bool IsTraceEnabled { get; set; }

        public bool IsInfoEnabled { get; set; }

        public bool IsWarningEnabled { get; set; }

        public bool IsErrorEnabled { get; set; }

        public bool IsFatalEnabled { get; set; }
    }
}
