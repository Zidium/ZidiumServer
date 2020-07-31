using System;

namespace Zidium.Storage
{
    /// <summary>
    /// Настройка проверки сайта с помощью серии http-запросов
    /// </summary>
    public class HttpRequestUnitTestForRead
    {
        public HttpRequestUnitTestForRead(
            Guid unitTestId, 
            bool hasBanner, 
            DateTime? lastBannerCheck)
        {
            UnitTestId = unitTestId;
            HasBanner = hasBanner;
            LastBannerCheck = lastBannerCheck;
        }

        /// <summary>
        /// Ссылка на юнит-тест (связь 1:1)
        /// </summary>
        public Guid UnitTestId { get; }

        /// <summary>
        /// Подтверждено наличие баннера
        /// </summary>
        public bool HasBanner { get; }

        /// <summary>
        /// Дата последней проверки баннера
        /// </summary>
        public DateTime? LastBannerCheck { get; }

        public HttpRequestUnitTestForUpdate GetForUpdate()
        {
            return new HttpRequestUnitTestForUpdate(UnitTestId);
        }

    }
}
