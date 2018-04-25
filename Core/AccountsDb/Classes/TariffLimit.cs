using System;

namespace Zidium.Core.AccountsDb
{
    /// <summary>
    /// Ограничения по тарифу
    /// </summary>
    public class TariffLimit
    {
        /// <summary>
        /// Id
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Название тарифа
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Описание тарифа
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Тип тарифа
        /// </summary>
        public TariffLimitType Type { get; set; }

        /// <summary>
        /// Источник тарифа
        /// </summary>
        public TariffLimitSource Source { get; set; }

        /// <summary>
        /// Количество вызовов api событий в день
        /// </summary>
        public int EventsRequestsPerDay { get; set; }

        /// <summary>
        /// Время хранения событий, дней
        /// </summary>
        public int EventsMaxDays { get; set; }

        /// <summary>
        /// Размер лога в день
        /// </summary>
        public Int64 LogSizePerDay { get; set; }

        /// <summary>
        /// Время хранения записей логов, дней
        /// </summary>
        public int LogMaxDays { get; set; }

        /// <summary>
        /// Количество вызовов api проверок в день
        /// </summary>
        public int UnitTestsRequestsPerDay { get; set; }

        /// <summary>
        /// Время хранения проверок, дней
        /// </summary>
        public int UnitTestsMaxDays { get; set; }

        /// <summary>
        /// Количество вызовов api метрик в день
        /// </summary>
        public int MetricsRequestsPerDay { get; set; }

        /// <summary>
        /// Время хранения метрик, дней
        /// </summary>
        public int MetricsMaxDays { get; set; }

        /// <summary>
        /// Количество компонентов
        /// </summary>
        public int ComponentsMax { get; set; }

        /// <summary>
        /// Количество типов компонентов
        /// </summary>
        public int ComponentTypesMax { get; set; }

        /// <summary>
        /// Количество типов проверок
        /// </summary>
        public int UnitTestTypesMax { get; set; }

        /// <summary>
        /// Количество проверок http без баннера
        /// </summary>
        public int HttpUnitTestsMaxNoBanner { get; set; }

        /// <summary>
        /// Количество проверок
        /// </summary>
        public int UnitTestsMax { get; set; }

        /// <summary>
        /// Максимальное количество метрик
        /// </summary>
        public int MetricsMax { get; set; }

        /// <summary>
        /// Общий размер хранилища
        /// </summary>
        public Int64 StorageSizeMax { get; set; }

        /// <summary>
        /// Количество sms в день
        /// </summary>
        public int SmsPerDay { get; set; }
    }
}
