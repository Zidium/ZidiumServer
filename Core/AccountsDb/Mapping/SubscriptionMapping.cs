using System.Data.Entity.ModelConfiguration;

namespace Zidium.Core.AccountsDb.Mapping
{
    internal class SubscriptionMapping : EntityTypeConfiguration<Subscription>
    {
        public SubscriptionMapping()
        {
            ToTable("Subscriptions");
            HasKey(t => t.Id);
            Property(t => t.NotifyBetterStatus).HasColumnName("NotifyBetterStatus");
            Property(t => t.Object).HasColumnName("Object");

            HasRequired(t => t.User).WithMany(d => d.Subscriptions).HasForeignKey(t => t.UserId);
            HasOptional(t => t.ComponentType).WithMany().HasForeignKey(t => t.ComponentTypeId);
            HasOptional(t => t.Component).WithMany().HasForeignKey(t => t.ComponentId).WillCascadeOnDelete(false);
        }
    }
}
