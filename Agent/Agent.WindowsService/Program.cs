using System;
using System.IO;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Extensions.Logging;
using Zidium.Core.InternalLogger;

namespace Zidium.Agent
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            Directory.SetCurrentDirectory(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location));

            var appConfiguration = new ConfigurationBuilder()
                .ConfigureAppSettingsFile()
                .Build();

            LogManager.Configuration = new NLogLoggingConfiguration(appConfiguration.GetSection("NLog"));

            if (args != null && args.Length == 1 && args[0].Length > 1 && (args[0][0] == '-' || args[0][0] == '/'))
            {
                switch (args[0].Substring(1).ToLower())
                {
                    case "install":
                    case "i":
                        Install(true);
                        break;
                    case "uninstall":
                    case "u":
                        Install(false);
                        break;
                    default:
                        Console.WriteLine("Неправильные параметры");
                        break;
                }
                return;
            }

            CreateHostBuilder(args).Build().Run();

            LogManager.Shutdown();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .UseWindowsService()
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<Worker>();
                })
                .ConfigureAppConfiguration(app =>
                {
                    app.ConfigureAppSettingsFile();
                })
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.AddConsole();
                    logging.AddDebug();
                    logging.AddInternalLog();
                    logging.AddNLog();
                });
        }

        private static void Install(bool isInstall)
        {
            try
            {
                var appConfiguration = new ConfigurationBuilder()
                    .ConfigureAppSettingsFile()
                    .Build();

                var configuration = new WindowsServiceConfiguration(appConfiguration);

                var appAssembly = Assembly.GetEntryAssembly();
                var fileName = Path.ChangeExtension(appAssembly.Location, "exe");

                using (var installer = new ProjectInstaller(configuration.ServiceName, configuration.ServiceDescription, fileName))
                {
                    if (isInstall)
                    {
                        installer.Install();
                    }
                    else
                    {
                        installer.Uninstall();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private static IConfigurationBuilder ConfigureAppSettingsFile(this IConfigurationBuilder appConfiguration)
        {
            appConfiguration.AddJsonFile("appsettings.json", false, false);
            appConfiguration.AddJsonFile("appsettings.prod.json", true, false);
            if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("ZIDIUM_CONFIG")))
                appConfiguration.AddJsonFile(Environment.GetEnvironmentVariable("ZIDIUM_CONFIG"), true, false);
            appConfiguration.AddEnvironmentVariables("ZIDIUM_");
            appConfiguration.AddUserSecrets(Assembly.GetEntryAssembly(), true);
            return appConfiguration;
        }
    }
}
