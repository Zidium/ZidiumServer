using System;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using NLog;
using NLog.Extensions.Logging;
using Zidium.Common;
using Zidium.Core;

namespace Zidium.DatabaseUpdater
{
    // Это приложение обновляет базу до последней версии
    public static class Program
    {
        public static void Main(string[] args)
        {
            var appConfiguration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", false, false)
                .AddJsonFile("appsettings.prod.json", true, false)
                .AddUserSecrets(Assembly.GetEntryAssembly(), true)
                .Build();

            LogManager.Configuration = new NLogLoggingConfiguration(appConfiguration.GetSection("NLog"));

            var configuration = new Configuration(appConfiguration);
            DependencyInjection.SetServicePersistent<IDebugConfiguration>(configuration);

            Console.WriteLine("Version " + VersionHelper.GetProductVersion());
            Console.WriteLine();

            Console.WriteLine("Выберите среду для обновления баз:");
            Console.WriteLine("Work (W) - среда для обычного использования");
            Console.WriteLine("Test (T) - среда для запуска юнит-тестов");

            var env = Console.ReadLine();

            IDatabaseConfiguration databaseConfiguration = null;
            var isTestEnviroment = false;

            if (string.Equals(env, "Work", StringComparison.OrdinalIgnoreCase) || string.Equals(env, "W", StringComparison.OrdinalIgnoreCase))
            {
                databaseConfiguration = configuration.WorkDatabase;
                isTestEnviroment = false;
            }
            else if (string.Equals(env, "Test", StringComparison.OrdinalIgnoreCase) || string.Equals(env, "T", StringComparison.OrdinalIgnoreCase))
            {
                databaseConfiguration = configuration.TestDatabase;
                isTestEnviroment = true;
            }

            if (databaseConfiguration == null)
            {
                Console.WriteLine("Неизвестная среда выполнения");
                Console.ReadKey();
                return;
            }

            DependencyInjection.SetServicePersistent<IDatabaseConfiguration>(databaseConfiguration);
            DatabaseUpdater.UpdateAll(isTestEnviroment);

            LogManager.Shutdown();
        }
    }
}
