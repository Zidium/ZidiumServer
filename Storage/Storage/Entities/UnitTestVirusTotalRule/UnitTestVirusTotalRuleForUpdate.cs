using System;

namespace Zidium.Storage
{
    /// <summary>
    /// Настройка проверки virus total
    /// </summary>
    public class UnitTestVirusTotalRuleForUpdate
    {
        public UnitTestVirusTotalRuleForUpdate(Guid unitTestId)
        {
            UnitTestId = unitTestId;
            Url = new ChangeTracker<string>();
            NextStep = new ChangeTracker<VirusTotalStep>();
            ScanTime = new ChangeTracker<DateTime?>();
            ScanId = new ChangeTracker<string>();
            LastRunErrorCode = new ChangeTracker<VirusTotalErrorCode?>();
        }

        public Guid UnitTestId { get; }

        public ChangeTracker<string> Url { get; }

        public ChangeTracker<VirusTotalStep> NextStep { get; }

        public ChangeTracker<DateTime?> ScanTime { get; }

        public ChangeTracker<string> ScanId { get; }

        /// <summary>
        /// Код ошибки последнего выполнения
        /// </summary>
        public ChangeTracker<VirusTotalErrorCode?> LastRunErrorCode { get; }

    }
}