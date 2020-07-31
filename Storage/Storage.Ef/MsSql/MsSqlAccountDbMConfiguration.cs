using System.Data.Entity.Migrations;

namespace Zidium.Storage.Ef.Migrations.MsSql
{
    internal class MsSqlAccountDbMConfiguration : DbMigrationsConfiguration<MsSqlAccountDbContext>
    {
        public MsSqlAccountDbMConfiguration()
        {
            ContextKey = "Zidium.Core.AccountsDb.Migrations.MsSql.MsSqlAccountDbMConfiguration";
            AutomaticMigrationsEnabled = false;
            MigrationsDirectory = @"MsSql\Migrations";
            CodeGenerator = new NonClusteredPrimaryKeyCSharpMigrationCodeGenerator();
            SetSqlGenerator("System.Data.SqlClient", new NonClusteredPrimaryKeySqlServerMigrationSqlGenerator());
            CommandTimeout = 600; // 10 минут
            AutomaticMigrationDataLossAllowed = true;
        }
    }
}
