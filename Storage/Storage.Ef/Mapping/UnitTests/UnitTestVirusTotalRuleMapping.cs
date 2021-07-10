using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Zidium.Storage.Ef.Mapping
{
    internal class UnitTestVirusTotalRuleMapping : IEntityTypeConfiguration<DbUnitTestVirusTotalRule>
    {
        public void Configure(EntityTypeBuilder<DbUnitTestVirusTotalRule> builder)
        {
            builder.ToTable("UnitTestVirusTotalRules");
            builder.HasKey(t => t.UnitTestId).IsClustered(false);
            builder.Property(t => t.Url).HasMaxLength(2000).IsRequired();
            builder.Property(t => t.ScanId).HasMaxLength(100);
            builder.HasOne(t => t.UnitTest).WithOne(t => t.VirusTotalRule).IsRequired();
        }
    }
}
