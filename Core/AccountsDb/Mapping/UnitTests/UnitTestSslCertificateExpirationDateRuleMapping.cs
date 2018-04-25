using System.Data.Entity.ModelConfiguration;

namespace Zidium.Core.AccountsDb.Mapping
{
    internal class UnitTestSslCertificateExpirationDateRuleMapping : EntityTypeConfiguration<UnitTestSslCertificateExpirationDateRule>
    {
        public UnitTestSslCertificateExpirationDateRuleMapping()
        {
            ToTable("UnitTestSslCertificateExpirationDateRules");
            HasKey(t => t.UnitTestId);
            Property(t => t.Url).HasColumnName("Url").HasMaxLength(200);
            Property(t => t.LastRunErrorCode).HasColumnName("LastRunErrorCode");
            Property(t => t.AlarmDaysCount).HasColumnName("AlarmDaysCount");
            Property(t => t.WarningDaysCount).HasColumnName("WarningDaysCount");
            HasRequired(t => t.UnitTest).WithOptional(t => t.SslCertificateExpirationDateRule).WillCascadeOnDelete(false);
        }
    }
}
