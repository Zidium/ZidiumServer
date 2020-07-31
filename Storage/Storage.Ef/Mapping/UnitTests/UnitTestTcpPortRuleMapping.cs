using System.Data.Entity.ModelConfiguration;

namespace Zidium.Storage.Ef.Mapping
{
    internal class UnitTestTcpPortRuleMapping : EntityTypeConfiguration<DbUnitTestTcpPortRule>
    {
        public UnitTestTcpPortRuleMapping()
        {
            ToTable("UnitTestTcpPortRules");
            HasKey(t => t.UnitTestId);
            HasRequired(t => t.UnitTest).WithOptional(t => t.TcpPortRule).WillCascadeOnDelete(false);
        }
    }
}
