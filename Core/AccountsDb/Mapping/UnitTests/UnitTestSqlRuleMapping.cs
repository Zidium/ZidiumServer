using System.Data.Entity.ModelConfiguration;

namespace Zidium.Core.AccountsDb.Mapping
{
    internal class UnitTestSqlRuleMapping : EntityTypeConfiguration<UnitTestSqlRule>
    {
        public UnitTestSqlRuleMapping()
        {
            ToTable("UnitTestSqlRules");
            HasKey(t => t.UnitTestId);
            Property(t => t.UnitTestId).HasColumnName("UnitTestId");
            Property(t => t.ConnectionString).HasColumnName("ConnectionString").HasMaxLength(500);
            Property(t => t.CommandTimeoutMs).HasColumnName("CommandTimeoutMs");
            Property(t => t.OpenConnectionTimeoutMs).HasColumnName("OpenConnectionTimeoutMs");
            Property(t => t.Provider).HasColumnName("Provider");
            Property(t => t.Query).HasColumnName("Query");
            HasRequired(t => t.UnitTest).WithOptional(t => t.SqlRule).WillCascadeOnDelete(false);
        }
    }
}
