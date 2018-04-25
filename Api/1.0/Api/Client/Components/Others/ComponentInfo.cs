using System;
using Zidium.Api.Common;
using Zidium.Api.Dto;

namespace Zidium.Api
{
    /// <summary>
    /// Класс описывающий свойства компонента.
    /// Используется в IComponentControl
    /// </summary>
    public class ComponentInfo
    {
        public ComponentInfo(ComponentDto dto)
        {
            if (dto == null)
            {
                throw new ArgumentNullException("dto");
            }
            Id = dto.Id;
            ParentId = dto.ParentId;
            Type = new ComponentTypeInfo(dto.Type);
            SystemName = dto.SystemName;
            DisplayName = dto.DisplayName;
            CreatedDate = dto.CreatedDate;
            Version = dto.Version;
            Properties = DataConverter.GetExtentionPropertyCollection(dto.Properties);
        }

        public Guid Id { get; protected set; }

        public Guid? ParentId { get; protected set; }
        
        /// <summary>
        /// Тип компонента
        /// </summary>
        public ComponentTypeInfo Type { get; protected set; }

        public string SystemName { get; protected set; }

        public string DisplayName { get; protected set; }

        public DateTime CreatedDate { get; protected set; }

        public string Version { get; protected set; }

        /// <summary>
        /// Расширенные свойства компонента
        /// </summary>
        public ExtentionPropertyCollection Properties { get; protected set; }
    }
}
