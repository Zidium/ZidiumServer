using System;

namespace Zidium.Storage
{
    public class UnitTestDomainNamePaymentPeriodRuleForUpdate
    {
        public UnitTestDomainNamePaymentPeriodRuleForUpdate(Guid unitTestId)
        {
            UnitTestId = unitTestId;
            Domain = new ChangeTracker<string>();
            AlarmDaysCount = new ChangeTracker<int>();
            WarningDaysCount = new ChangeTracker<int>();
            LastRunErrorCode = new ChangeTracker<DomainNamePaymentPeriodErrorCode?>();
        }

        public Guid UnitTestId { get; }

        public ChangeTracker<string> Domain { get; }

        public ChangeTracker<int> AlarmDaysCount { get; }

        public ChangeTracker<int> WarningDaysCount { get; }

        /// <summary>
        /// Код ошибки последнего выполнения
        /// </summary>
        public ChangeTracker<DomainNamePaymentPeriodErrorCode?> LastRunErrorCode { get; }

    }
}