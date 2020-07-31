using System;

namespace Zidium.Storage
{
    public interface IUnitTestPingRuleRepository
    {
        void Add(UnitTestPingRuleForAdd entity);

        void Update(UnitTestPingRuleForUpdate entity);

        UnitTestPingRuleForRead GetOneByUnitTestId(Guid unitTestId);

        UnitTestPingRuleForRead GetOneOrNullByUnitTestId(Guid unitTestId);

    }
}
