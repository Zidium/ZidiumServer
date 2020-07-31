using System;

namespace Zidium.Storage.Ef
{
    /// <summary>
    /// Настройка логера
    /// </summary>
    public class DbLogConfig
    {
        public Guid ComponentId { get; set; }

        public DateTime LastUpdateDate { get; set; }

        public virtual DbComponent Component { get; set; }

        public bool Enabled { get; set; }

        public bool IsDebugEnabled { get; set; }

        public bool IsTraceEnabled { get; set; }

        public bool IsInfoEnabled { get; set; }

        public bool IsWarningEnabled { get; set; }

        public bool IsErrorEnabled { get; set; }

        public bool IsFatalEnabled { get; set; }
    }
}
