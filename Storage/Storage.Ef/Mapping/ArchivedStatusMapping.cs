using System.Data.Entity.ModelConfiguration;

namespace Zidium.Storage.Ef.Mapping
{
    internal class ArchivedStatusMapping : EntityTypeConfiguration<DbArchivedStatus>
    {
        public ArchivedStatusMapping()
        {
            ToTable("ArchivedStatuses");
            HasKey(t => t.Id);
            HasRequired(t => t.Event).WithMany().HasForeignKey(t => t.EventId);
        }
    }
}
