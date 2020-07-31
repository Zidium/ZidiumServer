using System;

namespace Zidium.Storage
{
    /// <summary>
    /// Тип проверки
    /// </summary>
    public class UnitTestTypeForRead
    {
        public UnitTestTypeForRead(
            Guid id, 
            string systemName, 
            string displayName, 
            DateTime? createDate, 
            bool isDeleted, 
            bool isSystem, 
            ObjectColor? noSignalColor, 
            int? actualTimeSecs)
        {
            Id = id;
            SystemName = systemName;
            DisplayName = displayName;
            CreateDate = createDate;
            IsDeleted = isDeleted;
            IsSystem = isSystem;
            NoSignalColor = noSignalColor;
            ActualTimeSecs = actualTimeSecs;
        }

        /// <summary>
        /// Id
        /// </summary>
        public Guid Id { get; }

        /// <summary>
        /// Системное имя
        /// </summary>
        public string SystemName { get; }

        /// <summary>
        /// Дружелюбное имя
        /// </summary>
        public string DisplayName { get; }

        public DateTime? CreateDate { get; }

        /// <summary>
        /// Признак удалённого
        /// </summary>
        public bool IsDeleted { get; }

        /// <summary>
        /// Признак того, что тип системный (НЕ создан пользователем)
        /// </summary>
        public bool IsSystem { get; }

        /// <summary>
        /// Цвет проверки, если нет сигнала
        /// </summary>
        public ObjectColor? NoSignalColor { get; }

        /// <summary>
        /// Время актуальности проверки
        /// </summary>
        public int? ActualTimeSecs { get; }

        public UnitTestTypeForUpdate GetForUpdate()
        {
            return new UnitTestTypeForUpdate(Id);
        }

    }
}
