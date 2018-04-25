namespace Zidium.UserAccount
{
    /// <summary>
    /// Может управлять аккаунтом - изменять пользователей, тариф, проводить оплату и т.п.
    /// Может смотреть партнёрскую программу.
    /// </summary>
    public class CanManageAccountAttribute : PermissionAttribute
    {
        public override bool CheckPermissions()
        {
            return UserHelper.CurrentUser.CanManageAccount();
        }
    }
}