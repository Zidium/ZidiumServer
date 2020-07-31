using System;

namespace Zidium.Storage
{
    /// <summary>
    /// Контакт пользователя
    /// </summary>
    public class UserContactForUpdate
    {
        public UserContactForUpdate(Guid id)
        {
            Id = id;
            Type = new ChangeTracker<UserContactType>();
            Value = new ChangeTracker<string>();
        }

        /// <summary>
        /// Id
        /// </summary>
        public Guid Id { get; }

        /// <summary>
        /// Тип контакта
        /// </summary>
        public ChangeTracker<UserContactType> Type { get; }

        /// <summary>
        /// Значение контакта
        /// </summary>
        public ChangeTracker<string> Value { get; }

    }
}