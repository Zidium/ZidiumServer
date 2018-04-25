namespace Zidium.UserAccount
{
    /// <summary>
    /// Может изменять все данные. Может видеть секретный ключ аккаунта.
    /// </summary>
    public class CanEditAllDataAttribute : PermissionAttribute
    {
        public override bool CheckPermissions()
        {
            return UserHelper.CurrentUser.CanEditCommonData();
        }
    }
}