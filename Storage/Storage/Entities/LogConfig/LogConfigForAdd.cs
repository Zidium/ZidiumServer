using System;

namespace Zidium.Storage
{
    public class LogConfigForAdd
    {
        public Guid ComponentId;

        public DateTime LastUpdateDate;

        public bool Enabled;

        public bool IsDebugEnabled;

        public bool IsTraceEnabled;

        public bool IsInfoEnabled;

        public bool IsWarningEnabled;

        public bool IsErrorEnabled;

        public bool IsFatalEnabled;
    }
}
