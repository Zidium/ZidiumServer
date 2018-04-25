using System;

namespace Zidium.Core.Api
{
    /// <summary>
    /// Сообщение на регистрацию пользовательского типа компонента
    /// </summary>
    public class UpdateComponentTypeData
    {
        public Guid? Id { get; set; }

        public string SystemName { get; set; }

        public string DisplayName { get; set; }
    }
}
