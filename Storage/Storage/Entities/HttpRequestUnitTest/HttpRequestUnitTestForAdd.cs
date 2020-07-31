using System;

namespace Zidium.Storage
{
    public class HttpRequestUnitTestForAdd
    {
        /// <summary>
        /// Ссылка на юнит-тест (связь 1:1)
        /// </summary>
        public Guid UnitTestId;

        /// <summary>
        /// Подтверждено наличие баннера
        /// </summary>
        public bool HasBanner;

        /// <summary>
        /// Дата последней проверки баннера
        /// </summary>
        public DateTime? LastBannerCheck;
    }
}
