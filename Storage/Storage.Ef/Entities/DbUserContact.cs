using System;

namespace Zidium.Storage.Ef
{
    /// <summary>
    /// Контакт пользователя
    /// </summary>
    public class DbUserContact
    {
        /// <summary>
        /// Id
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Ссылка на пользователя
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Тип контакта
        /// </summary>
        public UserContactType Type { get; set; }

        /// <summary>
        /// Значение контакта
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Пользователь
        /// </summary>
        public virtual DbUser User { get; set; }

        /// <summary>
        /// Дата создания контакта
        /// </summary>
        public DateTime CreateDate { get; set; }

    }
}
