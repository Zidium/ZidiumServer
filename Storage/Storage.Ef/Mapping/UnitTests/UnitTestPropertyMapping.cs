using System.Data.Entity.ModelConfiguration;

namespace Zidium.Storage.Ef.Mapping
{
    internal class UnitTestPropertyMapping : EntityTypeConfiguration<DbUnitTestProperty>
    {
        public UnitTestPropertyMapping()
        {
            HasKey(t => t.Id);
            ToTable("UnitTestProperties");
            Property(t => t.Name).HasColumnName("Name").IsRequired().HasMaxLength(255);
            Property(t => t.Value).HasColumnName("Value").HasMaxLength(8000);
            Property(t => t.DataType);
            HasRequired(x => x.UnitTest).WithMany(x => x.Properties).HasForeignKey(x => x.UnitTestId).WillCascadeOnDelete(false);
        }
    }
}
