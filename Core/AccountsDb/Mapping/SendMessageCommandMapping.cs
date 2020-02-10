using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration;

namespace Zidium.Core.AccountsDb.Mapping
{
    internal class SendMessageCommandMapping : EntityTypeConfiguration<SendMessageCommand>
    {
        public SendMessageCommandMapping()
        {
            ToTable("SendMessageCommand");
            HasKey(t => t.Id);
            Property(t => t.Body).IsRequired();
            Property(t => t.To).IsRequired().HasMaxLength(255);

            Property(t => t.Channel).HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute("IX_ForSend", 1)));
            Property(t => t.Status).HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute("IX_ForSend", 2)));

        }
    }
}