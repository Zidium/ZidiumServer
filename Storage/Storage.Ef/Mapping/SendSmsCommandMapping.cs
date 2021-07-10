using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Zidium.Storage.Ef.Mapping
{
    internal class SendSmsCommandMapping : IEntityTypeConfiguration<DbSendSmsCommand>
    {
        public void Configure(EntityTypeBuilder<DbSendSmsCommand> builder)
        {
            builder.ToTable("SendSmsCommand");
            builder.HasKey(t => t.Id).IsClustered(false);
            builder.Property(t => t.Body).IsRequired();
            builder.Property(t => t.Phone).IsRequired().HasMaxLength(255);
            builder.Property(t => t.ExternalId).HasMaxLength(255);

            builder.HasIndex(t => t.Status, "IX_ForSend");
        }
    }
}
