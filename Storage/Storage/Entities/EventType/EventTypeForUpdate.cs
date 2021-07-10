using System;
using Zidium.Api.Dto;

namespace Zidium.Storage
{
    /// <summary>
    /// Тип события
    /// </summary>
    public class EventTypeForUpdate
    {
        public EventTypeForUpdate(Guid id)
        {
            Id = id;
            Category = new ChangeTracker<EventCategory>();
            DisplayName = new ChangeTracker<string>();
            SystemName = new ChangeTracker<string>();
            Code = new ChangeTracker<string>();
            JoinIntervalSeconds = new ChangeTracker<int?>();
            OldVersion = new ChangeTracker<string>();
            ImportanceForOld = new ChangeTracker<EventImportance?>();
            ImportanceForNew = new ChangeTracker<EventImportance?>();
            IsDeleted = new ChangeTracker<bool>();
            DefectId = new ChangeTracker<Guid?>();
        }

        /// <summary>
        /// Id
        /// </summary>
        public Guid Id { get; }

        /// <summary>
        /// Категория
        /// </summary>
        public ChangeTracker<EventCategory> Category { get; }

        /// <summary>
        /// Отображаемое название типа события
        /// </summary>
        public ChangeTracker<string> DisplayName { get; }

        /// <summary>
        /// Внутреннее название типа события
        /// </summary>
        public ChangeTracker<string> SystemName { get; }

        /// <summary>
        /// Код типа события. Используется для ошибок.
        /// </summary>
        public ChangeTracker<string> Code { get; }

        /// <summary>
        /// Интервал склейки в секундах. 0 - нельзя склеивать.
        /// </summary>
        public ChangeTracker<int?> JoinIntervalSeconds { get; }

        /// <summary>
        /// События с версиями, равными или меньше указанной, считаются старыми
        /// </summary>
        public ChangeTracker<string> OldVersion { get; }

        /// <summary>
        /// Переопределение важности для старых событий
        /// </summary>
        public ChangeTracker<EventImportance?> ImportanceForOld { get; }

        /// <summary>
        /// Переопределение важности для новых событий
        /// </summary>
        public ChangeTracker<EventImportance?> ImportanceForNew { get; }
        
        /// <summary>
        /// Признак удаления
        /// </summary>
        public ChangeTracker<bool> IsDeleted { get; }

        public ChangeTracker<Guid?> DefectId { get; }

    }
}