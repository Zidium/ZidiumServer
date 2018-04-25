namespace Zidium.Core.Api
{
    public class AccountUsedLimitsInstantDataInfo
    {
        /// <summary>
        /// Количество компонентов, использовано
        /// </summary>
        public int ComponentsCount { get; set; }

        /// <summary>
        /// Количество типов компонентов, использовано
        /// </summary>
        public int ComponentTypesCount { get; set; }

        /// <summary>
        /// Количество типов проверок, использовано
        /// </summary>
        public int UnitTestTypesCount { get; set; }

        /// <summary>
        /// Количество проверок http без баннера, использовано
        /// </summary>
        public int HttpUnitTestsNoBannerCount { get; set; }

        /// <summary>
        /// Количество проверок, использовано
        /// </summary>
        public int UnitTestsCount { get; set; }

        /// <summary>
        /// Количество метрик, использовано
        /// </summary>
        public int MetricsCount { get; set; }

    }
}
