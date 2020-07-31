using System;

namespace Zidium.Storage.Ef
{
    public class DbUnitTestSslCertificateExpirationDateRule
    {
        public Guid UnitTestId { get; set; }

        public virtual DbUnitTest UnitTest { get; set; }

        public string Url { get; set; }

        public int AlarmDaysCount { get; set; }

        public int WarningDaysCount { get; set; }

        /// <summary>
        /// Код ошибки последнего выполнения
        /// </summary>
        public SslCertificateExpirationDateErrorCode? LastRunErrorCode { get; set; }

    }
}
