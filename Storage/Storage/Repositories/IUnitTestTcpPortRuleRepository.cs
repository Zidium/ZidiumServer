using System;

namespace Zidium.Storage
{
    public interface IUnitTestTcpPortRuleRepository
    {
        void Add(UnitTestTcpPortRuleForAdd entity);

        void Update(UnitTestTcpPortRuleForUpdate entity);

        UnitTestTcpPortRuleForRead GetOneByUnitTestId(Guid unitTestId);

        UnitTestTcpPortRuleForRead GetOneOrNullByUnitTestId(Guid unitTestId);
    }
}
