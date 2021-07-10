using System;

namespace Zidium.Api
{
    /// <summary>
    /// Настройка лога
    /// </summary>
    public class WebLogConfig : LogConfig
    {
        public Guid ComponentId { get; set; }

        /// <summary>
        /// Последняя дата и время обновления конфига веб-лога
        /// </summary>
        public DateTime LastUpdateDate { get; set; }
    }
}
