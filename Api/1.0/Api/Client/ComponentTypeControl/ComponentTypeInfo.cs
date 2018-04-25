using System;
using Zidium.Api.Dto;

namespace Zidium.Api
{
    /// <summary>
    /// Описывает тип компонента
    /// </summary>
    public class ComponentTypeInfo //todo сделать все свойства только на чтение
    {
        public ComponentTypeInfo(ComponentTypeDto dto)
        {
            if (dto == null)
            {
                throw new ArgumentNullException("dto");
            }
            Id = dto.Id;
            SystemName = dto.SystemName;
            DisplayName = dto.DisplayName;
            IsSystem = dto.IsSystem;
        }

        public Guid Id { get; set; }

        /// <summary>
        /// Системное имя типа компонента. Например: System.Device.Router
        /// </summary>
        public string SystemName { get; set; }

        /// <summary>
        /// Дружелюбное имя типа компонента. Например: Роутер
        /// </summary>
        public string DisplayName { get; set; }

        public bool IsSystem { get; set; }

        public bool IsRoot()
        {
            return Id == SystemComponentType.Root.Id;
        }

        public bool IsFolder()
        {
            return Id == SystemComponentType.Folder.Id;
        }

        public bool IsComponent()
        {
            return Id != SystemComponentType.Folder.Id
                && Id != SystemComponentType.Root.Id;
        }
    }
}
