using System.Data.Entity.ModelConfiguration;

namespace Zidium.Core.AccountsDb
{
    internal class LimitDataForUnitTestMapping : EntityTypeConfiguration<LimitDataForUnitTest>
    {
        public LimitDataForUnitTestMapping()
        {
            ToTable("LimitDatasForUnitTests");
            HasKey(t => t.Id);
            HasRequired(t => t.LimitData).WithMany(t => t.UnitTestData).HasForeignKey(t => t.LimitDataId);
            HasRequired(t => t.UnitTest).WithMany().HasForeignKey(t => t.UnitTestId);
        }
    }
}
