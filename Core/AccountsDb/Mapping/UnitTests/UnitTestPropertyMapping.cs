using System.Data.Entity.ModelConfiguration;
using Zidium.Core.AccountsDb.Classes.UnitTests;

namespace Zidium.Core.AccountsDb.Mapping
{
    internal class UnitTestPropertyMapping : EntityTypeConfiguration<UnitTestProperty>
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
