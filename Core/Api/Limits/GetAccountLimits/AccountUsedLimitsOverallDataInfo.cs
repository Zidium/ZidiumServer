namespace Zidium.Core.Api
{
    public class AccountUsedLimitsOverallDataInfo
    {
        public AccountUsedLimitsPerDayDataInfo Total { get; set; }

        public AccountUsedLimitsArchiveDataInfo[] Archive { get; set; }
    }
}
