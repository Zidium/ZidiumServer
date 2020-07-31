using System.Data.Entity.ModelConfiguration;

namespace Zidium.Storage.Ef.Mapping
{
    internal class HttpRequestUnitTestMapping : EntityTypeConfiguration<DbHttpRequestUnitTest>
    {
        public HttpRequestUnitTestMapping()
        {
            ToTable("HttpRequestUnitTests");
            HasKey(t => t.UnitTestId);
            Property(t => t.UnitTestId).HasColumnName("UnitTestId");
            HasRequired(t => t.UnitTest).WithOptional(t => t.HttpRequestUnitTest);
        }
    }
}
