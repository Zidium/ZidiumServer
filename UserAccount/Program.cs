using System;
using System.IO;
using System.Reflection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using NLog.Web;
using Zidium.Api;

namespace Zidium.UserAccount
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .ConfigureAppSettingsFile()
                .Build();

            var nLogConfiguration = new NLogLoggingConfiguration(configuration.GetSection("NLog"));

            var logger = NLogBuilder.ConfigureNLog(nLogConfiguration).GetCurrentClassLogger();

            try
            {
                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception exception)
            {
                Tools.HandleOutOfMemoryException(exception);
                logger.Error(exception);
                throw;
            }
            finally
            {
                // Залогируем остановку
                logger.Info("Stop");
                Client.Instance.Flush();

                // Сохраним кеш
                NLog.LogManager.Shutdown();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration(appConfiguration =>
                {
                    appConfiguration.ConfigureAppSettingsFile();
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.AddDebug();
                })
                .UseNLog();

        private static IConfigurationBuilder ConfigureAppSettingsFile(this IConfigurationBuilder appConfiguration)
        {
            appConfiguration.AddJsonFile("appsettings.json", false, false);
            appConfiguration.AddJsonFile("appsettings.prod.json", true, false);
            appConfiguration.AddUserSecrets(Assembly.GetEntryAssembly(), true);
            return appConfiguration;
        }
    }
}
