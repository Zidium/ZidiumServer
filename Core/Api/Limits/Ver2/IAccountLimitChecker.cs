using System;
using Zidium.Api;

namespace Zidium.Core.Api.Limits.Ver2
{
    public interface IAccountLimitChecker
    {
        Guid AccountId { get; }

        void SetAccountOverlimitSignal();

        void SaveData();

        void RefreshTariffLimit();

        IComponentControl ComponentControl { get; }


        #region лимиты на количество

        ILimitCounter MaxComponents { get; }

        ILimitCounter MaxComponentTypes { get; }

        ILimitCounter MaxUnitTestTypes { get; }

        ILimitCounter MaxHttpChecksNoBanner { get; }

        ILimitCounter MaxUnitTests { get; }

        ILimitCounter MaxMetrics { get; }

        #endregion


        #region лимиты на количество в день

        ILimitCounter UnitTestResultsPerDay { get; }

        ILimitCounter UnitTestsSizePerDay { get; }

        ILimitCounter LogSizePerDay { get; }

        ILimitCounter EventsRequestsPerDay { get; }
        
        ILimitCounter EventsSizePerDay { get; }

        ILimitCounter MetricsRequestsPerDay { get; }

        ILimitCounter MetricsSizePerDay { get; }

        #endregion


        ILimitCounter StorageSize { get; }

        void AddUnitTestResult(Guid unitTestId, long size);
    }
}
