using System.Data.Entity.Migrations;

namespace Zidium.Core.AccountsDb.Migrations.MySql
{
    public class MySqlAccountDbMConfiguration : DbMigrationsConfiguration<MySqlAccountDbContext>
    {
        public MySqlAccountDbMConfiguration()
        {
            AutomaticMigrationsEnabled = true;
            MigrationsDirectory = @"AccountsDb\MySql\Migrations";
        }
    }
}
