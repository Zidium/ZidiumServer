using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration;

namespace Zidium.Storage.Ef.Mapping
{
    internal class SendEmailCommandMapping : EntityTypeConfiguration<DbSendEmailCommand>
    {
        public SendEmailCommandMapping()
        {
            ToTable("SendEmailCommand");
            HasKey(t => t.Id);
            Property(t => t.Body).IsRequired();
            Property(t => t.From).HasMaxLength(100);
            Property(t => t.To).IsRequired().HasMaxLength(255);
            Property(t => t.Subject).IsRequired().HasMaxLength(500);

            Property(t => t.Status).HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute("IX_ForSend", 2)));
        }
    }
}

