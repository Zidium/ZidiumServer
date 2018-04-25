using System.Data.Entity.ModelConfiguration;

namespace Zidium.Core.AccountsDb.Mapping
{
    internal class LogConfigMapping : EntityTypeConfiguration<LogConfig>
    {
        public LogConfigMapping()
        {
            HasKey(t => t.ComponentId);
            ToTable("LogConfigs");
            Property(t => t.Enabled).HasColumnName("Enabled");
            Property(t => t.LastUpdateDate).HasColumnName("LastUpdateDate");
            Property(t => t.IsDebugEnabled).HasColumnName("IsDebugEnabled");
            Property(t => t.IsTraceEnabled).HasColumnName("IsTraceEnabled");
            Property(t => t.IsInfoEnabled).HasColumnName("IsInfoEnabled");
            Property(t => t.IsWarningEnabled).HasColumnName("IsWarningEnabled");
            Property(t => t.IsErrorEnabled).HasColumnName("IsErrorEnabled");
            Property(t => t.IsFatalEnabled).HasColumnName("IsFatalEnabled");
            HasRequired(t => t.Component).WithOptional(t => t.LogConfig);
        }
    }
}
