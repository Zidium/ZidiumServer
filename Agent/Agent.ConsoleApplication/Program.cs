using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using Microsoft.Extensions.Configuration;
using NLog;
using NLog.Extensions.Logging;

namespace Zidium.Agent
{
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
