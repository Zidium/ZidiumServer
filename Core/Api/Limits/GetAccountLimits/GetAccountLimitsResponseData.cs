namespace Zidium.Core.Api
{
    public class GetAccountLimitsResponseData
    {
        /// <summary>
        /// Жесткие лимиты
        /// </summary>
        public AccountTotalLimitsDataInfo Hard { get; set; }

        /// <summary>
        /// Мягкие лимиты
        /// </summary>
        public AccountTotalLimitsDataInfo Soft { get; set; }

        /// <summary>
        /// Использовано сегодня
        /// </summary>
        public AccountUsedLimitsTodayDataInfo UsedToday { get; set; }

        /// <summary>
        /// Статистика использования объектов
        /// </summary>
        public AccountUsedLimitsInstantDataInfo UsedInstant { get; set; }

        /// <summary>
        /// Архив статистики запросов
        /// </summary>
        public AccountUsedLimitsOverallDataInfo UsedOverall { get; set; }
    }
}
