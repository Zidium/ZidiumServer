using System;

namespace Zidium.Core.AccountsDb
{
    /// <summary>
    /// Контакт пользователя
    /// </summary>
    public class UserContact
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
        public virtual User User { get; set; }

        /// <summary>
        /// Дата создания контакта
        /// </summary>
        public DateTime CreateDate { get; set; }

        public UserContact() {}

        public UserContact(UserContactType type, string value)
        {
            Type = type;
            Value = value;
        }
    }
}
