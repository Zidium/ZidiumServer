using System.Data.Entity.ModelConfiguration;

namespace Zidium.Storage.Ef.Mapping
{
    internal class TokenMapping : EntityTypeConfiguration<DbToken>
    {
        public TokenMapping()
        {
            ToTable("Tokens");
            HasKey(t => t.Id);
            Property(t => t.SecurityStamp).HasMaxLength(50);
            HasRequired(t => t.User).WithMany().HasForeignKey(t => t.UserId);
        }
    }
}
