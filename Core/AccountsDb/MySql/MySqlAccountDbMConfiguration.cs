using System.Data.Entity.Migrations;
using Zidium.Core.Common;

namespace Zidium.Core.AccountsDb.Migrations.MySql
{
    public class MySqlAccountDbMConfiguration : DbMigrationsConfiguration<MySqlAccountDbContext>
    {
        public MySqlAccountDbMConfiguration()
        {
            AutomaticMigrationsEnabled = true;
            MigrationsDirectory = @"AccountsDb\MySql\Migrations";
            CodeGenerator = new NonClusteredPrimaryKeyCSharpMigrationCodeGenerator();
            SetSqlGenerator("System.Data.SqlClient", new NonClusteredPrimaryKeyMySqlMigrationSqlGenerator());
            CommandTimeout = 600; // 10 минут
            AutomaticMigrationDataLossAllowed = true;
        }
    }
}
