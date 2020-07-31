using System.Data.Entity.ModelConfiguration;

namespace Zidium.Storage.Ef.Mapping
{
    internal class UnitTestPingRuleMapping : EntityTypeConfiguration<DbUnitTestPingRule>
    {
        public UnitTestPingRuleMapping()
        {
            ToTable("UnitTestPingRules");
            HasKey(t => t.UnitTestId);
            HasRequired(t => t.UnitTest).WithOptional(t => t.PingRule).WillCascadeOnDelete(false);
        }
    }
}
