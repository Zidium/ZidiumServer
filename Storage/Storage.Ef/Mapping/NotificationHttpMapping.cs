using System.Data.Entity.ModelConfiguration;

namespace Zidium.Storage.Ef.Mapping
{
    public class NotificationHttpMapping : EntityTypeConfiguration<DbNotificationHttp>
    {
        public NotificationHttpMapping()
        {
            ToTable("NotificationsHttp");
            HasKey(t => t.NotificationId);
            HasRequired(t => t.Notification).WithOptional(t => t.NotificationHttp);
        }
    }
}
