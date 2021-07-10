using System;

namespace Zidium.Storage
{
    public class UnitTestSslCertificateExpirationDateRuleForAdd
    {
        public Guid UnitTestId;

        public string Url;

        public int AlarmDaysCount;

        public int WarningDaysCount;

        /// <summary>
        /// Код ошибки последнего выполнения
        /// </summary>
        public SslCertificateExpirationDateErrorCode? LastRunErrorCode;

    }
}
