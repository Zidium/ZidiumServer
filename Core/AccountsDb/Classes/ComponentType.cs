using System;
using System.Collections.Generic;

namespace Zidium.Core.AccountsDb
{
    /// <summary>
    /// Тип компонента
    /// </summary>
    public class ComponentType
    {
        /// <summary>
        /// Id
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Название
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Системное название
        /// </summary>
        public string SystemName { get; set; }

        /// <summary>
        /// Признак системного типа
        /// </summary>
        public bool IsSystem { get; set; }

        /// <summary>
        /// Признак удалённого
        /// </summary>
        public bool IsDeleted { get; set; }

        public virtual HashSet<Component> Components { get; set; }

        public ComponentType()
        {
            Components = new HashSet<Component>();
        }
    }
}
