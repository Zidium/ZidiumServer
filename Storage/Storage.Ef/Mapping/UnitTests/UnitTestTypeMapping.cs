using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration;

namespace Zidium.Storage.Ef.Mapping
{
    internal class UnitTestTypeMapping : EntityTypeConfiguration<DbUnitTestType>
    {
        public UnitTestTypeMapping()
        {
            HasKey(t => t.Id);
            ToTable("UnitTestTypes");
            Property(t => t.Id).HasColumnName("Id");
            Property(t => t.SystemName).HasColumnName("SystemName").IsRequired().HasMaxLength(255);
            Property(t => t.DisplayName).HasColumnName("DisplayName").IsRequired().HasMaxLength(255);
            Property(t => t.IsDeleted).HasColumnName("IsDeleted");
            Property(t => t.IsSystem).HasColumnName("IsSystem");
            Property(t => t.NoSignalColor).HasColumnName("NoSignalColor");
            Property(t => t.ActualTimeSecs).HasColumnName("ActualTimeSecs");
            Property(t => t.SystemName).HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute()));
        }
    }
}
