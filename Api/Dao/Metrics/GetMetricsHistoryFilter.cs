using System;

namespace Zidium.Api
{
    public class GetMetricsHistoryFilter
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
        /// Название метрики
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Максимальное количество записей в ответе.
        /// Не может быть больше 1000.
        /// </summary>
        public int? MaxCount { get; set; }
    }
}
