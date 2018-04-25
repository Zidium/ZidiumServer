using System;
using System.Collections.Generic;

namespace Zidium.Core.Api
{
    /// <summary>
    /// Класс описывающий свойства компонента.
    /// Используется в основном в IComponent
    /// </summary>
    public class ComponentInfo
    {
        public Guid Id { get; set; }

        public Guid? ParentId { get; set; }
        
        /// <summary>
        /// Тип компонента
        /// </summary>
        public ComponentTypeInfo Type { get; set; }

        public string SystemName { get; set; }

        public string DisplayName { get; set; }

        public DateTime CreatedDate { get; set; }

        public string Version { get; set; }

        public bool IsRoot()
        {
             return ParentId == null; 
        }

        /// <summary>
        /// Расширенные свойства компонента
        /// </summary>
        public List<ExtentionPropertyDto> Properties { get; set; }
    }
}
