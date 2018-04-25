using System;

namespace Zidium.Core.Api
{
    public class TariffConfigurationInfo
    {
        /// <summary>
        /// Дополнительное место в хранилище, байт
        /// </summary>
        public long AdditionalStorageBytes { get; set; }

        /// <summary>
        /// Основное место в хранилище, ГБ
        /// </summary>
        public long StorageMinBytes { get; set; }

        /// <summary>
        /// Количество запросов событий в день
        /// </summary>
        public int EventRequestsPerDay { get; set; }

        /// <summary>
        /// Время жизни архива, дней
        /// </summary>
        public int EventsArhiveDays { get; set; }

        /// <summary>
        /// Количество запросов проверок
        /// </summary>
        public int UnitTestRequestsPerDay { get; set; }

        /// <summary>
        /// Время жизни архива проверок, дней
        /// </summary>
        public int UnitTestsArhiveDays { get; set; }

        /// <summary>
        /// Количество запросов метрик
        /// </summary>
        public int MetricRequestsPerDay { get; set; }

        /// <summary>
        /// Время жизни архива метрик, дней
        /// </summary>
        public int MetricsArhiveDays { get; set; }

        /// <summary>
        /// Количество логов в день, байт
        /// </summary>
        public long LogsPerDayBytes { get; set; }

        /// <summary>
        /// Время жизни архива логов, дней
        /// </summary>
        public int LogArhiveDays { get; set; }

        /// <summary>
        /// Период оплаты, месяцев
        /// </summary>
        public int PeriodMonths { get; set; }

        /// <summary>
        /// Начало периода
        /// </summary>
        public DateTime FromDate { get; set; }

        /// <summary>
        /// Завершение периода
        /// </summary>
        public DateTime ToDate { get; set; }

        /// <summary>
        /// Стоимость периода со скидкой
        /// </summary>
        public decimal Cost { get; set; }

        /// <summary>
        /// Стоимость без скидки
        /// </summary>
        public decimal CostWithoutDiscount { get; set; }

        /// <summary>
        /// Скидка за период
        /// </summary>
        public double PeriodK { get; set; }

        /// <summary>
        /// Сумма списания c баланса
        /// </summary>
        public decimal WriteOffSum { get; set; }

        public double OneEventRequestPrice { get; set; }

        public double OneUnitTestRequestPrice { get; set; }

        public double OneMetricRequestPrice { get; set; }

        public double OneMbLogPrice { get; set; }
    }
}
