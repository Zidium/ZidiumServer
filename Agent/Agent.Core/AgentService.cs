using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Zidium.Agent.AgentTasks;
using Zidium.Agent.AgentTasks.ComponentStatuses;
using Zidium.Agent.AgentTasks.DeleteEvents;
using Zidium.Agent.AgentTasks.DeleteLogs;
using Zidium.Agent.AgentTasks.DeleteMetricHistory;
using Zidium.Agent.AgentTasks.Dummy;
using Zidium.Agent.AgentTasks.HttpRequests;
using Zidium.Agent.AgentTasks.Notifications;
using Zidium.Agent.AgentTasks.OutdatedEventsStatuses;
using Zidium.Agent.AgentTasks.OutdatedMetrics;
using Zidium.Agent.AgentTasks.OutdatedUnitTests;
using Zidium.Agent.AgentTasks.SendEMails;
using Zidium.Agent.AgentTasks.SendMessages;
using Zidium.Agent.AgentTasks.SendSms;
using Zidium.Agent.AgentTasks.UnitTests.VirusTotal;
using Zidium.Api;
using Zidium.Common;
using Zidium.Core;
using Zidium.Core.Common;
using Zidium.Core.Common.Helpers;
using Zidium.Storage;
using Zidium.Storage.Ef;

namespace Zidium.Agent
{
    public class AgentService
    {
        private List<AgentTaskBase> _agentTasks;
        protected CancellationTokenSource TokenSource;
        protected ILogger Logger;

        protected void InitMonitoring()
        {
            var client = SystemAccountHelper.GetInternalSystemClient();
            client.WaitUntilAvailable(TimeSpan.FromSeconds(60));

            // Создадим компонент
            var debugConfiguration = DependencyInjection.GetServicePersistent<IDebugConfiguration>();
            var folder = !debugConfiguration.DebugMode ? 
                client.GetRootComponentControl().GetOrCreateChildFolderControl("Zidium") : 
                client.GetRootComponentControl().GetOrCreateChildFolderControl("DEBUG");
            var componentType = client.GetOrCreateComponentTypeControl(!debugConfiguration.DebugMode ? "Agent" : DebugHelper.DebugComponentType);
            var componentControl = folder
                .GetOrCreateChildComponentControl(new GetOrCreateComponentData("Agent", componentType)
                {
                    DisplayName = "Агент",
                    Version = AgentHelper.GetVersion()
                });

            // Присвоим Id компонента по умолчанию, чтобы адаптер логирования мог его использовать
            Client.Instance = client;
            Client.Instance.Config.DefaultComponent.Id = componentControl.Info?.Id;

            Logger = DependencyInjection.GetLogger("Agent");
            Logger.LogInformation("Запуск, IsFake={0}", componentControl.IsFake());
            Logger.LogInformation("Version {0}", VersionHelper.GetProductVersion());

            AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;
        }

        private void CurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs unhandledExceptionEventArgs)
        {
            if (unhandledExceptionEventArgs.ExceptionObject is Exception exception)
                Logger.LogCritical(exception, exception.Message);
        }

        public bool Start(IConfiguration appConfiguration)
        {
            var configuration = new Configuration(appConfiguration);
            DependencyInjection.SetServicePersistent<IDebugConfiguration>(configuration);
            DependencyInjection.SetServicePersistent<IDatabaseConfiguration>(configuration);
            DependencyInjection.SetServicePersistent<IDispatcherConfiguration>(configuration);
            DependencyInjection.SetServicePersistent<IAccessConfiguration>(configuration);
            DependencyInjection.SetServicePersistent<IAgentConfiguration>(configuration);
            DependencyInjection.SetServicePersistent<IStorageFactory>(new StorageFactory());
            DependencyInjection.SetServicePersistent<ITimeService>(new TimeService());

            InitMonitoring();

            try
            {
                // создаем список задач
                var agentConfiguration = DependencyInjection.GetServicePersistent<IAgentConfiguration>();

                _agentTasks = !agentConfiguration.DummyMode ? new List<AgentTaskBase>()
                {
                    new CreateNotificationsTask(),
                    new EmailNotificationsTask(),
                    new SmsNotificationsTask(),
                    new HttpNotificationsTask(),
                    new MessangerNotificationsTask(),
                    new OutdatedMetricsTask(),
                    new SendEmailsTask(),
                    new SendSmsTask(),
                    new SendToTelegramTask(),
                    new SendToVKontakteTask(),
                    new DeleteLogsTask(),
                    new DeleteMetricHistoryTask(),
                    new DeleteCustomerEventsTask(),
                    new DeleteUnittestEventsTask(),
                    new DeleteMetricEventsTask(),
                    new DeleteComponentEventsTask(),
                    new ComponentStatusTask(),
                    new OutdatedEventsStatusesTask(),
                    new OutdatedUnitTestsTask(),
                    new HttpRequestsTask(),
                    new PingTask(),
                    new TcpPortTask(),
                    new SqlCheckTask(),
                    new DomainNamePaymentPeriodCheckTask(),
                    new SslCertificateExpirationDateCheckTask(),
                    new VirusTotalTask()
                } : new List<AgentTaskBase>()
                {
                    new DummyTask()
                };

                // Загрузим сборку с подключаемыми задачами
                /*
                var folder = Path.GetDirectoryName(Environment.GetCommandLineArgs()[0]);
                var assemblyFilename = Path.Combine(folder ?? Environment.CurrentDirectory, "Zidium.Agent.AddIn.dll");
                if (File.Exists(assemblyFilename))
                {
                    var addInAssembly = Assembly.LoadFile(assemblyFilename);
                    var startupType = addInAssembly.GetExportedTypes().FirstOrDefault(t => string.Equals("Startup", t.Name, StringComparison.OrdinalIgnoreCase));
                    if (startupType != null)
                    {
                        var method = startupType.GetMethod("AddTasks", BindingFlags.Static | BindingFlags.Public);
                        if (method != null)
                            method.Invoke(null, new object[] { _agentTasks, agentConfiguration.DummyMode });
                    }
                }

                LogManager.ReconfigExistingLoggers();
                */

                // запускаем задачи
                TokenSource = new CancellationTokenSource();
                foreach (var agentTask in _agentTasks)
                {
                    Logger.LogDebug("Запуск задачи " + agentTask.Name);
                    agentTask.Start(TokenSource.Token);
                    Logger.LogInformation("Задача " + agentTask.Name + " запущена");
                }
                Logger.LogInformation("Все задачи запущены");

                return true;
            }
            catch (Exception exception)
            {
                Logger.LogCritical(exception, exception.Message);
                return false;
            }
        }

        public void Stop()
        {
            // даем всем задачам команду останавливаться
            TokenSource.Cancel();

            // ждем завершения всех задач
            foreach (var agentTask in _agentTasks)
            {
                Logger.LogDebug("Остановка задачи " + agentTask.Name);
                agentTask.WaitForStop();
                Logger.LogInformation("Задача " + agentTask.Name + " остановлена");
            }
            Logger.LogInformation("Все задачи остановлены");

            Logger.LogInformation("Агент остановлен");
            Client.Instance.Flush();
        }
    }
}
