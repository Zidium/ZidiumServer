using System.Data.Entity.ModelConfiguration;

namespace Zidium.Core.AccountsDb.Mapping
{
    internal class UnitTestPingRuleMapping : EntityTypeConfiguration<UnitTestPingRule>
    {
        public UnitTestPingRuleMapping()
        {
            ToTable("UnitTestPingRules");
            HasKey(t => t.UnitTestId);
            HasRequired(t => t.UnitTest).WithOptional(t => t.PingRule).WillCascadeOnDelete(false);
        }
    }
}
