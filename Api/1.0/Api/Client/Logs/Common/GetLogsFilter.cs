using System;
using System.Collections.Generic;

namespace Zidium.Api
{
    /// <summary>
    /// Параметры поиска лог сообщений
    /// </summary>
    public class GetLogsFilter
    {
        /// <summary>
        /// Начало диапазона поиска
        /// </summary>
        public DateTime? From { get; set; }

        /// <summary>
        /// Конец диапазона поиска
        /// </summary>
        public DateTime? To { get; set; }

        /// <summary>
        /// Уровни важности. Если не заполнено, то не учитывается в поиске.
        /// </summary>
        public List<LogLevel> Levels { get; set; }

        /// <summary>
        /// Контекст для поиска. Выполняется поиск сообщений, у которых контекст начинается с указанной строки
        /// </summary>
        public string Context { get; set; }

        /// <summary>
        /// Максимальное количество записей в ответе.
        /// Не может быть больше 1000.
        /// </summary>
        public int? MaxCount { get; set; }
    }
}
