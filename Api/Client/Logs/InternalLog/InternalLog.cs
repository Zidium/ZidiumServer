using System;
using System.IO;
using Zidium.Api.Dto;
using Zidium.Api.Others;

namespace Zidium.Api.Logs
{
    public class InternalLog : IInternalLog
    {
        protected IClient Client { get; set; }

        public bool IsTraceEnabled
        {
            get
            {
                return Client.Config.Logs.InternalLog.MinLevel == LogLevel.Trace;
            }
        }

        public bool IsDebugEnabled
        {
            get
            {
                return Client.Config.Logs.InternalLog.MinLevel <= LogLevel.Debug;
            }
        }

        public InternalLog(IClient client)
        {
            if (client == null)
            {
                throw new ArgumentNullException("client");
            }
            Client = client;
            Init();
        }

        protected void Init()
        {
            try
            {
                if (Client.Config.Logs.InternalLog.DeleteOldFileOnStartup)
                {
                    var logPath = GetLogPath();
                    if (File.Exists(logPath))
                    {
                        File.Delete(logPath);
                        Debug("Старый файл системного лога удален");
                    }
                }
                Debug("Системный лог успешно создан");
            }
            catch (Exception exception)
            {
                Error("Ошибка при удалении старого файла системного лога", exception);
            }
        }

        public void Trace(string message)
        {
            WriteLog(LogLevel.Trace, message, null);
        }

        public void Debug(string message)
        {
            WriteLog(LogLevel.Debug, message, null);
        }

        public void Info(string message)
        {
            WriteLog(LogLevel.Info, message, null);
        }

        public void Warning(string message)
        {
            WriteLog(LogLevel.Warning, message, null);
        }

        public void Error(string message)
        {
            WriteLog(LogLevel.Error, message, null);
        }

        public void Error(string message, Exception exception)
        {
            WriteLog(LogLevel.Error, message, exception);
        }

        public void Fatal(string message)
        {
            WriteLog(LogLevel.Fatal, message, null);
        }

        public void Fatal(string message, Exception exception)
        {
            WriteLog(LogLevel.Fatal, message, exception);
        }

        protected string GetLogPath()
        {
            string pathTemplate = Client.Config.Logs.InternalLog.FilePath;
            string path = TemplateHelper.GetLogPath(pathTemplate, Client);
            return path;
        }

        protected virtual void WriteLog(LogLevel level, string message, Exception exception)
        {
            try
            {
                if (Client.Config.Logs.InternalLog.Disable)
                {
                    return;
                }
                if (level < Client.Config.Logs.InternalLog.MinLevel)
                {
                    return;
                }
                string path = GetLogPath();
                string dir = Path.GetDirectoryName(path);
                var encoding = Client.Config.Logs.InternalLog.Encoding;
                if (exception != null)
                {
                    message = message + Environment.NewLine + exception;
                }
                message =
                    DateTime.Now.ToString("yyyy.MM.dd HH:mm:ss")
                    + " " + level.ToString().PadRight(7)
                    + " " + message
                    + Environment.NewLine;

                lock (this)
                {
                    if (string.IsNullOrEmpty(dir) == false)
                    {
                        if (Directory.Exists(dir) == false)
                        {
                            Directory.CreateDirectory(dir);
                        }
                    }
                    FileHelper.AppendAllText(path, message, encoding);
                }
            }
            catch
            {
            }
        }

        public void TraceFormat(string messageTemplate, params object[] parameters)
        {
            string message = string.Format(messageTemplate, parameters);
            Trace(message);
        }

        public void DebugFormat(string messageTemplate, params object[] parameters)
        {
            string message = string.Format(messageTemplate, parameters);
            Debug(message);
        }

        public void InfoFormat(string messageTemplate, params object[] parameters)
        {
            string message = string.Format(messageTemplate, parameters);
            Info(message);
        }

        public void WarningFormat(string messageTemplate, params object[] parameters)
        {
            string message = string.Format(messageTemplate, parameters);
            Warning(message);
        }

        public void ErrorFormat(string messageTemplate, params object[] parameters)
        {
            string message = string.Format(messageTemplate, parameters);
            Error(message);
        }

        public void FatalFormat(string messageTemplate, params object[] parameters)
        {
            string message = string.Format(messageTemplate, parameters);
            Fatal(message);
        }
    }
}
