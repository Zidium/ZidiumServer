using System;
using Zidium.Common;
using Zidium.Core;

namespace Zidium.DatabasesUpdate
{
    // Это приложение обновляет базу до последней версии
    public static class Program
    {
        public static void Main(string[] args)
        {
            var configuration = new Configuration();
            DependencyInjection.SetServicePersistent<IDebugConfiguration>(configuration);

            Console.WriteLine("Выберите среду для обновления баз:");
            Console.WriteLine("Work (W) - рабочая среда");
            Console.WriteLine("Test (T) - тестовая среда");

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
            DatabasesUpdate.UpdateAll(isTestEnviroment);

            Console.WriteLine("Press any key...");
            Console.ReadKey();
        }
    }
}
