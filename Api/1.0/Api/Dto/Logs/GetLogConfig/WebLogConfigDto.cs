using System;

namespace Zidium.Api.Dto
{
    /// <summary>
    /// Настройка лога
    /// </summary>
    public class WebLogConfigDto
    {
        /// <summary>
        /// Дата и время последнего обновления настроек веб-лога
        /// </summary>
        public DateTime LastUpdateDate { get; set; }

        /// <summary>
        /// Если True - запись в лог будет выполняться, False - не будет.
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// Нужно ли записывать сообщения уровня Trace
        /// </summary>
        public bool IsTraceEnabled { get; set; }

        /// <summary>
        /// Нужно ли записывать сообщения уровня Debug
        /// </summary>
        public bool IsDebugEnabled { get; set; }

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

        /// <summary>
        /// Id компонента
        /// </summary>
        public Guid ComponentId { get; set; }

        ///// <summary>
        ///// Для внутреннего применения
        ///// </summary>
        //public double? ReloadPeriodSeconds { get; set; }
    }
}
