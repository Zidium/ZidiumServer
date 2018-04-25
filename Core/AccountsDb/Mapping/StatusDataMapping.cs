using System.Data.Entity.ModelConfiguration;

namespace Zidium.Core.AccountsDb
{
    internal class StatusDataMapping : EntityTypeConfiguration<Bulb>
    {
        public StatusDataMapping()
        {
            ToTable("Bulbs");
            HasKey(t => t.Id);
            Property(t => t.Message).HasMaxLength(8000);
            HasOptional(t => t.Component).WithMany().HasForeignKey(t => t.ComponentId).WillCascadeOnDelete(false);
            HasOptional(t => t.UnitTest).WithMany().HasForeignKey(t => t.UnitTestId).WillCascadeOnDelete(false);
            HasOptional(t => t.Metric).WithMany().HasForeignKey(t => t.MetricId).WillCascadeOnDelete(false);
            Property(t => t.LastChildBulbId).HasColumnName("LastChildStatusDataId");
            //HasOptional(t=>t.LastChildStatus).WithMany().HasForeignKey(t=>t.LastChildStatusDataId).WillCascadeOnDelete(false);
        }
    }
}

