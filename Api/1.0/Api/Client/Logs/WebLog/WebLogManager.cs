using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Zidium.Api.Others;

namespace Zidium.Api
{
    public class WebLogManager : IWebLogManager
    {
        public string Name
        {
            get { return "web"; }
        }

        public Timer ProcessQueueTimer { get; protected set; }

        public Timer ReloadConfigTimer { get; protected set; }

        public IWebLogQueue LogQueue { get; set; }

        public ThreadTaskQueue TaskQueue { get; protected set; }

        protected List<IComponentControl> ReloadControls { get; set; }

        protected DateTime LastReloadConfigsDate { get; set; }

        protected readonly object ReloadConfigsSynchRoot = new object();

        protected int Order = 0;

        public bool Disabled
        {
            get { return Client.Config.Logs.WebLog.Disable; }
            set { Client.Config.Logs.WebLog.Disable = value; }
        }

        public WebLogManager(Client client)
        {
            if (client == null)
            {
                throw new ArgumentNullException("client");
            }
            ReloadControls = new List<IComponentControl>();
            Client = client;
            LogQueue = new WebLogQueue();
            TaskQueue = new ThreadTaskQueue(client.Config.Logs.WebLog.Threads);
        }

        protected Client Client { get; set; }

        protected virtual int GetMessageSize(WebLogMessage message)
        {
            int length = 40; // служебные поля componentId + дата + уровень
            length += StringHelper.GetLengthInMemory(message.Message);
            length += StringHelper.GetLengthInMemory(message.Context);
            if (message.Properties != null)
            {
                length += message.Properties.GetWebSize();
            }
            return length;
        }

        public void AddLogMessage(IComponentControl componentControl, LogMessage logMessage)
        {
            if (componentControl == null)
            {
                return;
            }
            if (logMessage == null)
            {
                return;
            }
            if (Disabled)
            {
                return;
            }
            var webLogs = new WebLogMessage()
            {
                Attemps = 0,
                ComponentControl = componentControl,
                CreateDate = DateTime.Now,
                Order = Order,
                Date = logMessage.Date,
                Level = logMessage.Level,
                Message = logMessage.Message,
                Context = logMessage.Context,
                Properties = logMessage.Properties
            };
            webLogs.Size = GetMessageSize(webLogs);
            LogQueue.Add(webLogs);
            Order++;

            OnAddLogMessage?.Invoke(componentControl, logMessage);
        }

        public long GetQueueSize()
        {
            return LogQueue.Bytes();
        }

        public int GetQueueCount()
        {
            return LogQueue.Count();
        }

        public void Flush()
        {
            lock (LogQueue.SynchRoot)
            {
                ProcessQueue(null);
                //return LogQueue.Count() == 0;
            }
            TaskQueue.WaitForAllTasksCompleted();
        }

        protected virtual void ProcessQueue(object state)
        {
            // если очередь переполнена, то почистим ее
            int maxSize = Client.Config.Logs.WebLog.QueueBytes;
            int currentSize = LogQueue.Bytes();
            if (currentSize > maxSize)
            {
                Client.InternalLog.Warning("Очищаем очередь веб-логов");
                LogQueue.ClearBySize(maxSize / 2); // очищаем половину очереди
                Client.InternalLog.Debug("Очередь веб-логов очищена");
            }

            if (Disabled)
            {
                return;
            }

            if (LogQueue.Count() == 0)
            {
                return;
            }

            // если все потоки заняты, то ждем следующей итерации
            if (TaskQueue.HasFreeThreads == false)
            {
                return;
            }
            if (TaskQueue.RunTasks > 0)
            {
                return;
            }

            // если канал оффлайн, то ничего не отправляем
            if (Client.CanSendData == false)
            {
                return;
            }

            // если не можем вычислить разницу по времени, значит канал дохлый
            if (Client.CanConvertToServerDate() == false)
            {
                return;
            }

            // разбиваем все логи по пачкам и отправляем
            int batchSize = Client.Config.Logs.WebLog.BatchBytes;
            lock (LogQueue.SynchRoot)
            {
                // todo отправляются ВСЕ логи которые сейчас в очереди, 
                // т.е. по факту в памяти будет логов = размер очереди * 2 (1 часть логов в тасках, 2-ая в очереди), 
                // надо подумать можно ли так много доставать из очереди
                while (true)
                {
                    // получим пачку логов для отправки
                    var batchLogs = LogQueue.GetBatchMessages(batchSize);
                    if (batchLogs.Count == 0)
                    {
                        Order = 0;
                        return;
                    }

                    // установим серверное время
                    foreach (var log in batchLogs)
                    {
                        if (log.Date == null)
                        {
                            log.Date = Client.ToServerTime(log.CreateDate);
                        }
                    }

                    // отправим
                    TaskQueue.Add(SendBatchLogs, batchLogs, "SendBatchLogs");
                }
            }
        }

        protected virtual void SendBatchLogs(List<WebLogMessage> webLogs)
        {
            // если канал оффлайн, то ничего не отправляем
            if (Client.CanSendData == false)
            {
                LogQueue.AddRange(webLogs);
                return;
            }

            Client.InternalLog.DebugFormat("SendBatchLogs {0}", webLogs.Count);

            var logs = webLogs.Select(webLog =>
            {
                var log = new SendLogData()
                {
                    ComponentId = webLog.ComponentControl.Info.Id,
                    Date = webLog.Date,
                    Order = webLog.Order,
                    Level = webLog.Level,
                    Message = webLog.Message,
                    Context = webLog.Context
                };
                log.Properties.CopyFrom(webLog.Properties);
                return log;
            }).ToArray();

            var now = DateTime.Now;
            var response = Client.ApiService.SendLogs(logs);

            Client.InternalLog.DebugFormat("ApiService.SendLogs, count: {0}, time: {1}", webLogs.Count, DateTime.Now - now);

            if (!response.Success)
            {
                Client.InternalLog.WarningFormat("Failed ApiService.SendLogs, code: {0}, error: {1}", response.Code, response.ErrorMessage);
                foreach (var webLog in webLogs)
                {
                    webLog.Attemps++;
                    webLog.LastAttempTime = now;
                }
                LogQueue.AddRange(webLogs);
            }
        }

        protected virtual Timer CreateTimer()
        {
            int period = (int)Client.Config.Logs.WebLog.SendPeriod.TotalSeconds;
            if (period < 1)
            {
                period = 1;
            }
            return new Timer(ProcessQueue, null, 0, period * 1000);
        }

        public void Start()
        {
            if (ProcessQueueTimer == null)
            {
                ProcessQueueTimer = CreateTimer();
            }

            if (ReloadConfigTimer == null)
            {
                ReloadConfigTimer = new Timer(ReloadConfigs, null, 1000, 1000);
            }
        }

        public void Stop()
        {
            if (ProcessQueueTimer != null)
            {
                ProcessQueueTimer.Dispose();
                ProcessQueueTimer = null;
            }
        }

        protected virtual void ReloadConfigs(object state)
        {
            try
            {
                if (Client.CanSendData == false)
                {
                    return;
                }
                var period = Client.Config.Logs.WebLog.ReloadConfigsPeriod;
                if (period == TimeSpan.Zero)
                {
                    return;
                }
                var now = DateTime.Now;
                if (LastReloadConfigsDate + period > now)
                {
                    return;
                }
                LastReloadConfigsDate = now;
                lock (ReloadConfigsSynchRoot)
                {
                    if (ReloadControls.Count == 0)
                    {
                        return;
                    }
                    var maxUpdateDate = ReloadControls.Max(x => x.WebLogConfig.LastUpdateDate);
                    var componentIds = ReloadControls.Select(x => x.Info.Id).ToList();
                    var newConfigs = Client.ApiService.GetChangedWebLogConfigs(maxUpdateDate, componentIds);
                    if (newConfigs.Success)
                    {
                        foreach (var config in newConfigs.Data)
                        {
                            // может быть 2 контрола с одинаковым ИД, из-за того, 
                            // что один получили (GetOrCreate) от родителя, а второго из папки родителя
                            var controls = ReloadControls.Where(x => x.Info.Id == config.ComponentId);
                            foreach (var control in controls)
                            {
                                var oldConfig = control.WebLogConfig;
                                oldConfig.Enabled = config.Enabled;
                                oldConfig.IsTraceEnabled = config.IsTraceEnabled;
                                oldConfig.IsDebugEnabled = config.IsDebugEnabled;
                                oldConfig.IsInfoEnabled = config.IsInfoEnabled;
                                oldConfig.IsWarningEnabled = config.IsWarningEnabled;
                                oldConfig.IsErrorEnabled = config.IsErrorEnabled;
                                oldConfig.IsFatalEnabled = config.IsFatalEnabled;
                                oldConfig.LastUpdateDate = config.LastUpdateDate;
                            }
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                Client.InternalLog.Error("Ошибка перезагрузки настроек веб-логов", exception);
            }
        }

        public void BeginReloadConfig(IComponentControl componentControl)
        {
            lock (ReloadConfigsSynchRoot)
            {
                var data = ReloadControls.FirstOrDefault(x => x == componentControl);
                if (data == null)
                {
                    ReloadControls.Add(componentControl);
                }
            }
        }

        public void EndReloadConfig(IComponentControl componentControl)
        {
            lock (ReloadConfigsSynchRoot)
            {
                ReloadControls.Remove(componentControl);
                if (componentControl.IsFake() == false)
                {
                    ReloadControls.RemoveAll(x => x.IsFake() == false && x.Info.Id == componentControl.Info.Id);
                }
            }
        }

        public event AddLogMessageDelegate OnAddLogMessage;

        public WebLogConfig GetLogConfig(string componentSystemName)
        {
            // используется только фейковым компонентом, ему можно все, а как он станет настоящим, 
            // то будут использоваться настройки из БД
            return new WebLogConfig()
            {
                Enabled = true,
                IsDebugEnabled = true,
                IsErrorEnabled = true,
                IsFatalEnabled = true,
                IsInfoEnabled = true,
                IsTraceEnabled = true,
                IsWarningEnabled = true,
                LastUpdateDate = DateTime.MinValue
            };
        }
    }
}
