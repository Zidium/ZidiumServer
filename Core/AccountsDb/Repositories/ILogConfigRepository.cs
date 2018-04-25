using System;
using System.Collections.Generic;

namespace Zidium.Core.AccountsDb.Repositories
{
    public interface ILogConfigRepository
    {
        LogConfig GetByComponentId(Guid componentId);

        LogConfig Add(LogConfig config);

        List<LogConfig> GetChanged(DateTime lastUpdateDate, List<Guid> componentIds);
    }
}
