using System;
using System.Collections.Generic;

namespace Zidium.Core.AccountsDb
{
    /// <summary>
    /// Настройка проверки сайта с помощью серии http-запросов
    /// </summary>
    public class HttpRequestUnitTest
    {
        public HttpRequestUnitTest()
        {
            Rules = new HashSet<HttpRequestUnitTestRule>();    
        }

        /// <summary>
        /// Ссылка на юнит-тест (связь 1:1)
        /// </summary>
        public Guid UnitTestId { get; set; }

        /// <summary>
        /// Юнит-тест
        /// </summary>
        public virtual UnitTest UnitTest { get; set; }

        /// <summary>
        /// Правила
        /// </summary>
        public virtual ICollection<HttpRequestUnitTestRule> Rules { get; set; }

        /// <summary>
        /// Подтверждено наличие баннера
        /// </summary>
        public bool HasBanner { get; set; }

        /// <summary>
        /// Дата последней проверки баннера
        /// </summary>
        public DateTime? LastBannerCheck { get; set; }
    }
}
