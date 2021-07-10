using System;
using Zidium.Api.Dto;

namespace Zidium.Storage
{
    public interface IUnitTestRepository
    {
        void Add(UnitTestForAdd entity);

        void Update(UnitTestForUpdate entity);

        void Update(UnitTestForUpdate[] entities);

        UnitTestForRead GetOneById(Guid id);

        UnitTestForRead GetOneOrNullById(Guid id);

        UnitTestForRead[] GetByComponentId(Guid componentId);

        UnitTestForRead[] GetByUnitTestTypeId(Guid unitTestTypeId);

        Guid[] GetNotActualIds(DateTime now, int maxCount);

        int GetCount();

        UnitTestForRead[] GetForProcessing(Guid typeId, DateTime date);

        UnitTestForRead[] GetAllWithDeleted();

        UnitTestForRead[] Filter(
            Guid? componentTypeId,
            Guid? componentId,
            Guid? unitTestTypeId,
            MonitoringStatus[] statuses, 
            int maxCount);

    }
}
