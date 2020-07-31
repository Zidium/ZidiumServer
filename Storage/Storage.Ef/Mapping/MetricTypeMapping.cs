using System.Data.Entity.ModelConfiguration;

namespace Zidium.Storage.Ef.Mapping
{
    internal class MetricTypeMapping : EntityTypeConfiguration<DbMetricType>
    {
        public MetricTypeMapping()
        {
            HasKey(t => t.Id);
            ToTable("MetricTypes");
            Property(t => t.SystemName).IsRequired().HasMaxLength(255);
            Property(t => t.DisplayName).HasMaxLength(255);
            Property(t => t.IsDeleted).HasColumnName("IsDeleted");
            Property(t => t.CreateDate).HasColumnName("CreateDate");
            Property(t => t.ConditionAlarm).HasColumnName("ConditionAlarm").HasMaxLength(500);
            Property(t => t.ConditionWarning).HasColumnName("ConditionWarning").HasMaxLength(500);
            Property(t => t.ConditionSuccess).HasColumnName("ConditionSuccess").HasMaxLength(500);
            Property(t => t.ConditionElseColor).HasColumnName("ConditionElseColor");
            Property(t => t.NoSignalColor).HasColumnName("NoSignalColor");
            Property(t => t.ActualTimeSecs).HasColumnName("ActualTimeSecs");
        }
    }
}
