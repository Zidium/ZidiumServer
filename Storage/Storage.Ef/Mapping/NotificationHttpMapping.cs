using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Zidium.Storage.Ef.Mapping
{
    public class NotificationHttpMapping : IEntityTypeConfiguration<DbNotificationHttp>
    {
        public void Configure(EntityTypeBuilder<DbNotificationHttp> builder)
        {
            builder.ToTable("NotificationsHttp");
            builder.HasKey(t => t.NotificationId).IsClustered(false);
            builder.HasOne(t => t.Notification).WithOne(t => t.NotificationHttp).IsRequired();
        }
    }
}
