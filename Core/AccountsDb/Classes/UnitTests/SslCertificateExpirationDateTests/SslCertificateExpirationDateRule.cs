using System;

namespace Zidium.Core.AccountsDb
{
    public class UnitTestSslCertificateExpirationDateRule
    {
        public Guid UnitTestId { get; set; }

        public virtual UnitTest UnitTest { get; set; }

        public string Url { get; set; }

        public int AlarmDaysCount { get; set; }

        public int WarningDaysCount { get; set; }

        /// <summary>
        /// Код ошибки последнего выполнения
        /// </summary>
        public SslCertificateExpirationDateErrorCode? LastRunErrorCode { get; set; }
    }
}
