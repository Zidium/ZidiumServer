using System;

namespace Zidium.Storage
{
    public class UnitTestDomainNamePaymentPeriodRuleForRead
    {
        public UnitTestDomainNamePaymentPeriodRuleForRead(
            Guid unitTestId, 
            string domain, 
            int alarmDaysCount, 
            int warningDaysCount, 
            DomainNamePaymentPeriodErrorCode? lastRunErrorCode)
        {
            UnitTestId = unitTestId;
            Domain = domain;
            AlarmDaysCount = alarmDaysCount;
            WarningDaysCount = warningDaysCount;
            LastRunErrorCode = lastRunErrorCode;
        }

        public Guid UnitTestId { get; }

        public string Domain { get; }

        public int AlarmDaysCount { get; }

        public int WarningDaysCount { get; }

        /// <summary>
        /// Код ошибки последнего выполнения
        /// </summary>
        public DomainNamePaymentPeriodErrorCode? LastRunErrorCode { get; }

        public UnitTestDomainNamePaymentPeriodRuleForUpdate GetForUpdate()
        {
            return new UnitTestDomainNamePaymentPeriodRuleForUpdate(UnitTestId);
        }

    }
}
