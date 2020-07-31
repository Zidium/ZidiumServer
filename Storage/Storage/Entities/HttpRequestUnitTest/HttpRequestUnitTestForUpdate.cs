using System;

namespace Zidium.Storage
{
    /// <summary>
    /// Настройка проверки сайта с помощью серии http-запросов
    /// </summary>
    public class HttpRequestUnitTestForUpdate
    {
        public HttpRequestUnitTestForUpdate(Guid unitTestId)
        {
            UnitTestId = unitTestId;
            HasBanner = new ChangeTracker<bool>();
            LastBannerCheck = new ChangeTracker<DateTime?>();
        }

        /// <summary>
        /// Ссылка на юнит-тест (связь 1:1)
        /// </summary>
        public Guid UnitTestId { get; }

        /// <summary>
        /// Подтверждено наличие баннера
        /// </summary>
        public ChangeTracker<bool> HasBanner { get; }

        /// <summary>
        /// Дата последней проверки баннера
        /// </summary>
        public ChangeTracker<DateTime?> LastBannerCheck { get; }

    }
}