using System;
using System.Collections.Generic;
using System.Linq;

namespace Zidium.Core.AccountsDb
{
    /// <summary>
    /// Пользователь
    /// </summary>
    public class User
    {
        public User()
        {
            UserContacts = new HashSet<UserContact>();
            Subscriptions = new HashSet<Subscription>();
            Roles = new HashSet<UserRole>();
            Settings = new HashSet<UserSetting>();
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
        public virtual HashSet<UserContact> UserContacts { get; set; }

        /// <summary>
        /// Подписки
        /// </summary>
        public virtual HashSet<Subscription> Subscriptions { get; set; }

        /// <summary>
        /// Роли пользователя
        /// </summary>
        public virtual HashSet<UserRole> Roles { get; set; }

        /// <summary>
        /// Настройки пользователя
        /// </summary>
        public virtual HashSet<UserSetting> Settings { get; set; }
        

        public string FioOrLogin
        {
            get
            {
                if (LastName != null)
                {
                    if (FirstName != null)
                    {
                        return LastName + " " + FirstName;
                    }
                    return LastName;
                }
                return Login;
            }
        }

        public bool HasRole(Guid roleId)
        {
            return Roles.Any(x => x.RoleId == roleId);
        }

        public bool IsAccountAdmin()
        {
            return HasRole(RoleId.AccountAdministrators);
        }

        public bool IsUser()
        {
            return HasRole(RoleId.Users);
        }

        public bool IsViewer()
        {
            return HasRole(RoleId.Viewers);
        }

        //public string Get2CharsName()
        //{
        //    if (LastName != null && FirstName != null)
        //    {
        //        return "" + LastName[0] + FirstName[0];
        //    }
        //    return Login.Substring(0, 2);
        //}
    }
}
