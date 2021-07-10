using System;
using System.Collections.Generic;
using Zidium.Api.Dto;

namespace Zidium.Api
{
    public class Log : ILog
    {
        protected IComponentControl ComponentControl { get; set; }

        public string Context { get; set; }

        public Log(IComponentControl componentControl)
        {
            if (componentControl == null)
            {
                throw new ArgumentNullException("componentControl");
            }

            ComponentControl = componentControl;
        }

        #region Configs

        public WebLogConfig WebLogConfig
        {
            get { return ComponentControl.WebLogConfig; }
        }

        #endregion

        protected void SendLogMessage(LogLevel level, string message, IDictionary<string, object> properties)
        {
            var client = ComponentControl.Client;
            AddLogMessage(client.WebLogManager, WebLogConfig, level, message, properties);
        }

        protected void AddLogMessage(
            ILogManager manager,
            LogConfig config,
            LogLevel level,
            string message,
            IDictionary<string, object> properties)
        {
            try
            {
                if (manager == null)
                {
                    return;
                }
                if (manager.Disabled)
                {
                    return;
                }
                if (config.Enabled == false)
                {
                    return;
                }
                if (level == LogLevel.Debug && config.IsDebugEnabled == false)
                {
                    return;
                }
                if (level == LogLevel.Trace && config.IsTraceEnabled == false)
                {
                    return;
                }
                if (level == LogLevel.Info && config.IsInfoEnabled == false)
                {
                    return;
                }
                if (level == LogLevel.Warning && config.IsWarningEnabled == false)
                {
                    return;
                }
                if (level == LogLevel.Error && config.IsErrorEnabled == false)
                {
                    return;
                }
                if (level == LogLevel.Fatal && config.IsFatalEnabled == false)
                {
                    return;
                }
                var mes = new LogMessage()
                {
                    Level = level,
                    Message = message
                };
                if (string.IsNullOrEmpty(Context) == false)
                {
                    mes.Context = Context;
                }
                if (properties != null)
                {
                    mes.Properties = new ExtentionPropertyCollection(properties);
                }
                manager.AddLogMessage(ComponentControl, mes);
            }
            catch (Exception exception)
            {
                var log = ((Client)ComponentControl.Client).InternalLog;
                log.Error("Ошибка добавления лога", exception);
            }
        }

        public void Flush()
        {
            ComponentControl.Client.WebLogManager.Flush();
        }

        public bool IsFake()
        {
            return false;
        }

        public ILog GetTaggedCopy(string tag)
        {
            return new Log(ComponentControl)
            {
                Context = tag
            };
        }

        public ILog AddTag(string tag)
        {
            if (Context == null)
            {
                return GetTaggedCopy(tag);
            }
            return GetTaggedCopy(Context + tag);
        }

        #region Trace

        public void Trace(string message)
        {
            Write(LogLevel.Trace, message);
        }

        public void Trace(string message, IDictionary<string, object> properties)
        {
            Write(LogLevel.Trace, message, properties);
        }

        public void Trace(string message, object properties)
        {
            Write(LogLevel.Trace, message, properties);
        }

        public void Trace(Exception exception)
        {
            Write(LogLevel.Trace, exception);
        }

        public void Trace(string message, Exception exception)
        {
            Write(LogLevel.Trace, message, exception);
        }

        public void Trace(string message, Exception exception, IDictionary<string, object> properties)
        {
            Write(LogLevel.Trace, message, exception, properties);
        }

        public void Trace(string message, Exception exception, object properties)
        {
            Write(LogLevel.Trace, message, exception, properties);
        }

        public void TraceFormat(string message, params object[] args)
        {
            WriteFormat(LogLevel.Trace, message, args);
        }

        public void TraceFormat(string message, IDictionary<string, object> properties, params object[] args)
        {
            WriteFormat(LogLevel.Trace, message, properties, args);
        }

        #endregion

        #region Debug

        public void Debug(string message)
        {
            Write(LogLevel.Debug, message);
        }

        public void Debug(string message, IDictionary<string, object> properties)
        {
            Write(LogLevel.Debug, message, properties);
        }

        public void Debug(string message, object properties)
        {
            Write(LogLevel.Debug, message, properties);
        }

        public void Debug(Exception exception)
        {
            Write(LogLevel.Debug, exception);
        }

        public void Debug(string message, Exception exception)
        {
            Write(LogLevel.Debug, message, exception);
        }

        public void Debug(string message, Exception exception, IDictionary<string, object> properties)
        {
            Write(LogLevel.Debug, message, exception, properties);
        }

        public void Debug(string message, Exception exception, object properties)
        {
            Write(LogLevel.Debug, message, exception, properties);
        }

        public void DebugFormat(string message, params object[] args)
        {
            WriteFormat(LogLevel.Debug, message, args);
        }

        public void DebugFormat(string message, IDictionary<string, object> properties, params object[] args)
        {
            WriteFormat(LogLevel.Debug, message, properties, args);
        }

        #endregion

        #region Info

        public void Info(string message)
        {
            Write(LogLevel.Info, message);
        }

        public void Info(string message, IDictionary<string, object> properties)
        {
            Write(LogLevel.Info, message, properties);
        }

        public void Info(string message, object properties)
        {
            Write(LogLevel.Info, message, properties);
        }

        public void Info(Exception exception)
        {
            Write(LogLevel.Info, exception);
        }

        public void Info(string message, Exception exception)
        {
            Write(LogLevel.Info, message, exception);
        }

        public void Info(string message, Exception exception, IDictionary<string, object> properties)
        {
            Write(LogLevel.Info, message, exception, properties);
        }

        public void Info(string message, Exception exception, object properties)
        {
            Write(LogLevel.Info, message, exception, properties);
        }

        public void InfoFormat(string message, params object[] args)
        {
            WriteFormat(LogLevel.Info, message, args);
        }

        public void InfoFormat(string message, IDictionary<string, object> properties, params object[] args)
        {
            WriteFormat(LogLevel.Info, message, properties, args);
        }

        #endregion

        #region Warning

        public void Warning(string message)
        {
            Write(LogLevel.Warning, message);
        }

        public void Warning(string message, IDictionary<string, object> properties)
        {
            Write(LogLevel.Warning, message, properties);
        }

        public void Warning(string message, object properties)
        {
            Write(LogLevel.Warning, message, properties);
        }

        public void Warning(Exception exception)
        {
            Write(LogLevel.Warning, exception);
        }

        public void Warning(string message, Exception exception)
        {
            Write(LogLevel.Warning, message, exception);
        }

        public void Warning(string message, Exception exception, IDictionary<string, object> properties)
        {
            Write(LogLevel.Warning, message, exception, properties);
        }

        public void Warning(string message, Exception exception, object properties)
        {
            Write(LogLevel.Warning, message, exception, properties);
        }

        public void WarningFormat(string message, params object[] args)
        {
            WriteFormat(LogLevel.Warning, message, args);
        }

        public void WarningFormat(string message, IDictionary<string, object> properties, params object[] args)
        {
            WriteFormat(LogLevel.Warning, message, properties, args);
        }

        #endregion

        #region Error

        public void Error(string message)
        {
            Write(LogLevel.Error, message);
        }

        public void Error(string message, IDictionary<string, object> properties)
        {
            Write(LogLevel.Error, message, properties);
        }

        public void Error(string message, object properties)
        {
            Write(LogLevel.Error, message, properties);
        }

        public void Error(Exception exception)
        {
            Write(LogLevel.Error, exception);
        }

        public void Error(string message, Exception exception)
        {
            Write(LogLevel.Error, message, exception);
        }

        public void Error(string message, Exception exception, IDictionary<string, object> properties)
        {
            Write(LogLevel.Error, message, exception, properties);
        }

        public void Error(string message, Exception exception, object properties)
        {
            Write(LogLevel.Error, message, exception, properties);
        }

        public void ErrorFormat(string message, params object[] args)
        {
            WriteFormat(LogLevel.Error, message, args);
        }

        public void ErrorFormat(string message, IDictionary<string, object> properties, params object[] args)
        {
            WriteFormat(LogLevel.Error, message, properties, args);
        }

        #endregion

        #region Fatal

        public void Fatal(string message)
        {
            Write(LogLevel.Fatal, message);
        }

        public void Fatal(string message, IDictionary<string, object> properties)
        {
            Write(LogLevel.Fatal, message, properties);
        }

        public void Fatal(string message, object properties)
        {
            Write(LogLevel.Fatal, message, properties);
        }

        public void Fatal(Exception exception)
        {
            Write(LogLevel.Fatal, exception);
        }

        public void Fatal(string message, Exception exception)
        {
            Write(LogLevel.Fatal, message, exception);
        }

        public void Fatal(string message, Exception exception, IDictionary<string, object> properties)
        {
            Write(LogLevel.Fatal, message, exception, properties);
        }

        public void Fatal(string message, Exception exception, object properties)
        {
            Write(LogLevel.Fatal, message, exception, properties);
        }

        public void FatalFormat(string message, params object[] args)
        {
            WriteFormat(LogLevel.Fatal, message, args);
        }

        public void FatalFormat(string message, IDictionary<string, object> properties, params object[] args)
        {
            WriteFormat(LogLevel.Fatal, message, properties, args);
        }

        #endregion

        #region Write

        public void Write(LogLevel level, string message)
        {
            Write(level, message, null, null);
        }

        public void Write(LogLevel level, string message, IDictionary<string, object> properties)
        {
            Write(level, message, null, properties);
        }

        public void Write(LogLevel level, string message, object properties)
        {
            var dictionary = Tools.GetPropertyCollection(properties);
            Write(level, message, null, dictionary);
        }

        public void Write(LogLevel level, Exception exception)
        {
            Write(level, exception.Message, exception, null);
        }

        public void Write(LogLevel level, string message, Exception exception)
        {
            Write(level, message, exception, null);
        }

        public void Write(LogLevel level, string message, Exception exception, IDictionary<string, object> properties)
        {
            if (properties == null)
            {
                properties = new Dictionary<string, object>();
            }

            FillPropertiesFromException(exception, properties);

            var fullMessage = message;
            if (exception != null)
            {
                var exceptionMessage = ComponentControl.Client.ExceptionRender.GetMessage(exception);
                if (exceptionMessage != message)
                    fullMessage += " : " + exceptionMessage;
            }

            SendLogMessage(level, fullMessage, properties);

            // Автосоздание события
            if (!ComponentControl.Client.Config.Logs.AutoCreateEvents.Disable)
            {
                if (exception == null)
                {
                    // Без exception превращаем в события только записи уровня Warning и выше
                    if (level <= LogLevel.Info)
                        return;
                }

                var data = ComponentControl.Client.ExceptionRender.CreateEventFromLog(ComponentControl, level, exception, message, properties);
                data.Add();
            }
        }

        public void Write(LogLevel level, string message, Exception exception, object properties)
        {
            var dictionary = Tools.GetPropertyCollection(properties);
            Write(level, message, exception, dictionary);
        }

        public void WriteFormat(LogLevel level, string message, params object[] args)
        {
            var resultMessage = string.Format(message, args);
            Write(level, resultMessage);
        }

        public void WriteFormat(LogLevel level, string message, IDictionary<string, object> properties, params object[] args)
        {
            var resultMessage = string.Format(message, args);
            Write(level, resultMessage, properties);
        }

        private void FillPropertiesFromException(Exception exception, IDictionary<string, object> properties)
        {
            if (exception == null)
                return;

            if (!properties.ContainsKey(ExtentionPropertyName.Stack))
            {
                properties.Add(ExtentionPropertyName.Stack, ComponentControl.Client.ExceptionRender.GetFullStackTrace(exception));
                var appErrorException = exception as ApplicationErrorException;
                if (appErrorException != null)
                {
                    foreach (var property in appErrorException.Properties)
                    {
                        var key = property.Name;
                        var value = property.Value.Value;
                        if (properties.ContainsKey(key))
                        {
                            properties[key] = value;
                        }
                        else
                        {
                            properties.Add(key, value);
                        }
                    }
                }
                //else
                //{
                //    foreach (var dataKey in exception.Data.Keys)
                //    {
                //        var key = dataKey.ToString();
                //        var value = exception.Data[dataKey];
                //        properties[key] = value;
                //    }
                //}
            }

#if !PocketPC
            foreach (var dataKey in exception.Data.Keys)
            {
                var key = dataKey.ToString();
                var value = exception.Data[dataKey];
                if (properties.ContainsKey(key))
                {
                    properties[key] = value;
                }
                else
                {
                    properties.Add(key, value);
                }
            }
#endif
        }

        #endregion

        #region IsLevelEnabled

        protected bool IsLevelEnabled(LogLevel level)
        {
            if (level == LogLevel.Trace)
            {
                return WebLogConfig.IsTraceEnabled;
            }
            if (level == LogLevel.Debug)
            {
                return WebLogConfig.IsDebugEnabled;
            }
            if (level == LogLevel.Info)
            {
                return WebLogConfig.IsInfoEnabled;
            }
            if (level == LogLevel.Warning)
            {
                return WebLogConfig.IsWarningEnabled;
            }
            if (level == LogLevel.Error)
            {
                return WebLogConfig.IsErrorEnabled;
            }
            if (level == LogLevel.Fatal)
            {
                return WebLogConfig.IsFatalEnabled;
            }
            return false;
        }

        public bool IsTraceEnabled
        {
            get { return IsLevelEnabled(LogLevel.Trace); }
        }

        public bool IsDebugEnabled
        {
            get { return IsLevelEnabled(LogLevel.Debug); }
        }

        public bool IsInfoEnabled
        {
            get { return IsLevelEnabled(LogLevel.Info); }
        }

        public bool IsWarningEnabled
        {
            get { return IsLevelEnabled(LogLevel.Warning); }
        }

        public bool IsErrorEnabled
        {
            get { return IsLevelEnabled(LogLevel.Error); }
        }

        public bool IsFatalEnabled
        {
            get { return IsLevelEnabled(LogLevel.Fatal); }
        }

        #endregion
    }
}
