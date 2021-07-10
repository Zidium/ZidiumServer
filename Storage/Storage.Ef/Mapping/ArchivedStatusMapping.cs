using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Zidium.Storage.Ef.Mapping
{
    internal class ArchivedStatusMapping : IEntityTypeConfiguration<DbArchivedStatus>
    {
        public void Configure(EntityTypeBuilder<DbArchivedStatus> builder)
        {
            builder.ToTable("ArchivedStatuses");
            builder.HasKey(t => t.Id);
            builder.HasOne(t => t.Event).WithMany().HasForeignKey(t => t.EventId).IsRequired();
        }
    }
}
