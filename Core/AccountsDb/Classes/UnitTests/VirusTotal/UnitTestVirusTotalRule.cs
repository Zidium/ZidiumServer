using System;

namespace Zidium.Core.AccountsDb
{
    /// <summary>
    /// Настройка проверки virus total
    /// </summary>
    public class UnitTestVirusTotalRule
    {
        public Guid UnitTestId { get; set; }

        public virtual UnitTest UnitTest { get; set; }

        public string Url { get; set; }

        public VirusTotalStep NextStep { get; set; }

        public DateTime? ScanTime { get; set; }

        public string ScanId { get; set; }

        /// <summary>
        /// Код ошибки последнего выполнения
        /// </summary>
        public VirusTotalErrorCode? LastRunErrorCode { get; set; }
    }
}
