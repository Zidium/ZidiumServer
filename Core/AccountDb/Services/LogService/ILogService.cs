using System;
using Zidium.Api.Dto;
using Zidium.Storage;

namespace Zidium.Core.AccountsDb
{
    public interface ILogService
    {
        LogConfigForRead GetLogConfig(Guid componentId);

        void SaveLogMessage(Guid componentId, SendLogRequestDataDto message);

        void SaveLogMessages(SendLogRequestDataDto[] messages);

        LogForRead[] GetLogs(Guid componentId, GetLogsRequestDataDto requestData);

        LogConfigForRead[] GetChangedConfigs(DateTime lastUpdateDate, Guid[] componentIds);
    }
}
