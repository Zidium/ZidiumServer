using System;

namespace Zidium.Storage
{
    /// <summary>
    /// Тип компонента
    /// </summary>
    public class ComponentTypeForRead
    {
        public ComponentTypeForRead(Guid id, string displayName, string systemName, bool isSystem, bool isDeleted)
        {
            Id = id;
            DisplayName = displayName;
            SystemName = systemName;
            IsSystem = isSystem;
            IsDeleted = isDeleted;
        }

        /// <summary>
        /// Id
        /// </summary>
        public Guid Id { get; }

        /// <summary>
        /// Название
        /// </summary>
        public string DisplayName { get; }

        /// <summary>
        /// Системное название
        /// </summary>
        public string SystemName { get; }

        /// <summary>
        /// Признак системного типа
        /// </summary>
        public bool IsSystem { get; }

        /// <summary>
        /// Признак удалённого
        /// </summary>
        public bool IsDeleted { get; }

        public ComponentTypeForUpdate GetForUpdate()
        {
            return new ComponentTypeForUpdate(Id);
        }
    }
}
