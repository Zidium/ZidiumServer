using System;
using Zidium.Api.Dto;

namespace Zidium.Storage.Ef
{
    /// <summary>
    /// Тип события
    /// </summary>
    public class DbEventType
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

        public virtual DbDefect Defect { get; set; }
    }
}
