//using System.Data.Entity.ModelConfiguration;

//namespace Zidium.Core.AccountsDb.Mapping
//{
//    internal class UnitTestResultMapping : EntityTypeConfiguration<UnitTestResult>
//    {
//        public UnitTestResultMapping()
//        {
//            HasKey(t => t.UnitTestId);
//            ToTable("UnitTestResults");
//            Property(t => t.UnitTestId).HasColumnName("UnitTestId");
//            Property(t => t.Message).HasColumnName("Message").HasMaxLength(255);
//            Property(t => t.ActualDate).HasColumnName("ActualDate");
//            Ignore(t => t.Duration);
//            Property(t => t.EndDate).HasColumnName("EndDate");
//            Property(t => t.NextDate).HasColumnName("NextDate");
//            Property(t => t.LastEventId).HasColumnName("LastEventId");
//            Property(t => t.StatusEventId).HasColumnName("StatusEventId");
//            Property(t => t.Status).HasColumnName("Status");
//            Property(t => t.HasSignal).HasColumnName("HasSignal");
//            Property(t => t.StartDate).HasColumnName("BeginDate");
//            HasRequired(t => t.UnitTest).WithOptional(t => t.UnitTestResult);
//        }
//    }
//}
