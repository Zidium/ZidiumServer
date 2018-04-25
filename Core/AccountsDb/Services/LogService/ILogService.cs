using System;
using System.Collections.Generic;
using Zidium.Core.AccountsDb;
using Zidium.Core.Api;

namespace Zidium.Core.AccountsDb
{
    public interface ILogService
    {
        LogConfig GetLogConfig(Guid accountId, Guid componentId);

        void SaveLogMessage(Guid accountId, Guid componentId, SendLogData message);

        void SaveLogMessages(Guid accountId, SendLogData[] messages);

        List<Log> GetLogs(Guid accountId, Guid componentId, GetLogsRequestData requestData);

        List<LogConfig> GetChangedConfigs(Guid accountId, DateTime lastUpdateDate, List<Guid> componentIds);
    }
}
