using System;
using System.Collections.Generic;
using Zidium.Api.Dto;

namespace Zidium.Api
{
    /// <summary>
    /// Интерфейс для работы с логом
    /// </summary>
    public interface ILog
    {
        /// <summary>
        /// Настройки веб-конфига
        /// </summary>
        WebLogConfig WebLogConfig { get; }

        bool IsTraceEnabled { get; }

        bool IsDebugEnabled { get; }

        bool IsInfoEnabled { get; }

        bool IsWarningEnabled { get; }

        bool IsErrorEnabled { get; }

        bool IsFatalEnabled { get; }

        /// <summary>
        /// Если тег указан, то все отправляемые сообщения будут помечены данным тегом
        /// </summary>
        string Context { get; set; }

        /// <summary>
        /// Получает копию лога, который все сообщения будет помечать указанным тэгом
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        ILog GetTaggedCopy(string tag);

        ILog AddTag(string tag);

        void Trace(string message);
        void Trace(string message, IDictionary<string, object> properties);
        void Trace(string message, object properties);
        void Trace(Exception exception);
        void Trace(string message, Exception exception);
        void Trace(string message, Exception exception, IDictionary<string, object> properties);
        void Trace(string message, Exception exception, object properties);
        void TraceFormat(string message, params object[] args);
        void TraceFormat(string message, IDictionary<string, object> properties, params object[] args);

        void Debug(string message);
        void Debug(string message, IDictionary<string, object> properties);
        void Debug(string message, object properties);
        void Debug(Exception exception);
        void Debug(string message, Exception exception);
        void Debug(string message, Exception exception, IDictionary<string, object> properties);
        void Debug(string message, Exception exception, object properties);
        void DebugFormat(string message, params object[] args);
        void DebugFormat(string message, IDictionary<string, object> properties, params object[] args);

        void Info(string message);
        void Info(string message, IDictionary<string, object> properties);
        void Info(string message, object properties);
        void Info(Exception exception);
        void Info(string message, Exception exception);
        void Info(string message, Exception exception, IDictionary<string, object> properties);
        void Info(string message, Exception exception, object properties);
        void InfoFormat(string message, params object[] args);
        void InfoFormat(string message, IDictionary<string, object> properties, params object[] args);

        void Warning(string message);
        void Warning(string message, IDictionary<string, object> properties);
        void Warning(string message, object properties);
        void Warning(Exception exception);
        void Warning(string message, Exception exception);
        void Warning(string message, Exception exception, IDictionary<string, object> properties);
        void Warning(string message, Exception exception, object properties);
        void WarningFormat(string message, params object[] args);
        void WarningFormat(string message, IDictionary<string, object> properties, params object[] args);

        void Error(string message);
        void Error(string message, IDictionary<string, object> properties);
        void Error(string message, object properties);
        void Error(Exception exception);
        void Error(string message, Exception exception);
        void Error(string message, Exception exception, IDictionary<string, object> properties);
        void Error(string message, Exception exception, object properties);
        void ErrorFormat(string message, params object[] args);
        void ErrorFormat(string message, IDictionary<string, object> properties, params object[] args);

        void Fatal(string message);
        void Fatal(string message, IDictionary<string, object> properties);
        void Fatal(string message, object properties);
        void Fatal(Exception exception);
        void Fatal(string message, Exception exception);
        void Fatal(string message, Exception exception, IDictionary<string, object> properties);
        void Fatal(string message, Exception exception, object properties);
        void FatalFormat(string message, params object[] args);
        void FatalFormat(string message, IDictionary<string, object> properties, params object[] args);

        void Write(LogLevel level, string message);
        void Write(LogLevel level, string message, IDictionary<string, object> properties);
        void Write(LogLevel level, string message, object properties);
        void Write(LogLevel level, Exception exception);
        void Write(LogLevel level, string message, Exception exception);
        void Write(LogLevel level, string message, Exception exception, IDictionary<string, object> properties);
        void Write(LogLevel level, string message, Exception exception, object properties);
        void WriteFormat(LogLevel level, string message, params object[] args);
        void WriteFormat(LogLevel level, string message, IDictionary<string, object> properties, params object[] args);

        /// <summary>
        /// Принудительная отправка данных лога на сервер
        /// </summary>
        void Flush();

        /// <summary>
        /// True - если программе не удалось получить данные объекта от сервера.
        /// Например, если нет интернета.
        /// </summary>
        /// <returns></returns>
        bool IsFake();
    }
}
