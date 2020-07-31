using System.Data.Entity.Migrations;

namespace Zidium.Storage.Ef.Migrations.PostgreSql
{
    internal class PostgreSqlAccountDbMConfiguration : DbMigrationsConfiguration<PostgreSqlAccountDbContext>
    {
        public PostgreSqlAccountDbMConfiguration()
        {
            ContextKey = "Zidium.Core.AccountsDb.Migrations.PostgreSql.PostgreSqlAccountDbMConfiguration";
            AutomaticMigrationsEnabled = false;
            MigrationsDirectory = @"PostgreSql\Migrations";
            CodeGenerator = new NonClusteredPrimaryKeyCSharpMigrationCodeGenerator();
            SetSqlGenerator("Npgsql", new NonClusteredPrimaryKeyPostgreSqlMigrationSqlGenerator());
            CommandTimeout = 600; // 10 минут
            AutomaticMigrationDataLossAllowed = true;
        }
    }
}
