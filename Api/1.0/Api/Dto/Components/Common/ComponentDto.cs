using System;
using System.Collections.Generic;

namespace Zidium.Api.Dto
{
    /// <summary>
    /// Класс описывающий свойства компонента.
    /// Используется в IComponentControl
    /// </summary>
    public class ComponentDto
    {
        public Guid Id { get; set; }

        public Guid? ParentId { get; set; }

        /// <summary>
        /// Тип компонента
        /// </summary>
        public ComponentTypeDto Type { get; set; }

        public string SystemName { get; set; }

        public string DisplayName { get; set; }

        public DateTime CreatedDate { get; set; }

        public string Version { get; set; }

        /// <summary>
        /// Расширенные свойства компонента
        /// </summary>
        public List<ExtentionPropertyDto> Properties { get; set; }
    }
}
