using System;
using Zidium.Api.Dto;

namespace Zidium.Storage
{
    public class EventTypeForAdd
    {
        /// <summary>
        /// Id
        /// </summary>
        public Guid Id;

        /// <summary>
        /// Категория
        /// </summary>
        public EventCategory Category;

        /// <summary>
        /// Отображаемое название типа события
        /// </summary>
        public string DisplayName;

        /// <summary>
        /// Внутреннее название типа события
        /// </summary>
        public string SystemName;

        /// <summary>
        /// Код типа события. Используется для ошибок.
        /// </summary>
        public string Code;

        /// <summary>
        /// Интервал склейки в секундах. 0 - нельзя склеивать.
        /// </summary>
        public int? JoinIntervalSeconds;

        /// <summary>
        /// Признак системного типа
        /// </summary>
        public bool IsSystem;

        /// <summary>
        /// События с версиями, равными или меньше указанной, считаются старыми
        /// </summary>
        public string OldVersion;

        /// <summary>
        /// Переопределение важности для старых событий
        /// </summary>
        public EventImportance? ImportanceForOld;

        /// <summary>
        /// Переопределение важности для новых событий
        /// </summary>
        public EventImportance? ImportanceForNew;

        /// <summary>
        /// Признак удаления
        /// </summary>
        public bool IsDeleted;

        public DateTime? CreateDate;

        public Guid? DefectId;
    }
}
