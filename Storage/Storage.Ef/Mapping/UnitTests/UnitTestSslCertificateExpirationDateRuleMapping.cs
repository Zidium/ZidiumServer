using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Zidium.Storage.Ef.Mapping
{
    internal class UnitTestSslCertificateExpirationDateRuleMapping : IEntityTypeConfiguration<DbUnitTestSslCertificateExpirationDateRule>
    {
        public void Configure(EntityTypeBuilder<DbUnitTestSslCertificateExpirationDateRule> builder)
        {
            builder.ToTable("UnitTestSslCertificateExpirationDateRules");
            builder.HasKey(t => t.UnitTestId).IsClustered(false);
            builder.Property(t => t.Url).HasMaxLength(200);
            builder.HasOne(t => t.UnitTest).WithOne(t => t.SslCertificateExpirationDateRule).IsRequired();
        }
    }
}
