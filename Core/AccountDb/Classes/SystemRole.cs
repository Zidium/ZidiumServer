using System;

namespace Zidium.Core.AccountsDb
{
    public class SystemRole
    {
        public Guid Id;

        public string SystemName;

        public string DisplayName;

        /// <summary>
        /// ИД роли администратора аккаунта - управляет аккаунтом
        /// </summary>
        public static readonly SystemRole AccountAdministrators = new SystemRole()
        {
            Id = new Guid("6AF40D54-80C3-43A2-AF31-AE2168B033CF"),
            SystemName = "AccountAdministrators",
            DisplayName = "Администраторы аккаунта"
        };

        /// <summary>
        /// Пользователь. Как админ, только не имеет доступа к настройкам аккаунта и другим пользователям.
        /// </summary>
        public static readonly SystemRole Users = new SystemRole()
        {
            Id = new Guid("7B87A950-4475-4573-A7C2-42881A93F916"),
            SystemName = "Users",
            DisplayName = "Пользователи"
        };

        /// <summary>
        /// Наблюдатель. Может только смотреть, ничего изменить не может, кроме своих подписок.
        /// </summary>
        public static readonly SystemRole Viewers = new SystemRole()
        {
            Id = new Guid("3581197F-5C48-4166-ADB7-59DE831231EC"),
            SystemName = "Viewers",
            DisplayName = "Наблюдатели"
        };
    }
}
