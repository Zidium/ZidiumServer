using System;

namespace Zidium.Storage
{
    /// <summary>
    /// Пользователь
    /// </summary>
    public class UserForUpdate
    {
        public UserForUpdate(Guid id)
        {
            Id = id;
            Login = new ChangeTracker<string>();
            EMail = new ChangeTracker<string>();
            PasswordHash = new ChangeTracker<string>();
            DisplayName = new ChangeTracker<string>();
            InArchive = new ChangeTracker<bool>();
            SecurityStamp = new ChangeTracker<string>();
        }

        /// <summary>
        /// Id
        /// </summary>
        public Guid Id { get; }

        /// <summary>
        /// Логин
        /// </summary>
        public ChangeTracker<string> Login { get; }

        /// <summary>
        /// EMail
        /// </summary>
        public ChangeTracker<string> EMail { get; }

        /// <summary>
        /// Хэш пароля
        /// </summary>
        public ChangeTracker<string> PasswordHash { get; }

        /// <summary>
        /// Отображаемое имя пользователя в ЛК
        /// </summary>
        public ChangeTracker<string> DisplayName { get; }

        /// <summary>
        /// В архиве?
        /// </summary>
        public ChangeTracker<bool> InArchive { get; }

        /// <summary>
        /// Метка безопасности
        /// </summary>
        public ChangeTracker<string> SecurityStamp { get; }

    }
}