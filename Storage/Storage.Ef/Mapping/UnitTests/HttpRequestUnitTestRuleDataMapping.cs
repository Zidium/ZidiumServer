using System.Data.Entity.ModelConfiguration;

namespace Zidium.Storage.Ef.Mapping
{
    internal class HttpRequestUnitTestRuleDataMapping : EntityTypeConfiguration<DbHttpRequestUnitTestRuleData>
    {
        public HttpRequestUnitTestRuleDataMapping()
        {
            ToTable("HttpRequestUnitTestRuleDatas");
            HasKey(t => t.Id);
            Property(t => t.Id).HasColumnName("Id");
            Property(t => t.Key).HasColumnName("Key").HasMaxLength(500);
            Property(t => t.Value).HasColumnName("Value");
            Property(t => t.Type).HasColumnName("Type");
            HasRequired(t => t.Rule).WithMany(t => t.Datas).HasForeignKey(t => t.RuleId).WillCascadeOnDelete(false);
        }
    }
}
