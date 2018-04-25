using System.Data.Entity.ModelConfiguration;

namespace Zidium.Core.AccountsDb.Mapping
{
    internal class HttpRequestUnitTestMapping : EntityTypeConfiguration<HttpRequestUnitTest>
    {
        public HttpRequestUnitTestMapping()
        {
            ToTable("HttpRequestUnitTests");
            HasKey(t => t.UnitTestId);
            Property(t => t.UnitTestId).HasColumnName("UnitTestId");
            Property(t => t.ProcessAllRulesOnError).HasColumnName("ProcessAllRulesOnError");
            HasRequired(t => t.UnitTest).WithOptional(t => t.HttpRequestUnitTest);
        }
    }
}
