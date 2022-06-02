using System;

namespace Zidium.Storage.Ef
{
    /// <summary>
    /// Ключ доступа к Api
    /// </summary>
    public class DbApiKey
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Value { get; set; }

        public DateTime UpdatedAt { get; set; }

        /// <summary>
        /// Пользователь, если ключ не общий
        /// </summary>
        public Guid? UserId { get; set; }

        public virtual DbUser User { get; set; }
    }
}
