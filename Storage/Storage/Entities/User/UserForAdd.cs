using System;

namespace Zidium.Storage
{
    public class UserForAdd
    {
        /// <summary>
        /// Id
        /// </summary>
        public Guid Id;

        /// <summary>
        /// Логин
        /// </summary>
        public string Login;

        /// <summary>
        /// Хэш пароля
        /// </summary>
        public string PasswordHash;

        /// <summary>
        /// Имя
        /// </summary>
        public string FirstName;

        /// <summary>
        /// Фамилия
        /// </summary>
        public string LastName;

        /// <summary>
        /// Отчество
        /// </summary>
        public string MiddleName;

        /// <summary>
        /// Отображаемое имя пользователя в ЛК
        /// </summary>
        public string DisplayName;

        /// <summary>
        /// Дата создания
        /// </summary>
        public DateTime CreateDate;

        /// <summary>
        /// Должность
        /// </summary>
        public string Post;

        /// <summary>
        /// В архиве?
        /// </summary>
        public bool InArchive;

        /// <summary>
        /// Метка безопасности
        /// </summary>
        public string SecurityStamp;
    }
}
