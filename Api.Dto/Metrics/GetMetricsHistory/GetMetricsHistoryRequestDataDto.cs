using System;

namespace Zidium.Api.Dto
{
    public class GetMetricsHistoryRequestDataDto
    {
        /// <summary>
        /// Компонент
        /// </summary>
        public Guid? ComponentId { get; set; }

        /// <summary>
        /// Начало диапазона поиска
        /// </summary>
        public DateTime? From { get; set; }

        /// <summary>
        /// Конец диапазона поиска
        /// </summary>
        public DateTime? To { get; set; }

        /// <summary>
        /// Имя метрики
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Максимальное количество записей в ответе.
        /// Не может быть больше 1000.
        /// </summary>
        public int? MaxCount { get; set; }
    }
}
