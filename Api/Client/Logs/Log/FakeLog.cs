using System;
using System.Collections.Generic;
using Zidium.Api.Dto;

namespace Zidium.Api
{
    /// <inheritdoc />
    /// <summary>
    /// Заглушка для лога
    /// </summary>
    public class FakeLog : ILog
    {
        public FakeLog()
        {
            WebLogConfig = new WebLogConfig();
            FileLogConfig = new LogConfig();
            ConsoleLogConfig = new LogConfig();
        }

        public void Flush()
        {
        }

        public bool IsFake()
        {
            return true;
        }

        public WebLogConfig WebLogConfig { get; set; }

        public LogConfig FileLogConfig { get; set; }

        public LogConfig ConsoleLogConfig { get; set; }

        public WebLogConfig GetLogConfig()
        {
            return new WebLogConfig();
        }

        public void Debug(string message)
        {
        }

        public void Debug(string message, IDictionary<string, object> properties)
        {
        }

        public void Debug(string message, object properties)
        {
        }

        public void Debug(Exception exception)
        {
        }

        public void Debug(string message, Exception exception)
        {
        }

        public void Debug(string message, Exception exception, IDictionary<string, object> properties)
        {
        }

        public void Debug(string message, Exception exception, object properties)
        {
        }

        public void Trace(string message)
        {
        }

        public void Trace(string message, IDictionary<string, object> properties)
        {
        }

        public void Trace(string message, object properties)
        {
        }

        public void Trace(Exception exception)
        {
        }

        public void Trace(string message, Exception exception)
        {
        }

        public void Trace(string message, Exception exception, IDictionary<string, object> properties)
        {
        }

        public void Trace(string message, Exception exception, object properties)
        {
        }

        public void Info(string message)
        {
        }

        public void Info(string message, IDictionary<string, object> properties)
        {
        }

        public void Info(string message, object properties)
        {
        }

        public void Info(Exception exception)
        {
        }

        public void Info(string message, Exception exception)
        {
        }

        public void Info(string message, Exception exception, IDictionary<string, object> properties)
        {
        }

        public void Info(string message, Exception exception, object properties)
        {
        }

        public void Warning(string message)
        {
        }

        public void Warning(string message, IDictionary<string, object> properties)
        {
        }

        public void Warning(string message, object properties)
        {
        }

        public void Warning(Exception exception)
        {
        }

        public void Warning(string message, Exception exception)
        {
        }

        public void Warning(string message, Exception exception, IDictionary<string, object> properties)
        {
        }

        public void Warning(string message, Exception exception, object properties)
        {
        }

        public void Error(string message)
        {
        }

        public void Error(string message, IDictionary<string, object> properties)
        {
        }

        public void Error(string message, object properties)
        {
        }

        public void Error(Exception exception)
        {
        }

        public void Error(string message, Exception exception)
        {
        }

        public void Error(string message, Exception exception, IDictionary<string, object> properties)
        {
        }

        public void Error(string message, Exception exception, object properties)
        {
        }

        public void Fatal(string message)
        {
        }

        public void Fatal(string message, IDictionary<string, object> properties)
        {
        }

        public void Fatal(string message, object properties)
        {
        }

        public void Fatal(Exception exception)
        {
        }

        public string Context { get; set; }

        public ILog GetTaggedCopy(string tag)
        {
            return new FakeLog()
            {
                Context = tag,
                WebLogConfig = WebLogConfig
            };
        }

        public ILog AddTag(string tag)
        {
            return GetTaggedCopy(tag);
        }

        public void Fatal(string message, Exception exception)
        {

        }

        public void Fatal(string message, Exception exception, IDictionary<string, object> properties)
        {

        }

        public void Fatal(string message, Exception exception, object properties)
        {

        }

        public void TraceFormat(string message, params object[] args)
        {

        }

        public void TraceFormat(string message, IDictionary<string, object> properties, params object[] args)
        {

        }

        public void DebugFormat(string message, params object[] args)
        {

        }

        public void DebugFormat(string message, IDictionary<string, object> properties, params object[] args)
        {

        }

        public void InfoFormat(string message, params object[] args)
        {

        }

        public void InfoFormat(string message, IDictionary<string, object> properties, params object[] args)
        {

        }

        public void WarningFormat(string message, params object[] args)
        {

        }

        public void WarningFormat(string message, IDictionary<string, object> properties, params object[] args)
        {

        }

        public void ErrorFormat(string message, params object[] args)
        {

        }

        public void ErrorFormat(string message, IDictionary<string, object> properties, params object[] args)
        {

        }

        public void FatalFormat(string message, params object[] args)
        {

        }

        public void FatalFormat(string message, IDictionary<string, object> properties, params object[] args)
        {

        }

        public void Write(LogLevel level, string message)
        {
        }

        public void Write(LogLevel level, string message, IDictionary<string, object> properties)
        {
        }

        public void Write(LogLevel level, string message, object properties)
        {
        }

        public void Write(LogLevel level, Exception exception)
        {
        }

        public void Write(LogLevel level, string message, Exception exception)
        {
        }

        public void Write(LogLevel level, string message, Exception exception, IDictionary<string, object> properties)
        {
        }

        public void Write(LogLevel level, string message, Exception exception, object properties)
        {
        }

        public void WriteFormat(LogLevel level, string message, params object[] args)
        {
        }

        public void WriteFormat(LogLevel level, string message, IDictionary<string, object> properties, params object[] args)
        {
        }

        public bool IsTraceEnabled
        {
            get { return false; }
        }

        public bool IsDebugEnabled
        {
            get { return false; }
        }

        public bool IsInfoEnabled
        {
            get { return false; }
        }

        public bool IsWarningEnabled
        {
            get { return false; }
        }

        public bool IsErrorEnabled
        {
            get { return false; }
        }

        public bool IsFatalEnabled
        {
            get { return false; }
        }
    }
}
