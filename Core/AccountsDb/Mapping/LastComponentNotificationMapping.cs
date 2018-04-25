using System.Data.Entity.ModelConfiguration;

namespace Zidium.Core.AccountsDb.Mapping
{
    internal class LastComponentNotificationMapping : EntityTypeConfiguration<LastComponentNotification>
    {
        public LastComponentNotificationMapping()
        {
            HasKey(t => t.Id);
            ToTable("LastComponentNotifications");
            Property(t => t.Address).HasColumnName("Address").IsRequired().HasMaxLength(255);
            Property(t => t.CreateDate).HasColumnName("CreateDate");
            Property(t => t.EventImportance).HasColumnName("EventImportance");
            Property(t => t.NotificationId).HasColumnName("NotificationId");
            Property(t => t.EventId).HasColumnName("EventId");
            Property(t => t.Type).HasColumnName("Type");
            Property(t => t.ComponentId).HasColumnName("ComponentId");

            HasRequired(t => t.Component).WithMany(t => t.LastNotifications).HasForeignKey(t => t.ComponentId).WillCascadeOnDelete(false);
            HasRequired(t => t.Event).WithMany().HasForeignKey(t => t.EventId);
            HasRequired(t => t.Notification).WithMany().HasForeignKey(t => t.NotificationId);
        }
    }
}
