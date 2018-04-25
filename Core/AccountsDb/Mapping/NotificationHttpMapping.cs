using System.Data.Entity.ModelConfiguration;

namespace Zidium.Core.AccountsDb.Mapping
{
    public class NotificationHttpMapping : EntityTypeConfiguration<NotificationHttp>
    {
        public NotificationHttpMapping()
        {
            ToTable("NotificationsHttp");
            HasKey(t => t.NotificationId);
            HasRequired(t => t.Notification).WithOptional(t => t.NotificationHttp);
        }
    }
}
