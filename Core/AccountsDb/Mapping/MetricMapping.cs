using System.Data.Entity.ModelConfiguration;

namespace Zidium.Core.AccountsDb.Mapping
{
    internal class MetricMapping : EntityTypeConfiguration<Metric>
    {
        public MetricMapping()
        {
            ToTable("Metrics");
            HasKey(t => t.Id);
            Property(t => t.IsDeleted).HasColumnName("IsDeleted");
            Property(t => t.Value).HasColumnName("Value");
            Property(t => t.BeginDate).HasColumnName("BeginDate");
            Property(t => t.CreateDate).HasColumnName("CreateDate");
            Property(t => t.ActualDate).HasColumnName("ActualDate");
            Property(t => t.DisableToDate).HasColumnName("DisableToDate");
            Property(t => t.DisableComment).HasColumnName("DisableComment").HasMaxLength(1000);
            Property(t => t.ActualTimeSecs).HasColumnName("ActualTimeSecs");
            Property(t => t.NoSignalColor).HasColumnName("NoSignalColor");
            Property(t => t.ConditionAlarm).HasColumnName("ConditionAlarm").HasMaxLength(500);
            Property(t => t.ConditionWarning).HasColumnName("ConditionWarning").HasMaxLength(500);
            Property(t => t.ConditionSuccess).HasColumnName("ConditionSuccess").HasMaxLength(500);
            Property(t => t.ConditionElseColor).HasColumnName("ConditionElseColor");
            HasRequired(t => t.Bulb).WithMany().HasForeignKey(t => t.StatusDataId).WillCascadeOnDelete(false);
            HasRequired(t => t.Component).WithMany(t => t.Metrics).HasForeignKey(t => t.ComponentId);
            HasRequired(t => t.MetricType).WithMany().HasForeignKey(t => t.MetricTypeId);
        }
    }
}
