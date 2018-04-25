using System.Data.Entity.ModelConfiguration;

namespace Zidium.Core.AccountsDb.Mapping
{
    internal class UnitTestPingRuleMapping : EntityTypeConfiguration<UnitTestPingRule>
    {
        public UnitTestPingRuleMapping()
        {
            ToTable("UnitTestPingRules");
            HasKey(t => t.UnitTestId);
            Property(t => t.UnitTestId).HasColumnName("UnitTestId");
            Property(t => t.Attemps).HasColumnName("Attemps");
            Property(t => t.TimeoutMs).HasColumnName("TimeoutMs");
            Property(t => t.Host).HasColumnName("Host");
            Property(t => t.LastRunErrorCode).HasColumnName("LastRunErrorCode");
            HasRequired(t => t.UnitTest).WithOptional(t => t.PingRule).WillCascadeOnDelete(false);
        }
    }
}
