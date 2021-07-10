using System;

namespace Zidium.Storage
{
    public class UserContactForAdd
    {
        /// <summary>
        /// Id
        /// </summary>
        public Guid Id;

        /// <summary>
        /// Ссылка на пользователя
        /// </summary>
        public Guid UserId;

        /// <summary>
        /// Тип контакта
        /// </summary>
        public UserContactType Type;

        /// <summary>
        /// Значение контакта
        /// </summary>
        public string Value;

        /// <summary>
        /// Дата создания контакта
        /// </summary>
        public DateTime CreateDate;

    }
}
