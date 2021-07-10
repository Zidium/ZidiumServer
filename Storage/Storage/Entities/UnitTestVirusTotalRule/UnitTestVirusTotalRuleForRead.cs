using System;

namespace Zidium.Storage
{
    /// <summary>
    /// Настройка проверки virus total
    /// </summary>
    public class UnitTestVirusTotalRuleForRead
    {
        public UnitTestVirusTotalRuleForRead(
            Guid unitTestId,
            string url, 
            VirusTotalStep nextStep, 
            DateTime? scanTime, 
            string scanId, 
            VirusTotalErrorCode? lastRunErrorCode)
        {
            UnitTestId = unitTestId;
            Url = url;
            NextStep = nextStep;
            ScanTime = scanTime;
            ScanId = scanId;
            LastRunErrorCode = lastRunErrorCode;
        }

        public Guid UnitTestId { get; }

        public string Url { get; }

        public VirusTotalStep NextStep { get; }

        public DateTime? ScanTime { get; }

        public string ScanId { get; }

        /// <summary>
        /// Код ошибки последнего выполнения
        /// </summary>
        public VirusTotalErrorCode? LastRunErrorCode { get; }

        public UnitTestVirusTotalRuleForUpdate GetForUpdate()
        {
            return new UnitTestVirusTotalRuleForUpdate(UnitTestId);
        }

    }
}
