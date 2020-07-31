using System;
using System.Collections.Generic;

namespace Zidium.Storage.Ef
{
    /// <summary>
    /// Сообщение лога
    /// </summary>
    public class DbLog
    {
        public DbLog()
        {
            Parameters = new HashSet<DbLogProperty>();    
        }

        /// <summary>
        /// ИД записи
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Ссылка на компонент
        /// </summary>
        public Guid ComponentId { get; set; }

        public virtual DbComponent Component { get; set; }

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

        public virtual ICollection<DbLogProperty> Parameters { get; set; }
    }
}
