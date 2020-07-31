using System;

namespace Zidium.Storage
{
    /// <summary>
    /// Ограничения по тарифу
    /// </summary>
    public class TariffLimitForRead
    {
        public TariffLimitForRead(
            Guid id, 
            string name, 
            string description, 
            TariffLimitType type, 
            TariffLimitSource source,
            int eventsRequestsPerDay, 
            int eventsMaxDays, 
            long logSizePerDay, 
            int logMaxDays, 
            int unitTestsRequestsPerDay, 
            int unitTestsMaxDays, 
            int metricsRequestsPerDay, 
            int metricsMaxDays, 
            int componentsMax, 
            int componentTypesMax, 
            int unitTestTypesMax, 
            int httpUnitTestsMaxNoBanner, 
            int unitTestsMax, 
            int metricsMax, 
            long storageSizeMax, 
            int smsPerDay)
        {
            Id = id;
            Name = name;
            Description = description;
            Type = type;
            Source = source;
            EventsRequestsPerDay = eventsRequestsPerDay;
            EventsMaxDays = eventsMaxDays;
            LogSizePerDay = logSizePerDay;
            LogMaxDays = logMaxDays;
            UnitTestsRequestsPerDay = unitTestsRequestsPerDay;
            UnitTestsMaxDays = unitTestsMaxDays;
            MetricsRequestsPerDay = metricsRequestsPerDay;
            MetricsMaxDays = metricsMaxDays;
            ComponentsMax = componentsMax;
            ComponentTypesMax = componentTypesMax;
            UnitTestTypesMax = unitTestTypesMax;
            HttpUnitTestsMaxNoBanner = httpUnitTestsMaxNoBanner;
            UnitTestsMax = unitTestsMax;
            MetricsMax = metricsMax;
            StorageSizeMax = storageSizeMax;
            SmsPerDay = smsPerDay;
        }

        /// <summary>
        /// Id
        /// </summary>
        public Guid Id { get; }

        /// <summary>
        /// Название тарифа
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Описание тарифа
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// Тип тарифа
        /// </summary>
        public TariffLimitType Type { get; }

        /// <summary>
        /// Источник тарифа
        /// </summary>
        public TariffLimitSource Source { get; }

        /// <summary>
        /// Количество вызовов api событий в день
        /// </summary>
        public int EventsRequestsPerDay { get; }

        /// <summary>
        /// Время хранения событий, дней
        /// </summary>
        public int EventsMaxDays { get; }

        /// <summary>
        /// Размер лога в день
        /// </summary>
        public long LogSizePerDay { get; }

        /// <summary>
        /// Время хранения записей логов, дней
        /// </summary>
        public int LogMaxDays { get; }

        /// <summary>
        /// Количество вызовов api проверок в день
        /// </summary>
        public int UnitTestsRequestsPerDay { get; }

        /// <summary>
        /// Время хранения проверок, дней
        /// </summary>
        public int UnitTestsMaxDays { get; }

        /// <summary>
        /// Количество вызовов api метрик в день
        /// </summary>
        public int MetricsRequestsPerDay { get; }

        /// <summary>
        /// Время хранения метрик, дней
        /// </summary>
        public int MetricsMaxDays { get; }

        /// <summary>
        /// Количество компонентов
        /// </summary>
        public int ComponentsMax { get; }

        /// <summary>
        /// Количество типов компонентов
        /// </summary>
        public int ComponentTypesMax { get; }

        /// <summary>
        /// Количество типов проверок
        /// </summary>
        public int UnitTestTypesMax { get; }

        /// <summary>
        /// Количество проверок http без баннера
        /// </summary>
        public int HttpUnitTestsMaxNoBanner { get; }

        /// <summary>
        /// Количество проверок
        /// </summary>
        public int UnitTestsMax { get; }

        /// <summary>
        /// Максимальное количество метрик
        /// </summary>
        public int MetricsMax { get; }

        /// <summary>
        /// Общий размер хранилища
        /// </summary>
        public long StorageSizeMax { get; }

        /// <summary>
        /// Количество sms в день
        /// </summary>
        public int SmsPerDay { get; }

        public TariffLimitForUpdate GetForUpdate()
        {
            return new TariffLimitForUpdate(Id);
        }
    }
}
