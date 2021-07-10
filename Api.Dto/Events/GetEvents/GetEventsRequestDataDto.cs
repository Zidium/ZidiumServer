using System;

namespace Zidium.Api.Dto
{
    public class GetEventsRequestDataDto
    {
        /// <summary>
        /// Id владельца
        /// </summary>
        public Guid? OwnerId { get; set; }

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
        public EventImportance[] Importance { get; set; }

        /// <summary>
        /// Системное имя типа
        /// </summary>
        public string TypeSystemName { get; set; }

        /// <summary>
        /// Строка для поиска. Выполняется поиск событий, которые содержат указнную подстроку.
        /// </summary>
        public string SearchText { get; set; }

        /// <summary>
        /// Категория для поиска
        /// </summary>
        public EventCategory? Category { get; set; }

        /// <summary>
        /// Максимальное количество записей в ответе
        /// Не может быть больше 1000
        /// </summary>
        public int? MaxCount { get; set; }

    }
}
