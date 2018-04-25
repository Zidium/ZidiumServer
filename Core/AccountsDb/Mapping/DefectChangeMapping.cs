using System.Data.Entity.ModelConfiguration;
using Zidium.Core.AccountsDb.Classes;

namespace Zidium.Core.AccountsDb.Mapping
{
    internal class DefectChangeMapping : EntityTypeConfiguration<DefectChange>
    {
        public DefectChangeMapping()
        {
            HasKey(t => t.Id);
            ToTable("DefectChanges");
            Property(t => t.Status).HasColumnName("Status");
            Property(t => t.Date).HasColumnName("Date");
            Property(t => t.Comment).HasColumnName("Comment").HasMaxLength(1000);
            HasRequired(t => t.Defect).WithMany(t=>t.Changes).HasForeignKey(t => t.DefectId).WillCascadeOnDelete(false);
            HasRequired(t => t.User).WithMany().HasForeignKey(t => t.UserId).WillCascadeOnDelete(false);
        }
    }
}
