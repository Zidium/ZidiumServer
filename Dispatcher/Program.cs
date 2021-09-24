using System;
using System.IO;
using System.Reflection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using NLog.Web;
using Zidium.Api;
using Zidium.Core;
using Zidium.Core.InternalLogger;

namespace Zidium.Dispatcher
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
            var nLogLogger = NLogBuilder.ConfigureNLog(nLogConfiguration).GetCurrentClassLogger();

            try
            {
                var host = CreateHostBuilder(args).Build();
                var logger = host.Services.GetRequiredService<ILogger<Startup>>();

                var hostApplicationLifetime = host.Services.GetRequiredService<IHostApplicationLifetime>();
                hostApplicationLifetime.ApplicationStopping.Register(() =>
                {
                    logger.LogInformation("Stop");

                    // Остановим обработку запросов
                    WebHandlerMiddlewareBase.Stop();

                    // Сохраним кеш
                    Client.Instance.Flush();
                    DispatcherService.Wrapper.SaveCaches();
                    NLog.LogManager.Shutdown();
                });

                host.Run();

            }
            catch (Exception exception)
            {
                Tools.HandleOutOfMemoryException(exception);
                nLogLogger.Fatal(exception);
                throw;
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
                    logging.AddConsole();
                    logging.AddDebug();
                    logging.AddInternalLog();
                })
                .UseNLog();

        private static IConfigurationBuilder ConfigureAppSettingsFile(this IConfigurationBuilder appConfiguration)
        {
            appConfiguration.AddJsonFile("appsettings.json", false, false);
            appConfiguration.AddJsonFile("appsettings.prod.json", true, false);
            if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("ZIDIUM_CONFIG")))
            {
                appConfiguration.AddJsonFile(Environment.GetEnvironmentVariable("ZIDIUM_CONFIG"), true, false);
            }
            appConfiguration.AddEnvironmentVariables("ZIDIUM_");
            appConfiguration.AddUserSecrets(Assembly.GetEntryAssembly(), true);
            return appConfiguration;
        }
    }
}
