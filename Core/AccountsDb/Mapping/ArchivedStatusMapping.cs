using System.Data.Entity.ModelConfiguration;

namespace Zidium.Core.AccountsDb.Mapping
{
    internal class ArchivedStatusMapping : EntityTypeConfiguration<ArchivedStatus>
    {
        public ArchivedStatusMapping()
        {
            ToTable("ArchivedStatuses");
            HasKey(t => t.Id);
            HasRequired(t => t.Event).WithMany().HasForeignKey(t => t.EventId);
        }
    }
}
