using System;

namespace Zidium.Storage
{
    public class UnitTestVirusTotalRuleForAdd
    {
        public Guid UnitTestId;

        public string Url;

        public VirusTotalStep NextStep;

        public DateTime? ScanTime;

        public string ScanId;

        /// <summary>
        /// Код ошибки последнего выполнения
        /// </summary>
        public VirusTotalErrorCode? LastRunErrorCode;
    }
}
