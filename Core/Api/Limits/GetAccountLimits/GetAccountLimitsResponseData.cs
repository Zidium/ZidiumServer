namespace Zidium.Core.Api
{
    public class GetAccountLimitsResponseData
    {
        /// <summary>
        /// Использовано сегодня
        /// </summary>
        public AccountUsedLimitsTodayDataInfo UsedToday { get; set; }

        /// <summary>
        /// Архив статистики запросов
        /// </summary>
        public AccountUsedLimitsOverallDataInfo UsedOverall { get; set; }
    }
}
