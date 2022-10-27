using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

            var services = new ServiceCollection();

            var appConfigurationBuilder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", false, false)
                .AddJsonFile("appsettings.prod.json", true, false);

            if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("ZIDIUM_CONFIG")))
                appConfigurationBuilder.AddJsonFile(Environment.GetEnvironmentVariable("ZIDIUM_CONFIG"), true, false);

            appConfigurationBuilder.AddEnvironmentVariables("ZIDIUM_");
            appConfigurationBuilder.AddUserSecrets(Assembly.GetEntryAssembly(), true);

            var appConfiguration = appConfigurationBuilder.Build();

            LogManager.Configuration = new NLogLoggingConfiguration(appConfiguration.GetSection("NLog"));

            services.AddSingleton<IConfiguration>(appConfiguration);
            services.AddLogging(o =>
            {
                o.AddConfiguration(appConfiguration.GetSection("Logging"));
                o.AddConsole();
                o.AddDebug();
                o.AddInternalLog();
                o.AddNLog();
            });

            var serviceProvider = services.BuildServiceProvider();
            var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
            DependencyInjection.SetLoggerFactory(loggerFactory);
            DependencyInjection.SetServicePersistent<InternalLoggerComponentMapping>(serviceProvider.GetRequiredService<InternalLoggerComponentMapping>());

            var application = new AgentService();
            application.Start(appConfiguration);

            bool runAsService = args.Any(x => string.Compare("--service", x, StringComparison.OrdinalIgnoreCase) == 0);

            if (runAsService)
            {
                Thread.Sleep(Timeout.Infinite);
            }
            else
            {
                Console.WriteLine("Press any key to stop...");
                Console.ReadKey();
                application.Stop();
            }

            LogManager.Shutdown();
        }
    }
}
