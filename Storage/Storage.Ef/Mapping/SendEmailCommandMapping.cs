using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Zidium.Storage.Ef.Mapping
{
    internal class SendEmailCommandMapping : IEntityTypeConfiguration<DbSendEmailCommand>
    {
        public void Configure(EntityTypeBuilder<DbSendEmailCommand> builder)
        {
            builder.ToTable("SendEmailCommand");
            builder.HasKey(t => t.Id).IsClustered(false);
            builder.Property(t => t.Body).IsRequired();
            builder.Property(t => t.From).HasMaxLength(100);
            builder.Property(t => t.To).IsRequired().HasMaxLength(255);
            builder.Property(t => t.Subject).IsRequired().HasMaxLength(500);

            builder.HasIndex(t => t.Status, "IX_ForSend");
        }
    }
}

