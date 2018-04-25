using System;

namespace DatabasesUpdate
{
    // Это приложение обновляет базу данных до последней версии
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Выберите среду для обновления баз:");
            Console.WriteLine("Work (W) - рабочая среда");
            Console.WriteLine("Test (T) - тестовая среда");
            Console.WriteLine("Local (L) - локальная среда");

            var env = Console.ReadLine();

            string sectionName = null;
            var isTestEnviroment = false;

            if (string.Equals(env, "Work", StringComparison.OrdinalIgnoreCase) || string.Equals(env, "W", StringComparison.OrdinalIgnoreCase))
            {
                sectionName = "DbContextWork";
                isTestEnviroment = false;
            }                
            else if (string.Equals(env, "Test", StringComparison.OrdinalIgnoreCase) || string.Equals(env, "T", StringComparison.OrdinalIgnoreCase))
            {
                sectionName = "DbContextTest";
                isTestEnviroment = true;
            }
            else if (string.Equals(env, "Local", StringComparison.OrdinalIgnoreCase) || string.Equals(env, "L", StringComparison.OrdinalIgnoreCase))
            {
                sectionName = "DbContextLocal";
                isTestEnviroment = true;
            }

            if (sectionName == null)
            {
                Console.WriteLine("Неизвестная среда выполнения");
                Console.ReadKey();
                return;
            }

            DatabasesUpdate.UpdateAll(sectionName, isTestEnviroment);

            Console.WriteLine("Нажмите для выхода...");
            Console.ReadKey();
        }
    }
}
