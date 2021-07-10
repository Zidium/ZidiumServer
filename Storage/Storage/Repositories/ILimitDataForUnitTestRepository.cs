using System;

namespace Zidium.Storage
{
    public interface ILimitDataForUnitTestRepository
    {
        void Add(LimitDataForUnitTestForAdd entity);

        void Add(LimitDataForUnitTestForAdd[] entity);

        LimitDataForUnitTestForRead GetOneById(Guid id);

        LimitDataForUnitTestForRead GetForUnitTest(Guid limitDataId, Guid unitTestId);

        Tuple<Guid, int>[] GetGroupedByUnitTest(DateTime date, LimitDataType type);

    }
}
