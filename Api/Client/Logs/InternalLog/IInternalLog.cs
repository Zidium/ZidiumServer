using System;

namespace Zidium.Api.Logs
{
    public interface IInternalLog
    {
        void Trace(string message);
        void TraceFormat(string messageTemplate, params object[] parameters);
        bool IsTraceEnabled { get; }

        void Debug(string message);
        void DebugFormat(string messageTemplate, params object[] parameters);
        bool IsDebugEnabled { get; }

        void Info(string message);
        void InfoFormat(string messageTemplate, params object[] parameters);

        void Warning(string message);
        void WarningFormat(string messageTemplate, params object[] parameters);

        void Error(string message);
        void ErrorFormat(string messageTemplate, params object[] parameters);
        void Error(string message, Exception exception);

        void Fatal(string message);
        void FatalFormat(string messageTemplate, params object[] parameters);
        void Fatal(string message, Exception exception);
    }
}
