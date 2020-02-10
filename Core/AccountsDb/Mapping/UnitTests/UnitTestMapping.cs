using System.Data.Entity.ModelConfiguration;

namespace Zidium.Core.AccountsDb.Mapping
{
    internal class UnitTestMapping : EntityTypeConfiguration<UnitTest>
    {
        public UnitTestMapping()
        {
            HasKey(t => t.Id);
            ToTable("UnitTests");
            Property(t => t.Id).HasColumnName("Id");
            Property(t => t.SystemName).HasColumnName("SystemName").IsRequired().HasMaxLength(255);
            Property(t => t.DisplayName).HasColumnName("DisplayName").IsRequired().HasMaxLength(255);
            Property(t => t.PeriodSeconds).HasColumnName("PeriodSeconds");
            Property(t => t.IsDeleted).HasColumnName("IsDeleted");
            Property(t => t.CreateDate).HasColumnName("CreateDate");
            Property(t => t.ErrorColor).HasColumnName("ErrorColor");
            Property(t => t.Enable).HasColumnName("Enable");
            Property(t => t.DisableToDate).HasColumnName("DisableToDate");
            Property(t => t.DisableComment).HasColumnName("DisableComment").HasMaxLength(1000);
            Property(t => t.ParentEnable).HasColumnName("ParentEnable");
            Property(t => t.SimpleMode).HasColumnName("SimpleMode");
            Property(t => t.NextExecutionDate).HasColumnName("NextExecutionDate");
            Property(t => t.LastExecutionDate).HasColumnName("LastExecutionDate");
            Property(t => t.NextStepProcessDate).HasColumnName("NextStepProcessDate");
            Property(t => t.NoSignalColor).HasColumnName("NoSignalColor");
            Property(t => t.ActualTimeSecs).HasColumnName("ActualTimeSecs");
            HasRequired(x => x.Type).WithMany().HasForeignKey(x => x.TypeId);
            HasRequired(x => x.Component).WithMany(x => x.UnitTests).HasForeignKey(x => x.ComponentId);
            HasRequired(x => x.Bulb).WithMany().HasForeignKey(x => x.StatusDataId);
        }
    }
}
