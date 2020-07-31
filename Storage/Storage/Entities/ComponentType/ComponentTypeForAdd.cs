using System;

namespace Zidium.Storage
{
    public class ComponentTypeForAdd
    {
        /// <summary>
        /// Id
        /// </summary>
        public Guid Id;

        /// <summary>
        /// Название
        /// </summary>
        public string DisplayName;

        /// <summary>
        /// Системное название
        /// </summary>
        public string SystemName;

        /// <summary>
        /// Признак системного типа
        /// </summary>
        public bool IsSystem;

        /// <summary>
        /// Признак удалённого
        /// </summary>
        public bool IsDeleted;
    }
}
