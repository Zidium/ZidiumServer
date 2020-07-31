using System;

namespace Zidium.Storage
{
    public class TariffLimitForAdd
    {
        /// <summary>
        /// Id
        /// </summary>
        public Guid Id;

        /// <summary>
        /// Название тарифа
        /// </summary>
        public string Name;

        /// <summary>
        /// Описание тарифа
        /// </summary>
        public string Description;

        /// <summary>
        /// Тип тарифа
        /// </summary>
        public TariffLimitType Type;

        /// <summary>
        /// Источник тарифа
        /// </summary>
        public TariffLimitSource Source;

        /// <summary>
        /// Количество вызовов api событий в день
        /// </summary>
        public int EventsRequestsPerDay;

        /// <summary>
        /// Время хранения событий, дней
        /// </summary>
        public int EventsMaxDays;

        /// <summary>
        /// Размер лога в день
        /// </summary>
        public long LogSizePerDay;

        /// <summary>
        /// Время хранения записей логов, дней
        /// </summary>
        public int LogMaxDays;

        /// <summary>
        /// Количество вызовов api проверок в день
        /// </summary>
        public int UnitTestsRequestsPerDay;

        /// <summary>
        /// Время хранения проверок, дней
        /// </summary>
        public int UnitTestsMaxDays;

        /// <summary>
        /// Количество вызовов api метрик в день
        /// </summary>
        public int MetricsRequestsPerDay;

        /// <summary>
        /// Время хранения метрик, дней
        /// </summary>
        public int MetricsMaxDays;

        /// <summary>
        /// Количество компонентов
        /// </summary>
        public int ComponentsMax;

        /// <summary>
        /// Количество типов компонентов
        /// </summary>
        public int ComponentTypesMax;

        /// <summary>
        /// Количество типов проверок
        /// </summary>
        public int UnitTestTypesMax;

        /// <summary>
        /// Количество проверок http без баннера
        /// </summary>
        public int HttpUnitTestsMaxNoBanner;

        /// <summary>
        /// Количество проверок
        /// </summary>
        public int UnitTestsMax;

        /// <summary>
        /// Максимальное количество метрик
        /// </summary>
        public int MetricsMax;

        /// <summary>
        /// Общий размер хранилища
        /// </summary>
        public long StorageSizeMax;

        /// <summary>
        /// Количество sms в день
        /// </summary>
        public int SmsPerDay;
    }
}
