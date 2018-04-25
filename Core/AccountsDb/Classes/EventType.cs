using System;
using Zidium.Core.AccountsDb.Classes;
using Zidium.Core.Api;

namespace Zidium.Core.AccountsDb
{
    /// <summary>
    /// Тип события
    /// </summary>
    public class EventType
    {
        /// <summary>
        /// Id
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Категория
        /// </summary>
        public EventCategory Category { get; set; }

        /// <summary>
        /// Отображаемое название типа события
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Внутреннее название типа события
        /// </summary>
        public string SystemName { get; set; }

        /// <summary>
        /// Код типа события. Используется для ошибок.
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Интервал склейки в секундах. 0 - нельзя склеивать.
        /// </summary>
        public int? JoinIntervalSeconds { get; set; }

        /// <summary>
        /// Интервал склейки - обертка над свойством JoinIntervalSeconds
        /// </summary>
        public TimeSpan? JoinInterval
        {
            get { return JoinIntervalSeconds.HasValue ? TimeSpan.FromSeconds(JoinIntervalSeconds.Value) : (TimeSpan?)null; }
            set { JoinIntervalSeconds = value.HasValue ? (int)value.Value.TotalSeconds : (int?)null; }
        }

        /// <summary>
        /// Признак системного типа
        /// </summary>
        public bool IsSystem { get; set; }

        /// <summary>
        /// События с версиями, равными или меньше указанной, считаются старыми
        /// </summary>
        public string OldVersion { get; set; }

        /// <summary>
        /// Переопределение важности для старых событий
        /// </summary>
        public EventImportance? ImportanceForOld { get; set; }

        /// <summary>
        /// Переопределение важности для новых событий
        /// </summary>
        public EventImportance? ImportanceForNew { get; set; }
        
        /// <summary>
        /// Признак удаления
        /// </summary>
        public bool IsDeleted { get; set; }

        public DateTime? CreateDate { get; set; }

        public Guid? DefectId { get; set; }

        public virtual Defect Defect { get; set; }
    }
}
