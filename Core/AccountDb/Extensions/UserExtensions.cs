using Zidium.Storage;

namespace Zidium.Core.AccountsDb
{
    public static class UserExtensions
    {
        public static string FioOrLogin(this UserForAdd user)
        {
            if (user.LastName != null)
            {
                if (user.FirstName != null)
                {
                    return user.LastName + " " + user.FirstName;
                }
                return user.LastName;
            }
            return user.Login;
        }

        public static string FioOrLogin(this UserForRead user)
        {
            if (user.LastName != null)
            {
                if (user.FirstName != null)
                {
                    return user.LastName + " " + user.FirstName;
                }
                return user.LastName;
            }
            return user.Login;
        }
    }
}
