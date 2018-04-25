using System;
using System.Collections.Generic;
using Zidium.Core.Api;

namespace Zidium.Core.AccountsDb
{
    /// <summary>
    /// Сообщение лог-а
    /// </summary>
    public class Log
    {
        public Log()
        {
            Parameters = new HashSet<LogProperty>();    
        }

        /// <summary>
        /// ИД записи
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Ссылка на компонент
        /// </summary>
        public Guid ComponentId { get; set; }

        public virtual Component Component { get; set; }

        /// <summary>
        /// Важность
        /// </summary>
        public LogLevel Level { get; set; }

        /// <summary>
        /// Дата и время
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Номер по порядку в пределах секунды
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        /// Сообщение
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Количество расширенных свойств
        /// </summary>
        public int ParametersCount { get; set; }

        /// <summary>
        /// Контекст
        /// </summary>
        public string Context { get; set; }

        public virtual ICollection<LogProperty> Parameters { get; set; }
    }
}
