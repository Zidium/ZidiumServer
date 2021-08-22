using System.Linq;
using System.Reflection.Emit;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Zidium.Storage.Ef.Sqlite;

namespace Zidium.Storage.Ef.MigrationRuntime
{
    public static class Program
    {
        public static void Main() { }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            var hostBuilder = Host.CreateDefaultBuilder(args);
            return hostBuilder
               .ConfigureServices((hostContext, services) =>
               {
                   var configuration = hostContext.Configuration;

                   services.AddDbContext<MsSqlAccountDbContext>(options =>
                   {
                       options.UseSqlServer(
                           configuration.GetConnectionString("MsSql"),
                           x => x.CommandTimeout(10000));
                   });

                   services.AddDbContext<PostgreSqlAccountDbContext>(options =>
                   {
                       options.UseNpgsql(
                           configuration.GetConnectionString("PostgreSql"),
                           x => x.CommandTimeout(10000));
                   });

                   services.AddDbContext<SqliteAccountDbContext>(options =>
                   {
                       var connectionString = configuration.GetConnectionString("Sqlite");
                       connectionString = SqliteConnectionStringHelper.SubstituteEnvVariables(connectionString);
                       var connection = new SqliteConnection(connectionString).AddUnicodeSupport();
                       options.UseSqlite(connection, x => x.CommandTimeout(10000));
                       SqliteAccountDbContext.OnModelCreatingOverride =
                           modelBuilder => modelBuilder.Model
                               .GetEntityTypes()
                               .SelectMany(t => t.GetProperties())
                               .Where(t => t.ClrType == typeof(string))
                               .ToList()
                               .ForEach(t => t.SetCollation("UTF8CI"));
                   });
               });
        }
    }
}
