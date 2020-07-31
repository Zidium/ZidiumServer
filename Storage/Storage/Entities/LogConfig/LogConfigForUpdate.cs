using System;

namespace Zidium.Storage
{
    /// <summary>
    /// Настройка логера
    /// </summary>
    public class LogConfigForUpdate
    {
        public LogConfigForUpdate(Guid componentId)
        {
            ComponentId = componentId;
            LastUpdateDate = new ChangeTracker<DateTime>();
            Enabled = new ChangeTracker<bool>();
            IsDebugEnabled = new ChangeTracker<bool>();
            IsTraceEnabled = new ChangeTracker<bool>();
            IsInfoEnabled = new ChangeTracker<bool>();
            IsWarningEnabled = new ChangeTracker<bool>();
            IsErrorEnabled = new ChangeTracker<bool>();
            IsFatalEnabled = new ChangeTracker<bool>();
        }

        public Guid ComponentId { get; }

        public ChangeTracker<DateTime> LastUpdateDate { get; }

        public ChangeTracker<bool> Enabled { get; }

        public ChangeTracker<bool> IsDebugEnabled { get; }

        public ChangeTracker<bool> IsTraceEnabled { get; }

        public ChangeTracker<bool> IsInfoEnabled { get; }

        public ChangeTracker<bool> IsWarningEnabled { get; }

        public ChangeTracker<bool> IsErrorEnabled { get; }

        public ChangeTracker<bool> IsFatalEnabled { get; }

    }
}