using System;

namespace Zidium.Storage
{
    /// <summary>
    /// Тип компонента
    /// </summary>
    public class ComponentTypeForUpdate
    {
        public ComponentTypeForUpdate(Guid id)
        {
            Id = id;
            DisplayName = new ChangeTracker<string>();
            SystemName = new ChangeTracker<string>();
            IsDeleted = new ChangeTracker<bool>();
        }

        /// <summary>
        /// Id
        /// </summary>
        public Guid Id { get; }

        /// <summary>
        /// Название
        /// </summary>
        public ChangeTracker<string> DisplayName { get; }

        /// <summary>
        /// Системное название
        /// </summary>
        public ChangeTracker<string> SystemName { get; }

        /// <summary>
        /// Признак удалённого
        /// </summary>
        public ChangeTracker<bool> IsDeleted { get; }

    }
}