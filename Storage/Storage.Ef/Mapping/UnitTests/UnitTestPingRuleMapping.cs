using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Zidium.Storage.Ef.Mapping
{
    internal class UnitTestPingRuleMapping : IEntityTypeConfiguration<DbUnitTestPingRule>
    {
        public void Configure(EntityTypeBuilder<DbUnitTestPingRule> builder)
        {
            builder.ToTable("UnitTestPingRules");
            builder.HasKey(t => t.UnitTestId).IsClustered(false);
            builder.HasOne(t => t.UnitTest).WithOne(t => t.PingRule).IsRequired();
        }
    }
}
