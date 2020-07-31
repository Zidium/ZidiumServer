using System;

namespace Zidium.Core.Api
{
    /// <summary>
    /// Сообщение на обновление пользовательского типа компонента
    /// </summary>
    public class UpdateComponentTypeRequestData
    {
        public Guid? Id { get; set; }

        public string SystemName { get; set; }

        public string DisplayName { get; set; }
    }
}
