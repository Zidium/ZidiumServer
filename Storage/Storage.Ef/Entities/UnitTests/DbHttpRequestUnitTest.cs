using System;
using System.Collections.Generic;

namespace Zidium.Storage.Ef
{
    /// <summary>
    /// Настройка проверки сайта с помощью серии http-запросов
    /// </summary>
    public class DbHttpRequestUnitTest
    {
        public DbHttpRequestUnitTest()
        {
            Rules = new HashSet<DbHttpRequestUnitTestRule>();    
        }

        /// <summary>
        /// Ссылка на юнит-тест (связь 1:1)
        /// </summary>
        public Guid UnitTestId { get; set; }

        /// <summary>
        /// Юнит-тест
        /// </summary>
        public virtual DbUnitTest UnitTest { get; set; }

        /// <summary>
        /// Правила
        /// </summary>
        public virtual ICollection<DbHttpRequestUnitTestRule> Rules { get; set; }

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
