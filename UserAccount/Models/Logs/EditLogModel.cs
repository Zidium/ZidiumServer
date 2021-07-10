using System;

namespace Zidium.UserAccount.Models
{
    public class EditLogModel
    {
        public Guid Id { get; set; }

        public bool IsDebugEnabled { get; set; }

        public bool IsTraceEnabled { get; set; }

        public bool IsInfoEnabled { get; set; }

        public bool IsWarningEnabled { get; set; }

        public bool IsErrorEnabled { get; set; }

        public bool IsFatalEnabled { get; set; }

        public string ComponentName { get; set; }
    }
}