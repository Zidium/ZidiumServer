using System;

namespace Zidium.Api.Dto
{
    /// <summary>
    /// Сообщение на регистрацию пользовательского типа компонента
    /// </summary>
    public class UpdateComponentTypeRequestDtoData
    {
        public Guid? Id { get; set; }

        public string SystemName { get; set; }

        public string DisplayName { get; set; }
    }
}
