using System;

namespace Zidium.Storage.Ef
{
    public class DbUnitTestDomainNamePaymentPeriodRule
    {
        public Guid UnitTestId { get; set; }

        public virtual DbUnitTest UnitTest { get; set; }

        public string Domain { get; set; }

        public int AlarmDaysCount { get; set; }

        public int WarningDaysCount { get; set; }

        /// <summary>
        /// Код ошибки последнего выполнения
        /// </summary>
        public DomainNamePaymentPeriodErrorCode? LastRunErrorCode { get; set; }

    }
}
