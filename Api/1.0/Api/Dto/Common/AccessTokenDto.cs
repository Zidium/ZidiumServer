using System;

namespace Zidium.Api.Dto
{
    /// <summary>
    /// Информация для проверки прав доступа
    /// </summary>
    public class AccessTokenDto
    {
        /// <summary>
        /// ИД аккаунта
        /// </summary>
        public Guid? AccountId { get; set; }

        /// <summary>
        /// Секретный ключ аккаунта для доступа к API
        /// </summary>
        public string SecretKey { get; set; }

        /// <summary>
        /// Имя программы, которая использует АПИ. Необязательный параметр.
        /// </summary>
        public string Program { get; set; }
    }
}
