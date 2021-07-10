using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Zidium.Storage.Ef.Mapping
{
    internal class HttpRequestUnitTestRuleDataMapping : IEntityTypeConfiguration<DbHttpRequestUnitTestRuleData>
    {
        public void Configure(EntityTypeBuilder<DbHttpRequestUnitTestRuleData> builder)
        {
            builder.ToTable("HttpRequestUnitTestRuleDatas");
            builder.HasKey(t => t.Id).IsClustered(false);
            builder.Property(t => t.Key).HasMaxLength(500);
            builder.HasOne(t => t.Rule).WithMany(t => t.Datas).HasForeignKey(t => t.RuleId).IsRequired();
        }
    }
}
