using System;

namespace Zidium.Storage
{
    public interface IUnitTestDomainNamePaymentPeriodRuleRepository
    {
        void Add(UnitTestDomainNamePaymentPeriodRuleForAdd entity);

        void Update(UnitTestDomainNamePaymentPeriodRuleForUpdate entity);

        UnitTestDomainNamePaymentPeriodRuleForRead GetOneByUnitTestId(Guid unitTestId);

        UnitTestDomainNamePaymentPeriodRuleForRead GetOneOrNullByUnitTestId(Guid unitTestId);

    }
}
