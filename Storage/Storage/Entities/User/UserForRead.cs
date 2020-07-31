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
            string passwordHash, 
            string firstName, 
            string lastName, 
            string middleName, 
            string displayName, 
            DateTime createDate, 
            string post, 
            bool inArchive, 
            string securityStamp)
        {
            Id = id;
            Login = login;
            PasswordHash = passwordHash;
            FirstName = firstName;
            LastName = lastName;
            MiddleName = middleName;
            DisplayName = displayName;
            CreateDate = createDate;
            Post = post;
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
        /// Хэш пароля
        /// </summary>
        public string PasswordHash { get; }

        /// <summary>
        /// Имя
        /// </summary>
        public string FirstName { get; }

        /// <summary>
        /// Фамилия
        /// </summary>
        public string LastName { get; }

        /// <summary>
        /// Отчество
        /// </summary>
        public string MiddleName { get; }

        /// <summary>
        /// Отображаемое имя пользователя в ЛК
        /// </summary>
        public string DisplayName { get; }

        /// <summary>
        /// Дата создания
        /// </summary>
        public DateTime CreateDate { get; }

        /// <summary>
        /// Должность
        /// </summary>
        public string Post { get; }

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
