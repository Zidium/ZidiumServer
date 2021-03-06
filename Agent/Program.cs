﻿using System;
using System.IO;
using System.ServiceProcess;
using System.Reflection;
using Zidium.Core;
using Zidium.Common;

namespace Zidium.Agent
{
    internal static class Program
    {
        public static void Main(string[] args)
        {
            var configuration = new Configuration();
            DependencyInjection.SetServicePersistent<IDebugConfiguration>(configuration);
            DependencyInjection.SetServicePersistent<IDatabaseConfiguration>(configuration);
            DependencyInjection.SetServicePersistent<IDispatcherConfiguration>(configuration);
            DependencyInjection.SetServicePersistent<IAgentConfiguration>(configuration);

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
            }
            else
            {
                try
                {
                    var service = new AgentService();
                    if (Environment.UserInteractive)
                    {
                        Console.CancelKeyPress += (x, y) => service.Stop();
                        if (service.Start())
                        {
                            Console.WriteLine("Отправлена команда запуска, нажмите любую клавишу для выхода");
                            Console.ReadKey();
                            service.Stop();
                            Console.WriteLine("Отправлена команда остановки...");
                        }
                        else
                        {
                            Console.Read();
                        }
                    }
                    else
                    {
                        var servicesToRun = new ServiceBase[] { service };
                        ServiceBase.Run(servicesToRun);
                    }
                }                                                                     
                catch (IOException e)
                {
                    Console.WriteLine(e.Message + Environment.NewLine + e.StackTrace);
                    Console.Read();
                }
                catch (Exception e)
                {
                    Console.WriteLine("Невозможно запустить службу в режиме консоли" + Environment.NewLine + e.Message + Environment.NewLine + e.StackTrace);
                    Console.Read();
                }
            }
        }

        private static void Install(bool isInstall)
        {
            try
            {
                var appAssembly = Assembly.GetEntryAssembly();
                var agentConfiguration = DependencyInjection.GetServicePersistent<IAgentConfiguration>();

                using (var installer = new ProjectInstaller(agentConfiguration.ServiceName, agentConfiguration.ServiceDescription, appAssembly))
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
    }
}
