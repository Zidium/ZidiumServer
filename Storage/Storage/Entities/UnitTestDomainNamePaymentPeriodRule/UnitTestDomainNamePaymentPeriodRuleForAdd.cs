using System;

namespace Zidium.Storage
{
    public class UnitTestDomainNamePaymentPeriodRuleForAdd
    {
        public Guid UnitTestId;

        public string Domain;

        public int AlarmDaysCount;

        public int WarningDaysCount;

        /// <summary>
        /// Код ошибки последнего выполнения
        /// </summary>
        public DomainNamePaymentPeriodErrorCode? LastRunErrorCode;

    }
}
