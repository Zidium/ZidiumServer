using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Zidium.Storage.Ef.Mapping
{
    internal class EventMapping : IEntityTypeConfiguration<DbEvent>
    {
        public void Configure(EntityTypeBuilder<DbEvent> builder)
        {
            builder.ToTable("Events");
            builder.HasKey(t => t.Id).IsClustered(false);
            builder.Property(t => t.Message).HasMaxLength(8000);
            builder.Property(t => t.Version).HasMaxLength(255);
            builder.Property(t => t.FirstReasonEventId).HasColumnName("FirstReasonEventId");

            builder.HasOne(t => t.LastStatusEvent).WithMany().HasForeignKey(t => t.LastStatusEventId);
            builder.HasMany(t => t.StatusEvents).WithMany(t => t.ReasonEvents).UsingEntity<DbEventStatus>(
                x => x.HasOne(z => z.Event).WithMany().HasForeignKey("EventId").IsRequired(),
                x => x.HasOne(z => z.StatusEvent).WithMany().HasForeignKey("StatusId").IsRequired(),
                x =>
                {
                    x.ToTable("EventStatuses");
                    x.HasIndex(t => t.EventId);
                    x.HasIndex(t => t.StatusId);
                });

            builder.HasOne(t => t.EventType).WithMany().HasForeignKey(t => t.EventTypeId).IsRequired();

            // IX_ForDeletion - Индекс для удаления старых событий - Category, ActualDate
            // IX_OwnerBased - Индекс для поиска - OwnerId, Category, StartDate
            // IX_ForJoin - Индекс для склейки - OwnerId, EventTypeId, Importance

            builder.HasIndex(t => new { t.Category, t.ActualDate }, "IX_ForDeletion");
            builder.HasIndex(t => new { t.OwnerId, t.Category, t.StartDate }, "IX_OwnerBased");
            builder.HasIndex(t => new { t.OwnerId, t.EventTypeId, t.Importance }, "IX_ForJoin");
        }
    }
}
