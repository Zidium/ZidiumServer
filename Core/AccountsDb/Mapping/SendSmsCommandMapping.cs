using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration;

namespace Zidium.Core.AccountsDb.Mapping
{
    internal class SendSmsCommandMapping : EntityTypeConfiguration<SendSmsCommand>
    {
        public SendSmsCommandMapping()
        {
            ToTable("SendSmsCommand");
            HasKey(t => t.Id);
            Property(t => t.Body).IsRequired();
            Property(t => t.Phone).IsRequired().HasMaxLength(255);
            Property(t => t.ExternalId).HasMaxLength(255);

            Property(t => t.Status).HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute("IX_ForSend", 2)));

        }
    }
}
