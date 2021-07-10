using System;
using System.Collections.Generic;

namespace Zidium.Storage.Ef
{
    /// <summary>
    /// Пользователь
    /// </summary>
    public class DbUser
    {
        public DbUser()
        {
            UserContacts = new HashSet<DbUserContact>();
            Subscriptions = new HashSet<DbSubscription>();
            Roles = new HashSet<DbUserRole>();
            Settings = new HashSet<DbUserSetting>();
        }

        /// <summary>
        /// Id
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Логин
        /// </summary>
        public string Login { get; set; }

        /// <summary>
        /// Хэш пароля
        /// </summary>
        public string PasswordHash { get; set; }

        /// <summary>
        /// Имя
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Фамилия
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Отчество
        /// </summary>
        public string MiddleName { get; set; }

        /// <summary>
        /// Отображаемое имя пользователя в ЛК
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Дата создания
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// Должность
        /// </summary>
        public string Post { get; set; }

        /// <summary>
        /// В архиве?
        /// </summary>
        public bool InArchive { get; set; }

        /// <summary>
        /// Метка безопасности
        /// </summary>
        public string SecurityStamp { get; set; }

        /// <summary>
        /// Контакты
        /// </summary>
        public virtual HashSet<DbUserContact> UserContacts { get; set; }

        /// <summary>
        /// Подписки
        /// </summary>
        public virtual HashSet<DbSubscription> Subscriptions { get; set; }

        /// <summary>
        /// Роли пользователя
        /// </summary>
        public virtual HashSet<DbUserRole> Roles { get; set; }

        /// <summary>
        /// Настройки пользователя
        /// </summary>
        public virtual HashSet<DbUserSetting> Settings { get; set; }
        
    }
}
