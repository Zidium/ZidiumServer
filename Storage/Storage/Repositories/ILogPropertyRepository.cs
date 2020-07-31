using System;

namespace Zidium.Storage
{
    public interface ILogPropertyRepository
    {
        LogPropertyForRead GetOneById(Guid id);

        LogPropertyForRead[] GetByLogId(Guid logId);
    }
}
