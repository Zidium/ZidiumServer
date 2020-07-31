using System;
using Zidium.Core.Api;
using Zidium.Storage;

namespace Zidium.Core.AccountsDb
{
    public interface ILogService
    {
        LogConfigForRead GetLogConfig(Guid componentId);

        void SaveLogMessage(Guid accountId, Guid componentId, SendLogData message);

        void SaveLogMessages(Guid accountId, SendLogData[] messages);

        LogForRead[] GetLogs(Guid accountId, Guid componentId, GetLogsRequestData requestData);

        LogConfigForRead[] GetChangedConfigs(Guid accountId, DateTime lastUpdateDate, Guid[] componentIds);
    }
}
