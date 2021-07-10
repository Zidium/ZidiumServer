using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Zidium.Storage.Ef.Mapping
{
    internal class LimitDataForUnitTestMapping : IEntityTypeConfiguration<DbLimitDataForUnitTest>
    {
        public void Configure(EntityTypeBuilder<DbLimitDataForUnitTest> builder)
        {
            builder.ToTable("LimitDatasForUnitTests");
            builder.HasKey(t => t.Id).IsClustered(false);
            builder.HasOne(t => t.LimitData).WithMany(t => t.UnitTestData).HasForeignKey(t => t.LimitDataId).IsRequired();
            builder.HasOne(t => t.UnitTest).WithMany().HasForeignKey(t => t.UnitTestId).IsRequired();
        }
    }
}
