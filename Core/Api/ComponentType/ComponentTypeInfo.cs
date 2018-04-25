using System;

namespace Zidium.Core.Api
{
    /// <summary>
    /// Описывает тип компонента
    /// </summary>
    public class ComponentTypeInfo
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

        /// <summary>
        /// Системный тип?
        /// </summary>
        public bool IsSystem { get; set; }
    }
}
