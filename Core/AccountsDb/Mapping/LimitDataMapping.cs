using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration;

namespace Zidium.Core.AccountsDb
{
    internal class LimitDataMapping : EntityTypeConfiguration<LimitData>
    {
        public LimitDataMapping()
        {
            ToTable("LimitDatas");
            HasKey(t => t.Id);

            Property(t => t.Type).HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute("IX_AccountId", 2)));
            Property(t => t.BeginDate).HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute("IX_AccountId", 3)));
        }
    }
}
