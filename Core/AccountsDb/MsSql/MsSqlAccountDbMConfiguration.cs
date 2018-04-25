using System.Data.Entity.Migrations;
using Zidium.Core.Common;

namespace Zidium.Core.AccountsDb.Migrations.MsSql
{
    public class MsSqlAccountDbMConfiguration : DbMigrationsConfiguration<MsSqlAccountDbContext>
    {
        public MsSqlAccountDbMConfiguration()
        {
            AutomaticMigrationsEnabled = false;
            MigrationsDirectory = @"AccountsDb\MsSql\Migrations";
            CodeGenerator = new NonClusteredPrimaryKeyCSharpMigrationCodeGenerator();
            SetSqlGenerator("System.Data.SqlClient", new NonClusteredPrimaryKeySqlMigrationSqlGenerator());
            CommandTimeout = 600; // 10 минут
            AutomaticMigrationDataLossAllowed = true;
        }
    }
}
