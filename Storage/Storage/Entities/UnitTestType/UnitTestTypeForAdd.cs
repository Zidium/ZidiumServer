using System;
using Zidium.Api.Dto;

namespace Zidium.Storage
{
    public class UnitTestTypeForAdd
    {
        /// <summary>
        /// Id
        /// </summary>
        public Guid Id;

        /// <summary>
        /// Системное имя
        /// </summary>
        public string SystemName;

        /// <summary>
        /// Дружелюбное имя
        /// </summary>
        public string DisplayName;

        public DateTime? CreateDate;

        /// <summary>
        /// Признак удалённого
        /// </summary>
        public bool IsDeleted;

        /// <summary>
        /// Признак того, что тип системный (НЕ создан пользователем)
        /// </summary>
        public bool IsSystem;

        /// <summary>
        /// Цвет проверки, если нет сигнала
        /// </summary>
        public ObjectColor? NoSignalColor;

        /// <summary>
        /// Время актуальности проверки
        /// </summary>
        public int? ActualTimeSecs;
    }
}
