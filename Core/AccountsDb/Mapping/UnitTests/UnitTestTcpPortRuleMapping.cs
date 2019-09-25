using System.Data.Entity.ModelConfiguration;

namespace Zidium.Core.AccountsDb.Mapping
{
    internal class UnitTestTcpPortRuleMapping : EntityTypeConfiguration<UnitTestTcpPortRule>
    {
        public UnitTestTcpPortRuleMapping()
        {
            ToTable("UnitTestTcpPortRules");
            HasKey(t => t.UnitTestId);
            HasRequired(t => t.UnitTest).WithOptional(t => t.TcpPortRule).WillCascadeOnDelete(false);
        }
    }
}
