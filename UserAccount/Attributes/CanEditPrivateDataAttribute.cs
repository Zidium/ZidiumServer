namespace Zidium.UserAccount
{
    /// <summary>
    /// Может изменять личные данные - пользователь и подписки.
    /// </summary>
    public class CanEditPrivateDataAttribute : PermissionAttribute
    {
        public override bool CheckPermissions()
        {
            return UserHelper.CurrentUser.CanEditPrivateData();
        }
    }
}