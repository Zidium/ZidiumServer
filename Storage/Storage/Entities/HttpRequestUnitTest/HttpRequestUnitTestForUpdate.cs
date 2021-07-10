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
        }

        /// <summary>
        /// Ссылка на юнит-тест (связь 1:1)
        /// </summary>
        public Guid UnitTestId { get; }

    }
}