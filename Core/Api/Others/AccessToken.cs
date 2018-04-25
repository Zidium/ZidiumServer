using System;

namespace Zidium.Core.Api
{
    /// <summary>
    /// Информация для проверки прав доступа
    /// </summary>
    public class AccessToken
    {
        /// <summary>
        /// ИД аккаунта
        /// </summary>
        public Guid? AccountId { get; set; }

        public string AccountName { get; set; }

        /// <summary>
        /// Секретный ключ аккаунта для доступа к API
        /// </summary>
        public string SecretKey { get; set; }

        /// <summary>
        /// Если False - значит это запрос от АПИ пользователя, если True - значит это запрос от компонентов АПП
        /// </summary>
        public bool IsLocalRequest { get; set; }

        public string ProgramName { get; set; }

    }
}
