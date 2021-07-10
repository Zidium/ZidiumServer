using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Zidium.Storage.Ef.Mapping
{
    internal class LastComponentNotificationMapping : IEntityTypeConfiguration<DbLastComponentNotification>
    {
        public void Configure(EntityTypeBuilder<DbLastComponentNotification> builder)
        {
            builder.HasKey(t => t.Id).IsClustered(false);
            builder.ToTable("LastComponentNotifications");
            builder.Property(t => t.Address).IsRequired().HasMaxLength(255);

            builder.HasOne(t => t.Component).WithMany(t => t.LastNotifications).HasForeignKey(t => t.ComponentId).IsRequired();
            builder.HasOne(t => t.Event).WithMany().HasForeignKey(t => t.EventId).IsRequired();
            builder.HasOne(t => t.Notification).WithMany().HasForeignKey(t => t.NotificationId).IsRequired();
        }
    }
}
