using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.ServiceProcess;
using System.Threading;
using NLog;
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
using Zidium.Agent.AgentTasks.SendSms;
using Zidium.Api;
using Zidium.Core;
using Zidium.Core.AccountsDb;
using Zidium.Core.Common.Helpers;
using Zidium.Core.ConfigDb;
using System.Linq;

namespace Zidium.Agent
{
    [DesignerCategory("Code")]
    public class AgentService : ServiceBase
    {
        private List<AgentTaskBase> _agentTasks;
        protected CancellationTokenSource TokenSource;
        protected IComponentControl ComponentControl;
        protected Timer ContextCountTimer;
        protected TimeSpan ContextCountTimerInterval = TimeSpan.FromMinutes(5);
        protected ILogger Logger;

        protected void InitMonitoring()
        {
            var client = SystemAccountHelper.GetInternalSystemClient();

            // Создадим компонент
            // Если запускаемся в отладке, то компонент будет не в корне, а в папке DEBUG
            var folder = !DebugHelper.IsDebugMode ? client.GetRootComponentControl() : client.GetRootComponentControl().GetOrCreateChildFolderControl("DEBUG");
            var componentType = client.GetOrCreateComponentTypeControl(!DebugHelper.IsDebugMode ? "Agent" : DebugHelper.DebugComponentType);
            ComponentControl = folder
                .GetOrCreateChildComponentControl(new GetOrCreateComponentData("Agent", componentType)
                {
                    DisplayName = "Агент",
                    Version = AgentHelper.GetVersion()
                });

            // Присвоим Id компонента по умолчанию, чтобы адаптер NLog мог его использовать
            Client.Instance = client;
            Client.Instance.Config.DefaultComponent.Id = ComponentControl.Info?.Id;

            Logger = LogManager.GetLogger("Agent");
            Logger.Info("Запуск, IsFake={0}", ComponentControl.IsFake());

            AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;
        }

        private void CurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs unhandledExceptionEventArgs)
        {
            if (unhandledExceptionEventArgs.ExceptionObject is Exception exception)
                Logger.Fatal(exception);
        }

        public bool Start()
        {
            // Приложение не должно накатывать миграции или создавать базы
            AccountDbContext.DisableMigrations();

            Initialization.SetServices();
            InitMonitoring();

            try
            {
                // запускаем периодический сбор данных о количестве используемых контекстов
                ContextCountTimer = new Timer(SaveContextsCount, null, 0, (int) ContextCountTimerInterval.TotalMilliseconds);

                // создаем список фоновых задач
                _agentTasks = !ServiceConfiguration.DummyMode ? new List<AgentTaskBase>()
                {
                    new CreateNotificationsTask(),
                    new EmailNotificationsTask(),
                    new SmsNotificationsTask(),
                    new HttpNotificationsTask(),
                    new OutdatedMetricsTask(),
                    new SendEmailsTask(),
                    new SendSmsTask(),
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
                } : new List<AgentTaskBase>()
                {
                    new DummyTask()
                };

                // Загрузим сборку с подключаемыми задачами
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
                            method.Invoke(null, new object[] {_agentTasks, ServiceConfiguration.DummyMode});
                    }
                }

                LogManager.ReconfigExistingLoggers();

                // запускаем задачи
                TokenSource = new CancellationTokenSource();
                foreach (var agentTask in _agentTasks)
                {
                    Logger.Debug("Запуск задачи " + agentTask.Name);
                    agentTask.Start(TokenSource.Token);
                    Logger.Info("Задача " + agentTask.Name + " запущена");
                }
                Logger.Info("Все задачи запущены");

                return true;
            }
            catch (Exception exception)
            {
                Logger.Fatal(exception);
                return false;
            }
        }

        public new void Stop()
        {
            ContextCountTimer.Dispose();

            // даем всем задачам команду останавливаться
            TokenSource.Cancel();

            // ждем завершения всех задач
            foreach (var agentTask in _agentTasks)
            {
                Logger.Debug("Остановка задачи " + agentTask.Name);
                agentTask.WaitForStop();
                Logger.Info("Задача " + agentTask.Name + " остановлена");
            }
            Logger.Info("Все задачи остановлены");

            // финальный сбор статистики
            SaveContextsCount(null);

            Logger.Info("Агент остановлен");
            Client.Instance.Flush();
        }

        protected override void OnStart(string[] args)
        {
            Start();
        }

        protected override void OnStop()
        {
            Stop();
        }

        protected void SaveContextsCount(object state)
        {
            var actualInterval = TimeSpan.FromHours(1);

            ComponentControl.SendMetric("Contexts.Account.Active", AccountDbContext.ActiveCount, actualInterval);
            ComponentControl.SendMetric("Contexts.Account.Max", AccountDbContext.MaxActiveCount, actualInterval);
        }
    }
}
