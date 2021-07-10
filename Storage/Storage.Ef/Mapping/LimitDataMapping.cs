using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Zidium.Storage.Ef.Mapping
{
    internal class LimitDataMapping : IEntityTypeConfiguration<DbLimitData>
    {
        public void Configure(EntityTypeBuilder<DbLimitData> builder)
        {
            builder.ToTable("LimitDatas");
            builder.HasKey(t => t.Id).IsClustered(false);

            // TODO Rename index
            builder.HasIndex(t => new { t.Type, t.BeginDate}, "IX_AccountId");
        }
    }
}
