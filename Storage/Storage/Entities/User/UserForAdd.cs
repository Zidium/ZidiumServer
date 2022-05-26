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
        /// Email
        /// </summary>
        public string EMail;

        /// <summary>
        /// Хэш пароля
        /// </summary>
        public string PasswordHash;

        /// <summary>
        /// Отображаемое имя пользователя в ЛК
        /// </summary>
        public string DisplayName;

        /// <summary>
        /// Дата создания
        /// </summary>
        public DateTime CreateDate;

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
