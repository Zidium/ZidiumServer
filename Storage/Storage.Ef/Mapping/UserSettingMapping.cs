using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Zidium.Storage.Ef.Mapping
{
    internal class UserSettingMapping : IEntityTypeConfiguration<DbUserSetting>
    {
        public void Configure(EntityTypeBuilder<DbUserSetting> builder)
        {
            builder.ToTable("UserSettings");
            builder.HasKey(t => t.Id).IsClustered(false);
            builder.Property(t => t.Name).HasMaxLength(255).IsRequired();
            builder.Property(t => t.Value).HasMaxLength(255);
            builder.HasOne(t => t.User).WithMany(t => t.Settings).HasForeignKey(t => t.UserId).IsRequired();
        }
    }
}
