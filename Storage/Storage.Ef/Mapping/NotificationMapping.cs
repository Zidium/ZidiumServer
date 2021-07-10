using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Zidium.Storage.Ef.Mapping
{
    internal class NotificationMapping : IEntityTypeConfiguration<DbNotification>
    {
        public void Configure(EntityTypeBuilder<DbNotification> builder)
        {
            builder.ToTable("Notifications");
            builder.HasKey(t => t.Id).IsClustered(false);
            builder.Property(t => t.SendError).HasMaxLength(4000);
            builder.Property(t => t.Address).IsRequired().HasMaxLength(1000);

            builder.HasOne(t => t.Event).WithMany(t => t.Notifications).HasForeignKey(t => t.EventId).IsRequired();
            builder.HasOne(t => t.User).WithMany().HasForeignKey(t => t.UserId).IsRequired();
            builder.HasOne(t => t.Subscription).WithMany().HasForeignKey(t => t.SubscriptionId);
            builder.HasOne(t => t.SendEmailCommand).WithMany().HasForeignKey(t => t.SendEmailCommandId);
            builder.HasOne(t => t.SendMessageCommand).WithMany().HasForeignKey(t => t.SendMessageCommandId);

            builder.HasIndex(t => t.Status);
        }
    }
}
