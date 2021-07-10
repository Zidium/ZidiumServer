using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Zidium.Storage.Ef.Mapping
{
    internal class HttpRequestUnitTestMapping : IEntityTypeConfiguration<DbHttpRequestUnitTest>
    {
        public void Configure(EntityTypeBuilder<DbHttpRequestUnitTest> builder)
        {
            builder.ToTable("HttpRequestUnitTests");
            builder.HasKey(t => t.UnitTestId).IsClustered(false);
            builder.HasOne(t => t.UnitTest).WithOne(t => t.HttpRequestUnitTest).IsRequired();
        }
    }
}
