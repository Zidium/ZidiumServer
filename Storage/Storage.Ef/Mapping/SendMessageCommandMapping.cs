using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Zidium.Storage.Ef.Mapping
{
    internal class SendMessageCommandMapping : IEntityTypeConfiguration<DbSendMessageCommand>
    {
        public void Configure(EntityTypeBuilder<DbSendMessageCommand> builder)
        {
            builder.ToTable("SendMessageCommand");
            builder.HasKey(t => t.Id).IsClustered(false);
            builder.Property(t => t.Body).IsRequired();
            builder.Property(t => t.To).IsRequired().HasMaxLength(255);

            builder.HasIndex(t => new { t.Channel, t.Status }, "IX_ForSend");
        }
    }
}