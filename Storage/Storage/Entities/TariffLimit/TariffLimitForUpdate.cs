using System;

namespace Zidium.Storage
{
    /// <summary>
    /// Ограничения по тарифу
    /// </summary>
    public class TariffLimitForUpdate
    {
        public TariffLimitForUpdate(Guid id)
        {
            Id = id;
            EventsRequestsPerDay = new ChangeTracker<int>();
            EventsMaxDays = new ChangeTracker<int>();
            LogSizePerDay = new ChangeTracker<long>();
            LogMaxDays = new ChangeTracker<int>();
            UnitTestsRequestsPerDay = new ChangeTracker<int>();
            UnitTestsMaxDays = new ChangeTracker<int>();
            MetricsRequestsPerDay = new ChangeTracker<int>();
            MetricsMaxDays = new ChangeTracker<int>();
            ComponentsMax = new ChangeTracker<int>();
            ComponentTypesMax = new ChangeTracker<int>();
            UnitTestTypesMax = new ChangeTracker<int>();
            HttpUnitTestsMaxNoBanner = new ChangeTracker<int>();
            UnitTestsMax = new ChangeTracker<int>();
            MetricsMax = new ChangeTracker<int>();
            StorageSizeMax = new ChangeTracker<long>();
            SmsPerDay = new ChangeTracker<int>();
        }

        /// <summary>
        /// Id
        /// </summary>
        public Guid Id { get; }

        /// <summary>
        /// Количество вызовов api событий в день
        /// </summary>
        public ChangeTracker<int> EventsRequestsPerDay { get; }

        /// <summary>
        /// Время хранения событий, дней
        /// </summary>
        public ChangeTracker<int> EventsMaxDays { get; }

        /// <summary>
        /// Размер лога в день
        /// </summary>
        public ChangeTracker<long> LogSizePerDay { get; }

        /// <summary>
        /// Время хранения записей логов, дней
        /// </summary>
        public ChangeTracker<int> LogMaxDays { get; }

        /// <summary>
        /// Количество вызовов api проверок в день
        /// </summary>
        public ChangeTracker<int> UnitTestsRequestsPerDay { get; }

        /// <summary>
        /// Время хранения проверок, дней
        /// </summary>
        public ChangeTracker<int> UnitTestsMaxDays { get; }

        /// <summary>
        /// Количество вызовов api метрик в день
        /// </summary>
        public ChangeTracker<int> MetricsRequestsPerDay { get; }

        /// <summary>
        /// Время хранения метрик, дней
        /// </summary>
        public ChangeTracker<int> MetricsMaxDays { get; }

        /// <summary>
        /// Количество компонентов
        /// </summary>
        public ChangeTracker<int> ComponentsMax { get; }

        /// <summary>
        /// Количество типов компонентов
        /// </summary>
        public ChangeTracker<int> ComponentTypesMax { get; }

        /// <summary>
        /// Количество типов проверок
        /// </summary>
        public ChangeTracker<int> UnitTestTypesMax { get; }

        /// <summary>
        /// Количество проверок http без баннера
        /// </summary>
        public ChangeTracker<int> HttpUnitTestsMaxNoBanner { get; }

        /// <summary>
        /// Количество проверок
        /// </summary>
        public ChangeTracker<int> UnitTestsMax { get; }

        /// <summary>
        /// Максимальное количество метрик
        /// </summary>
        public ChangeTracker<int> MetricsMax { get; }

        /// <summary>
        /// Общий размер хранилища
        /// </summary>
        public ChangeTracker<long> StorageSizeMax { get; }

        /// <summary>
        /// Количество sms в день
        /// </summary>
        public ChangeTracker<int> SmsPerDay { get; }

    }
}