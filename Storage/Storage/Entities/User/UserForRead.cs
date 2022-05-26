using System;

namespace Zidium.Storage
{
    /// <summary>
    /// Пользователь
    /// </summary>
    public class UserForRead
    {
        public UserForRead(
            Guid id,
            string login,
            string email,
            string passwordHash,
            string displayName,
            DateTime createDate,
            bool inArchive,
            string securityStamp)
        {
            Id = id;
            Login = login;
            Email = email;
            PasswordHash = passwordHash;
            DisplayName = displayName;
            CreateDate = createDate;
            InArchive = inArchive;
            SecurityStamp = securityStamp;
        }

        /// <summary>
        /// Id
        /// </summary>
        public Guid Id { get; }

        /// <summary>
        /// Логин
        /// </summary>
        public string Login { get; }

        /// <summary>
        /// EMail
        /// </summary>
        public string Email { get; }

        /// <summary>
        /// Хэш пароля
        /// </summary>
        public string PasswordHash { get; }

        /// <summary>
        /// Отображаемое имя пользователя в ЛК
        /// </summary>
        public string DisplayName { get; }

        /// <summary>
        /// Дата создания
        /// </summary>
        public DateTime CreateDate { get; }

        /// <summary>
        /// В архиве?
        /// </summary>
        public bool InArchive { get; }

        /// <summary>
        /// Метка безопасности
        /// </summary>
        public string SecurityStamp { get; }

        public UserForUpdate GetForUpdate()
        {
            return new UserForUpdate(Id);
        }

    }
}
