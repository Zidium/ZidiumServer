using System;

namespace Zidium.Storage
{
    /// <summary>
    /// Контакт пользователя
    /// </summary>
    public class UserContactForRead
    {
        public UserContactForRead(Guid id, Guid userId, UserContactType type, string value, DateTime date)
        {
            Id = id;
            UserId = userId;
            Type = type;
            Value = value;
            CreateDate = date;
        }

        /// <summary>
        /// Id
        /// </summary>
        public Guid Id { get; }

        /// <summary>
        /// Ссылка на пользователя
        /// </summary>
        public Guid UserId { get; }

        /// <summary>
        /// Тип контакта
        /// </summary>
        public UserContactType Type { get; }

        /// <summary>
        /// Значение контакта
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// Дата создания контакта
        /// </summary>
        public DateTime CreateDate { get; }

        public UserContactForUpdate GetForUpdate()
        {
            return new UserContactForUpdate(Id);
        }

    }
}
