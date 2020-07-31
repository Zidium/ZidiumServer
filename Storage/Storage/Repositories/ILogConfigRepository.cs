using System;

namespace Zidium.Storage
{
    public interface ILogConfigRepository
    {
        void Add(LogConfigForAdd entity);

        void Update(LogConfigForUpdate entity);

        LogConfigForRead GetOneByComponentId(Guid id);

        LogConfigForRead GetOneOrNullByComponentId(Guid id);

        LogConfigForRead[] GetChanged(DateTime lastUpdateDate, Guid[] componentIds);

    }
}
