using System.Data.Entity.Migrations;
using Zidium.Core.Common;

namespace Zidium.Core.AccountsDb.Migrations.PostgreSql
{
    public class PostgreSqlAccountDbMConfiguration : DbMigrationsConfiguration<PostgreSqlAccountDbContext>
    {
        public PostgreSqlAccountDbMConfiguration()
        {
            AutomaticMigrationsEnabled = false;
            MigrationsDirectory = @"AccountsDb\PostgreSql\Migrations";
            CodeGenerator = new NonClusteredPrimaryKeyCSharpMigrationCodeGenerator();
            SetSqlGenerator("Npgsql", new NonClusteredPrimaryKeyPostgreSqlMigrationSqlGenerator());
            CommandTimeout = 600; // 10 минут
            AutomaticMigrationDataLossAllowed = true;
        }
    }
}
