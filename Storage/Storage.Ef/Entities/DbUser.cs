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
        /// EMail
        /// </summary>
        public string EMail { get; set; }

        /// <summary>
        /// Хэш пароля
        /// </summary>
        public string PasswordHash { get; set; }

        /// <summary>
        /// Отображаемое имя пользователя в ЛК
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Дата создания
        /// </summary>
        public DateTime CreateDate { get; set; }

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
        /// Роли
        /// </summary>
        public virtual HashSet<DbUserRole> Roles { get; set; }

        /// <summary>
        /// Настройки
        /// </summary>
        public virtual HashSet<DbUserSetting> Settings { get; set; }

    }
}
