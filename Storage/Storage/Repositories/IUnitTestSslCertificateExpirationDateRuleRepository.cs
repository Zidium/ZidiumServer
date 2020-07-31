using System;

namespace Zidium.Storage
{
    public interface IUnitTestSslCertificateExpirationDateRuleRepository
    {
        void Add(UnitTestSslCertificateExpirationDateRuleForAdd entity);

        void Update(UnitTestSslCertificateExpirationDateRuleForUpdate entity);

        UnitTestSslCertificateExpirationDateRuleForRead GetOneByUnitTestId(Guid unitTestId);

        UnitTestSslCertificateExpirationDateRuleForRead GetOneOrNullByUnitTestId(Guid unitTestId);
    }
}
