using System;

namespace Zidium.Core.AccountsDb
{
    public static class RoleId
    {
        /// <summary>
        /// ИД роли администратора аккаунта - управляет аккаунтом
        /// </summary>
        public static readonly Guid AccountAdministrators = new Guid("6AF40D54-80C3-43A2-AF31-AE2168B033CF");

        /// <summary>
        /// Пользователь. Как админ, только не имеет доступа к настройкам аккаунта и другим пользователям.
        /// </summary>
        public static readonly Guid Users = new Guid("7B87A950-4475-4573-A7C2-42881A93F916");

        /// <summary>
        /// Наблюдатель. Может только смотреть, ничего изменить не может, кроме своих подписок.
        /// </summary>
        public static readonly Guid Viewers = new Guid("3581197F-5C48-4166-ADB7-59DE831231EC");
    }
}
