using Microsoft.Extensions.Logging;

namespace Zidium.Core.InternalLogger
{
    internal static class LogLevelHelper
    {
        public static Zidium.Api.Dto.LogLevel GetLogLevel(LogLevel logLevel)
        {
            if (logLevel == LogLevel.Trace)
                return Zidium.Api.Dto.LogLevel.Trace;

            if (logLevel == LogLevel.Debug)
                return Zidium.Api.Dto.LogLevel.Debug;

            if (logLevel == LogLevel.Information || logLevel == LogLevel.None)
                return Zidium.Api.Dto.LogLevel.Info;

            if (logLevel == LogLevel.Warning)
                return Zidium.Api.Dto.LogLevel.Warning;

            if (logLevel == LogLevel.Error)
                return Zidium.Api.Dto.LogLevel.Error;

            return Zidium.Api.Dto.LogLevel.Fatal;
        }
    }
}
