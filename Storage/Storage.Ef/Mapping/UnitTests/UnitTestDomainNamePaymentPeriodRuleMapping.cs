using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Zidium.Storage.Ef.Mapping
{
    internal class UnitTestDomainNamePaymentPeriodRuleMapping : IEntityTypeConfiguration<DbUnitTestDomainNamePaymentPeriodRule>
    {
        public void Configure(EntityTypeBuilder<DbUnitTestDomainNamePaymentPeriodRule> builder)
        {
            builder.ToTable("UnitTestDomainNamePaymentPeriodRules");
            builder.HasKey(t => t.UnitTestId).IsClustered(false);
            builder.HasOne(t => t.UnitTest).WithOne(t => t.DomainNamePaymentPeriodRule).IsRequired();
        }
    }
}
