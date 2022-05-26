using Zidium.Storage;

namespace Zidium.Core.AccountsDb
{
    public static class UserExtensions
    {
        public static string NameOrLogin(this UserForAdd user)
        {
            if (!string.IsNullOrEmpty(user.DisplayName))
            {
                return user.DisplayName;
            }
            return user.Login;
        }

        public static string NameOrLogin(this UserForRead user)
        {
            if (!string.IsNullOrEmpty(user.DisplayName))
            {
                return user.DisplayName;
            }
            return user.Login;
        }
    }
}
