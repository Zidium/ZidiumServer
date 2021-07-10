using System;

namespace Zidium.Api.Dto
{
    /// <summary>
    /// Описывает тип компонента
    /// </summary>
    public class ComponentTypeDto
    {
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

        public bool IsFolder { get; set; }

        public bool IsRoot { get; set; }
    }
}
