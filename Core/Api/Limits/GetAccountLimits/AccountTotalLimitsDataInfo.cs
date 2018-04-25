using System;

namespace Zidium.Core.Api
{
    public class AccountTotalLimitsDataInfo
    {
        // Лимиты на день

        /// <summary>
        /// Количество вызовов api событий в день, всего
        /// </summary>
        public int EventRequestsPerDay { get; set; }

        /// <summary>
        /// Размер лога в день, всего, байт
        /// </summary>
        public Int64 LogSizePerDay { get; set; }

        /// <summary>
        /// Количество вызовов api метрик в день
        /// </summary>
        public int MetricRequestsPerDay { get; set; }

        /// <summary>
        /// Количество вызовов api проверок в день
        /// </summary>
        public int UnitTestsRequestsPerDay { get; set; }

        /// <summary>
        /// Количество sms в день
        /// </summary>
        public int SmsPerDay { get; set; }

        // Лимиты общие

        /// <summary>
        /// Количество компонентов, всего
        /// </summary>
        public int ComponentsMax { get; set; }

        /// <summary>
        /// Количество типов компонентов, всего
        /// </summary>
        public int ComponentTypesMax { get; set; }

        /// <summary>
        /// Количество типов проверок, всего
        /// </summary>
        public int UnitTestTypesMax { get; set; }

        /// <summary>
        /// Количество проверок http без баннера, всего
        /// </summary>
        public int HttpChecksMaxNoBanner { get; set; }

        /// <summary>
        /// Количество проверок, всего
        /// </summary>
        public int UnitTestsMax { get; set; }

        /// <summary>
        /// Максимальное количество метрик
        /// </summary>
        public int MetricsMax { get; set; }

        /// <summary>
        /// Максимальный размер хранилища
        /// </summary>
        public Int64 StorageSizeMax { get; set; }

        // Которые не тратятся

        /// <summary>
        /// Время хранения событий, дней
        /// </summary>
        public int EventsMaxDays { get; set; }

        /// <summary>
        /// Время хранения записей логов, дней
        /// </summary>
        public int LogMaxDays { get; set; }

        /// <summary>
        /// Время хранения метрик, дней
        /// </summary>
        public int MetricsMaxDays { get; set; }

        /// <summary>
        /// Время хранения проверок, дней
        /// </summary>
        public int UnitTestsMaxDays { get; set; }

    }
}
