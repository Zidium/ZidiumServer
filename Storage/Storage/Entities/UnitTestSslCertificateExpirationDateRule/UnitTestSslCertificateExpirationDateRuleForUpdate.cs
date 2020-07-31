using System;

namespace Zidium.Storage
{
    public class UnitTestSslCertificateExpirationDateRuleForUpdate
    {
        public UnitTestSslCertificateExpirationDateRuleForUpdate(Guid unitTestId)
        {
            UnitTestId = unitTestId;
            Url = new ChangeTracker<string>();
            AlarmDaysCount = new ChangeTracker<int>();
            WarningDaysCount = new ChangeTracker<int>();
            LastRunErrorCode = new ChangeTracker<SslCertificateExpirationDateErrorCode?>();
        }

        public Guid UnitTestId { get; }

        public ChangeTracker<string> Url { get; }

        public ChangeTracker<int> AlarmDaysCount { get; }

        public ChangeTracker<int> WarningDaysCount { get; }

        /// <summary>
        /// Код ошибки последнего выполнения
        /// </summary>
        public ChangeTracker<SslCertificateExpirationDateErrorCode?> LastRunErrorCode { get; }

    }
}