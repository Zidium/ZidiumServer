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
            PasswordHash = new ChangeTracker<string>();
            FirstName = new ChangeTracker<string>();
            LastName = new ChangeTracker<string>();
            MiddleName = new ChangeTracker<string>();
            DisplayName = new ChangeTracker<string>();
            Post = new ChangeTracker<string>();
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
        /// Хэш пароля
        /// </summary>
        public ChangeTracker<string> PasswordHash { get; }

        /// <summary>
        /// Имя
        /// </summary>
        public ChangeTracker<string> FirstName { get; }

        /// <summary>
        /// Фамилия
        /// </summary>
        public ChangeTracker<string> LastName { get; }

        /// <summary>
        /// Отчество
        /// </summary>
        public ChangeTracker<string> MiddleName { get; }

        /// <summary>
        /// Отображаемое имя пользователя в ЛК
        /// </summary>
        public ChangeTracker<string> DisplayName { get; }

        /// <summary>
        /// Должность
        /// </summary>
        public ChangeTracker<string> Post { get; }

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