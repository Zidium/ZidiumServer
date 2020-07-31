using System.Data.Entity.ModelConfiguration;

namespace Zidium.Storage.Ef.Mapping
{
    internal class LimitDataForUnitTestMapping : EntityTypeConfiguration<DbLimitDataForUnitTest>
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
