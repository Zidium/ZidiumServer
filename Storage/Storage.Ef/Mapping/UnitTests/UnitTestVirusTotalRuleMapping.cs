using System.Data.Entity.ModelConfiguration;

namespace Zidium.Storage.Ef.Mapping
{
    internal class UnitTestVirusTotalRuleMapping : EntityTypeConfiguration<DbUnitTestVirusTotalRule>
    {
        public UnitTestVirusTotalRuleMapping()
        {
            ToTable("UnitTestVirusTotalRules");
            HasKey(t => t.UnitTestId);
            Property(t => t.Url).HasMaxLength(2000).IsRequired();
            Property(t => t.ScanId).HasMaxLength(100);
            Property(t => t.NextStep);
            Property(t => t.ScanTime);
            Property(t => t.LastRunErrorCode);
            HasRequired(t => t.UnitTest).WithOptional(t => t.VirusTotalRule).WillCascadeOnDelete(false);
        }
    }
}
