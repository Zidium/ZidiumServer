using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Zidium.Storage.Ef.Mapping
{
    internal class UnitTestSqlRuleMapping : IEntityTypeConfiguration<DbUnitTestSqlRule>
    {
        public void Configure(EntityTypeBuilder<DbUnitTestSqlRule> builder)
        {
            builder.ToTable("UnitTestSqlRules");
            builder.HasKey(t => t.UnitTestId).IsClustered(false);
            builder.Property(t => t.ConnectionString).HasMaxLength(500);
            builder.HasOne(t => t.UnitTest).WithOne(t => t.SqlRule).IsRequired();
        }
    }
}
