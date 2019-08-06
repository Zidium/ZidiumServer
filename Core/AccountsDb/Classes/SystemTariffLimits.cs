using System;
using Zidium.Core.Api;

namespace Zidium.Core.AccountsDb
{
    public static class SystemTariffLimits
    {
        /// <summary>
        /// Базовый бесплатный аккаунт
        /// </summary>
        public static readonly AccountTotalLimitsDataInfo BaseFree = new AccountTotalLimitsDataInfo()
        {
            EventRequestsPerDay = 2000,
            EventsMaxDays = 30,

            UnitTestsRequestsPerDay = 50 * 6 * 24, // 50 проверок раз в 10 минут
            UnitTestsMaxDays = 30,

            LogSizePerDay = 200 * 1024 * 1024,
            LogMaxDays = 7,

            MetricRequestsPerDay = 50 * 6 * 24, // 50 метрик раз в 10 минут
            MetricsMaxDays = 30,

            ComponentsMax = 100,
            ComponentTypesMax = 100,
            UnitTestTypesMax = 100,
            HttpChecksMaxNoBanner = 10,
            UnitTestsMax = 10,
            MetricsMax = 10,
            
            StorageSizeMax = 1 * 1024 * 1024 * 1024,

            SmsPerDay = 1
        };

        /// <summary>
        /// Безлимитный аккаунт
        /// </summary>
        public static readonly AccountTotalLimitsDataInfo BaseUnlimited = new AccountTotalLimitsDataInfo()
        {
            EventRequestsPerDay = int.MaxValue,
            EventsMaxDays = 21,

            UnitTestsRequestsPerDay = int.MaxValue,
            UnitTestsMaxDays = 31,

            LogSizePerDay = Int64.MaxValue,
            LogMaxDays = 31,

            MetricRequestsPerDay = int.MaxValue,
            MetricsMaxDays = 31,

            ComponentsMax = int.MaxValue,
            ComponentTypesMax = int.MaxValue,
            UnitTestTypesMax = int.MaxValue,
            HttpChecksMaxNoBanner = int.MaxValue,
            UnitTestsMax = int.MaxValue,
            MetricsMax = int.MaxValue,

            StorageSizeMax = Int64.MaxValue,

            SmsPerDay = 100
        };

        /// <summary>
        /// Бонус за друга
        /// </summary>
        public static readonly TariffLimit Friend = new TariffLimit()
        {
            Id = new Guid("B709428E-9EED-4BCA-8DB0-E1E9CB2E30EC"),
            Name = "Приведи друга",
            Description = "+5 компонентов, если ваш друг зарегистрирует платный аккаунт",
            Type = TariffLimitType.Soft,
            Source = TariffLimitSource.Friend,

            ComponentsMax = 5,
            ComponentTypesMax = 5
        };

        /// <summary>
        /// Тестовый бонус - для юнит-тестов
        /// </summary>
        public static readonly TariffLimit TestBonus = new TariffLimit()
        {
            Id = new Guid("3170047D-1270-416B-BCC3-037B1FDCD6CF"),
            Name = "Тестовый бонус - для юнит-тестов",
            Description = "Тестовый бонус - для юнит-тестов",
            Type = TariffLimitType.Hard,
            Source = TariffLimitSource.TestBonus,

            EventsRequestsPerDay = 1,
            EventsMaxDays = 1,

            LogMaxDays = 2,
            LogSizePerDay = 200,

            ComponentsMax = 1,
            ComponentTypesMax = 1,

            UnitTestTypesMax = 1,
            HttpUnitTestsMaxNoBanner = 1,

            UnitTestsMax = 1,
            UnitTestsRequestsPerDay = 1,

            MetricsMax = 1,
            MetricsRequestsPerDay = 1,
            MetricsMaxDays = 1,

            SmsPerDay = 1
        };
    }
}
