using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Zidium.Storage.Ef.Mapping
{
    internal class AccountSettingMapping : IEntityTypeConfiguration<DbAccountSetting>
    {
        public void Configure(EntityTypeBuilder<DbAccountSetting> builder)
        {
            builder.ToTable("AccountSettings");
            builder.HasKey(t => t.Id).IsClustered(false);
            builder.Property(t => t.Name).HasColumnName("Name").HasMaxLength(255).IsRequired();
        }
    }
}
