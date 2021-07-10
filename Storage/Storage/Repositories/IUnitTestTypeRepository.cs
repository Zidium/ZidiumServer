using System;

namespace Zidium.Storage
{
    public interface IUnitTestTypeRepository
    {
        void Add(UnitTestTypeForAdd entity);

        void Update(UnitTestTypeForUpdate[] entities);

        UnitTestTypeForRead GetOneById(Guid id);

        UnitTestTypeForRead GetOneOrNullById(Guid id);

        UnitTestTypeForRead GetOneOrNullBySystemName(string systemName);

        UnitTestTypeForRead[] GetMany(Guid[] ids);

        UnitTestTypeForRead[] GetAllWithDeleted();

        int GetNonSystemCount();

        UnitTestTypeForRead[] Filter(string search, int maxCount);

    }
}
