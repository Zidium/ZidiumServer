using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Zidium.Storage.Ef.Mapping
{
    internal class UnitTestTcpPortRuleMapping : IEntityTypeConfiguration<DbUnitTestTcpPortRule>
    {
        public void Configure(EntityTypeBuilder<DbUnitTestTcpPortRule> builder)
        {
            builder.ToTable("UnitTestTcpPortRules");
            builder.HasKey(t => t.UnitTestId).IsClustered(false);
            builder.HasOne(t => t.UnitTest).WithOne(t => t.TcpPortRule).IsRequired();
        }
    }
}
