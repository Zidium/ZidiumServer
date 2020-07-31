using System;

namespace Zidium.Storage
{
    public class UnitTestSslCertificateExpirationDateRuleForRead
    {
        public UnitTestSslCertificateExpirationDateRuleForRead(
            Guid unitTestId,
            string url,
            int alarmDaysCount,
            int warningDaysCount,
            SslCertificateExpirationDateErrorCode? lastRunErrorCode)
        {
            UnitTestId = unitTestId;
            Url = url;
            AlarmDaysCount = alarmDaysCount;
            WarningDaysCount = warningDaysCount;
            LastRunErrorCode = lastRunErrorCode;
        }

        public Guid UnitTestId { get; }

        public string Url { get; }

        public int AlarmDaysCount { get; }

        public int WarningDaysCount { get; }

        /// <summary>
        /// Код ошибки последнего выполнения
        /// </summary>
        public SslCertificateExpirationDateErrorCode? LastRunErrorCode { get; }

        public UnitTestSslCertificateExpirationDateRuleForUpdate GetForUpdate()
        {
            return new UnitTestSslCertificateExpirationDateRuleForUpdate(UnitTestId);
        }

    }
}
