using System;

namespace Zidium.Core.Api
{
    /// <summary>
    /// Настройка лога
    /// </summary>
    public class WebLogConfig
    {
        public Guid ComponentId { get; set; }

        /// <summary>
        /// Дата и время последнего обновления конфига
        /// </summary>
        public DateTime LastUpdateDate { get; set; }

        /// <summary>
        /// Если True - запись в лог будет выполняться, False - не будет.
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// Нужно ли записывать сообщения уровня Debug
        /// </summary>
        public bool IsDebugEnabled { get; set; }

        /// <summary>
        /// Нужно ли записывать сообщения уровня Trace
        /// </summary>
        public bool IsTraceEnabled { get; set; }

        /// <summary>
        /// Нужно ли записывать сообщения уровня Info
        /// </summary>
        public bool IsInfoEnabled { get; set; }

        /// <summary>
        /// Нужно ли записывать сообщения уровня Warning
        /// </summary>
        public bool IsWarningEnabled { get; set; }

        /// <summary>
        /// Нужно ли записывать сообщения уровня Error
        /// </summary>
        public bool IsErrorEnabled { get; set; }

        /// <summary>
        /// Нужно ли записывать сообщения уровня Fatal
        /// </summary>
        public bool IsFatalEnabled { get; set; }
    }
}
