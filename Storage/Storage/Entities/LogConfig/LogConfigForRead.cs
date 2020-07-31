using System;

namespace Zidium.Storage
{
    /// <summary>
    /// Настройка логера
    /// </summary>
    public class LogConfigForRead
    {
        public LogConfigForRead(
            Guid componentId, 
            DateTime lastUpdateDate, 
            bool enabled, 
            bool isDebugEnabled, 
            bool isTraceEnabled, 
            bool isInfoEnabled, 
            bool isWarningEnabled, 
            bool isErrorEnabled, 
            bool isFatalEnabled)
        {
            ComponentId = componentId;
            LastUpdateDate = lastUpdateDate;
            Enabled = enabled;
            IsDebugEnabled = isDebugEnabled;
            IsTraceEnabled = isTraceEnabled;
            IsInfoEnabled = isInfoEnabled;
            IsWarningEnabled = isWarningEnabled;
            IsErrorEnabled = isErrorEnabled;
            IsFatalEnabled = isFatalEnabled;
        }

        public Guid ComponentId { get; }

        public DateTime LastUpdateDate { get; }

        public bool Enabled { get; }

        public bool IsDebugEnabled { get; }

        public bool IsTraceEnabled { get; }

        public bool IsInfoEnabled { get; }

        public bool IsWarningEnabled { get; }

        public bool IsErrorEnabled { get; }

        public bool IsFatalEnabled { get; }

        public LogConfigForUpdate GetForUpdate()
        {
            return new LogConfigForUpdate(ComponentId);
        }
    }
}
