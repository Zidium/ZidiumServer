using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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

               });
        }
    }
}
