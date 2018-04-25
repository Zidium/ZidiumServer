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
    }
}
