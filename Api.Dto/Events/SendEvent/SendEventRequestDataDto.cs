using System;
using System.Collections.Generic;

namespace Zidium.Api.Dto
{
    /// <summary>
    /// Описывает данные события в запросе (DTO-объект)
    /// </summary>
    public class SendEventRequestDataDto
    {
        /// <summary>
        /// Id компонента
        /// </summary>
        public Guid? ComponentId { get; set; }

        /// <summary>
        /// Системное имя типа события
        /// </summary>
        public string TypeSystemName { get; set; }

        /// <summary>
        /// Отображаемое имя типа события
        /// </summary>
        public string TypeDisplayName { get; set; }

        /// <summary>
        /// Код события
        /// </summary>
        public string TypeCode { get; set; }

        /// <summary>
        /// Описание события
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Дата, когда событие началось
        /// </summary>
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// Количество случаев события
        /// </summary>
        public int? Count { get; set; }

        /// <summary>
        /// Важность
        /// </summary>
        public EventImportance? Importance { get; set; }

        /// <summary>
        /// Ключ для склеивания событий
        /// </summary>
        public long? JoinKey { get; set; }

        /// <summary>
        /// Интервал склейки в секундах
        /// </summary>
        public double? JoinIntervalSeconds { get; set; }
        
        /// <summary>
        /// Категория события
        /// </summary>
        public SendEventCategory? Category { get; set; }

        /// <summary>
        /// Версия компонента
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// Расширенные свойства
        /// </summary>
        public List<ExtentionPropertyDto> Properties { get; set; }
    }
}
