using Zidium.Core.ConfigDb;

namespace Zidium.Core.Common.Helpers
{
    public static class AccountTypeHelper
    {
        public static string GetDisplayName(AccountType type)
        {
            if (type == AccountType.Free)
            {
                return "Бесплатный";
            }
            if (type == AccountType.Paid)
            {
                return "Платный";
            }
            if (type == AccountType.System)
            {
                return "Системный";
            }
            if (type == AccountType.Test)
            {
                return "Тестовый";
            }
            return type.ToString();
        }
    }
}
