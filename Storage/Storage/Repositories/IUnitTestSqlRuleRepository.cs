using System;

namespace Zidium.Storage
{
    public interface IUnitTestSqlRuleRepository
    {
        void Add(UnitTestSqlRuleForAdd entity);

        void Update(UnitTestSqlRuleForUpdate entity);

        UnitTestSqlRuleForRead GetOneByUnitTestId(Guid unitTestId);

        UnitTestSqlRuleForRead GetOneOrNullByUnitTestId(Guid unitTestId);
    }
}
