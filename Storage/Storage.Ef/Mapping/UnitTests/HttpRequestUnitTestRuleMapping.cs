using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Zidium.Storage.Ef.Mapping
{
    internal class HttpRequestUnitTestRuleMapping : IEntityTypeConfiguration<DbHttpRequestUnitTestRule>
    {
        public void Configure(EntityTypeBuilder<DbHttpRequestUnitTestRule> builder)
        {
            builder.ToTable("HttpRequestUnitTestRules");
            builder.HasKey(t => t.Id).IsClustered(false);
            builder.HasOne(t => t.HttpRequestUnitTest).WithMany(t => t.Rules).HasForeignKey(t => t.HttpRequestUnitTestId).IsRequired();
            builder.Property(t => t.DisplayName).IsRequired().HasMaxLength(255);
            builder.Property(t => t.Url).IsRequired().HasMaxLength(255);
            builder.Property(t => t.Method).IsRequired();
            builder.Property(t => t.Body).HasMaxLength(4000);
            builder.Property(t => t.SuccessHtml).HasMaxLength(255);
            builder.Property(t => t.ErrorHtml).HasMaxLength(255);
        }
    }
}
