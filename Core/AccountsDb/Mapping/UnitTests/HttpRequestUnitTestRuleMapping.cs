using System.Data.Entity.ModelConfiguration;

namespace Zidium.Core.AccountsDb.Mapping
{
    internal class HttpRequestUnitTestRuleMapping : EntityTypeConfiguration<HttpRequestUnitTestRule>
    {
        public HttpRequestUnitTestRuleMapping()
        {
            ToTable("HttpRequestUnitTestRules");
            HasKey(t => t.Id);
            Property(t => t.Id).HasColumnName("Id");
            HasRequired(t => t.HttpRequestUnitTest).WithMany(t => t.Rules).HasForeignKey(t => t.HttpRequestUnitTestId);
            Property(t => t.SortNumber).HasColumnName("SortNumber");
            Property(t => t.DisplayName).HasColumnName("DisplayName").IsRequired().HasMaxLength(255);
            Property(t => t.Url).HasColumnName("Url").IsRequired().HasMaxLength(255);
            Property(t => t.Method).HasColumnName("Method").IsRequired();
            Property(t => t.ResponseCode).HasColumnName("ResponseCode");
            Property(t => t.SuccessHtml).HasColumnName("SuccessHtml").HasMaxLength(255);
            Property(t => t.ErrorHtml).HasColumnName("ErrorHtml").HasMaxLength(255);
            Property(t => t.TimeoutSeconds).HasColumnName("TimeoutSeconds");
            Property(t => t.LastRunErrorCode).HasColumnName("LastRunErrorCode");
            Property(t => t.LastRunErrorMessage).HasColumnName("LastRunErrorMessage");
            Property(t => t.LastRunTime).HasColumnName("LastRunTime");
            Property(t => t.LastRunDurationMs).HasColumnName("LastRunDurationMs");
            Property(t => t.IsDeleted).HasColumnName("IsDeleted");
            Property(t => t.MaxResponseSize).HasColumnName("MaxResponseSize");
        }
    }
}
