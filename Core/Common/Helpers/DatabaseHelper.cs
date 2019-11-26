using Zidium.Core.AccountsDb;
using Zidium.Core.Api;

namespace Zidium.Core
{
    public static class DatabaseHelper
    {
        public static void ValidateBroken(this DatabaseInfo database)
        {
            using (var context = AccountDbContext.CreateFromConnectionString(database.ConnectionString))
            {
                context.Check();
            }
        }

        /// <summary>
        /// Убирает некорректные символы из строки
        /// </summary>
        public static string FixStringSymbols(this string value)
        {
            if (value == null)
                return null;

            if (value.Contains("\x00"))
                return value.Replace("\x00", " ");

            return value;
        }
    }
}
