using System;

namespace Zidium.Storage
{
    /// <summary>
    /// Тип события
    /// </summary>
    public class EventTypeForRead
    {
        public EventTypeForRead(
            Guid id, 
            EventCategory category, 
            string displayName, 
            string systemName, 
            string code, 
            int? joinIntervalSeconds, 
            bool isSystem, 
            string oldVersion, 
            EventImportance? importanceForOld, 
            EventImportance? importanceForNew, 
            bool isDeleted, 
            DateTime? createDate, 
            Guid? defectId)
        {
            Id = id;
            Category = category;
            DisplayName = displayName;
            SystemName = systemName;
            Code = code;
            JoinIntervalSeconds = joinIntervalSeconds;
            IsSystem = isSystem;
            OldVersion = oldVersion;
            ImportanceForOld = importanceForOld;
            ImportanceForNew = importanceForNew;
            IsDeleted = isDeleted;
            CreateDate = createDate;
            DefectId = defectId;
        }

        /// <summary>
        /// Id
        /// </summary>
        public Guid Id { get; }

        /// <summary>
        /// Категория
        /// </summary>
        public EventCategory Category { get; }

        /// <summary>
        /// Отображаемое название типа события
        /// </summary>
        public string DisplayName { get; }

        /// <summary>
        /// Внутреннее название типа события
        /// </summary>
        public string SystemName { get; }

        /// <summary>
        /// Код типа события. Используется для ошибок.
        /// </summary>
        public string Code { get; }

        /// <summary>
        /// Интервал склейки в секундах. 0 - нельзя склеивать.
        /// </summary>
        public int? JoinIntervalSeconds { get; }

        /// <summary>
        /// Признак системного типа
        /// </summary>
        public bool IsSystem { get; }

        /// <summary>
        /// События с версиями, равными или меньше указанной, считаются старыми
        /// </summary>
        public string OldVersion { get; }

        /// <summary>
        /// Переопределение важности для старых событий
        /// </summary>
        public EventImportance? ImportanceForOld { get; }

        /// <summary>
        /// Переопределение важности для новых событий
        /// </summary>
        public EventImportance? ImportanceForNew { get; }
        
        /// <summary>
        /// Признак удаления
        /// </summary>
        public bool IsDeleted { get; }

        public DateTime? CreateDate { get; }

        public Guid? DefectId { get; }

        public EventTypeForUpdate GetForUpdate()
        {
            return new EventTypeForUpdate(Id);
        }

    }
}
