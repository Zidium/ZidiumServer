namespace Zidium.UserAccount.Helpers
{
    public static class PermissionsHelper
    {
        /// <summary>
        /// Может управлять аккаунтом - изменять пользователей, тариф, проводить оплату и т.п.
        /// Может смотреть партнёрскую программу.
        /// </summary>
        public static bool CanManageAccount(this UserInfo user)
        {
            return user.IsAdmin();
        }

        /// <summary>
        /// Может изменять все данные. Может видеть секретный ключ аккаунта.
        /// </summary>
        public static bool CanEditCommonData(this UserInfo user)
        {
            return user.IsAdmin() || user.IsUser();
        }

        /// <summary>
        /// Может изменять личные данные - пользователь и подписки.
        /// </summary>
        public static bool CanEditPrivateData(this UserInfo user)
        {
            return user.IsAdmin() || user.IsUser() || user.IsViewer();
        }

    }
}