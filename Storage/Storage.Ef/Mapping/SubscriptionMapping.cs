using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Zidium.Storage.Ef.Mapping
{
    internal class SubscriptionMapping : IEntityTypeConfiguration<DbSubscription>
    {
        public void Configure(EntityTypeBuilder<DbSubscription> builder)
        {
            builder.ToTable("Subscriptions");
            builder.HasKey(t => t.Id).IsClustered(false);

            builder.HasOne(t => t.User).WithMany(d => d.Subscriptions).HasForeignKey(t => t.UserId).IsRequired();
            builder.HasOne(t => t.ComponentType).WithMany().HasForeignKey(t => t.ComponentTypeId);
            builder.HasOne(t => t.Component).WithMany().HasForeignKey(t => t.ComponentId);
        }
    }
}
