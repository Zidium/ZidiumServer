using System;

namespace Zidium.Storage
{
    public interface IUnitTestVirusTotalRuleRepository
    {
        void Add(UnitTestVirusTotalRuleForAdd entity);

        void Update(UnitTestVirusTotalRuleForUpdate entity);

        UnitTestVirusTotalRuleForRead GetOneByUnitTestId(Guid unitTestId);

        UnitTestVirusTotalRuleForRead GetOneOrNullByUnitTestId(Guid unitTestId);
    }
}
