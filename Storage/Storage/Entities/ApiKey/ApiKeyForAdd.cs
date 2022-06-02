using System;

namespace Zidium.Storage
{
    /// <summary>
    /// Ключ доступа к Api
    /// </summary>
    public class ApiKeyForAdd
    {
        public Guid Id;

        public string Name;

        public string Value;

        public DateTime UpdatedAt;

        /// <summary>
        /// Пользователь, если ключ не общий
        /// </summary>
        public Guid? UserId;
    }
}
