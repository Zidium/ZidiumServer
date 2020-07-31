using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration;

namespace Zidium.Storage.Ef.Mapping
{
    internal class EventMapping : EntityTypeConfiguration<DbEvent>
    {
        public EventMapping()
        {
            ToTable("Events");
            HasKey(t => t.Id);
            Property(t => t.Message).HasMaxLength(8000);
            Property(t => t.Version).HasMaxLength(255);
            Property(t => t.FirstReasonEventId).HasColumnName("FirstReasonEventId");

            HasOptional(t => t.LastStatusEvent).WithMany().HasForeignKey(t => t.LastStatusEventId);
            HasMany(t => t.StatusEvents).WithMany(t => t.ReasonEvents).Map(x =>
            {
                x.MapLeftKey("EventId");
                x.MapRightKey("StatusId");
                x.ToTable("EventStatuses");
            });
            HasRequired(t => t.EventType).WithMany().HasForeignKey(t => t.EventTypeId);

            // IX_AccountBased - Индекс для поиска по аккаунту - Category, ActualDate, StartDate, Id
            // IX_OwnerBased - Индекс для поиска по владельцу - OwnerId, Category, ActualDate, StartDate
            // IX_ForJoin - Индекс для склейки - OwnerId, EventTypeId, Importance, ActualDate 
            // IX_ForProcessing - Индекс для игнорирования и обработки - IsUserHandled, EventTypeId, VersionLong

            // Из-за особенностей EF 6 для каждого поля нужно в одном вызове сразу указать все индексы, в которых поле участвует, и позиции
            Property(t => t.OwnerId).HasColumnAnnotation("Index", new IndexAnnotation(
                new[]{
                    new IndexAttribute("IX_OwnerBased", 1),
                    new IndexAttribute("IX_ForJoin", 1)
                }
                ));

            Property(t => t.Category).HasColumnAnnotation("Index", new IndexAnnotation(
                new[]{
                    new IndexAttribute("IX_AccountBased", 2),
                    new IndexAttribute("IX_OwnerBased", 2),
                    new IndexAttribute("IX_ForJoin", 7)
                }
                ));

            Property(t => t.ActualDate).HasColumnAnnotation("Index", new IndexAnnotation(
                new[]{
                    new IndexAttribute("IX_AccountBased", 3),
                    new IndexAttribute("IX_OwnerBased", 3),
                    new IndexAttribute("IX_ForJoin", 8)
                }
                ));

            Property(t => t.StartDate).HasColumnAnnotation("Index", new IndexAnnotation(
                new[]{
                    new IndexAttribute("IX_AccountBased", 4),
                    new IndexAttribute("IX_OwnerBased", 4)
                }
                ));

            Property(t => t.EventTypeId).HasColumnAnnotation("Index", new IndexAnnotation(
                new[]{
                    new IndexAttribute("IX_ForJoin", 2),
                    new IndexAttribute("IX_ForProcessing", 2)
                }
                ));

            Property(t => t.Importance).HasColumnAnnotation("Index", new IndexAnnotation(
                new[]{
                    new IndexAttribute("IX_ForJoin", 3)
                }
                ));

            Property(t => t.JoinKeyHash).HasColumnAnnotation("Index", new IndexAnnotation(
                new[]{
                    new IndexAttribute("IX_ForJoin", 4)
                }
                ));

            Property(t => t.IsSpace).HasColumnAnnotation("Index", new IndexAnnotation(
                new[]{
                    new IndexAttribute("IX_ForJoin", 5)
                }
                ));

            Property(t => t.Version).HasColumnAnnotation("Index", new IndexAnnotation(
                new[]{
                    new IndexAttribute("IX_ForJoin", 6)
                }
                ));

            Property(t => t.IsUserHandled).HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute("IX_ForProcessing", 3)));
            
            Property(t => t.VersionLong).HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute("IX_ForProcessing", 4)));

            Property(t => t.Id).HasColumnAnnotation("Index", new IndexAnnotation(
                new[]{
                    new IndexAttribute("IX_AccountBased", 5)
                }
                ));
        }
    }
}
