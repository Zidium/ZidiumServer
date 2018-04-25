using System;

namespace Zidium.Api
{
    /// <summary>
    /// Описывает данные события в запросе (DTO-объект)
    /// </summary>
    public class SendEventData
    {
        public SendEventData()
        {
            Properties = new ExtentionPropertyCollection();
        }

        public Guid ComponentId { get; set; }

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
        /// Для внутреннего применения
        /// </summary>
        public TimeSpan? JoinInterval{ get; set; }
        
        /// <summary>
        /// Категория события
        /// </summary>
        public SendEventCategory Category { get; set; }

        /// <summary>
        /// Версия компонента
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// Расширенные свойства
        /// </summary>
        public ExtentionPropertyCollection Properties { get; set; }
    }
}
