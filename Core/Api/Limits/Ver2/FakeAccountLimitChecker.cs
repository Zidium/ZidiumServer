using System;
using Zidium.Api;

namespace Zidium.Core.Api.Limits.Ver2
{
    public class FakeAccountLimitChecker : IAccountLimitChecker
    {
        public Guid AccountId { get; set; }

        public void SetAccountOverlimitSignal()
        {
        }

        public void SaveData()
        {
        }

        public void RefreshTariffLimit()
        {
        }

        public Zidium.Api.IComponentControl ComponentControl
        {
            get { return new FakeComponentControl("fake");}
        }

        public ILimitCounter MaxComponents
        {
            get { return new FakeLimitCounter();}
        }

        public ILimitCounter MaxComponentTypes
        {
            get { return new FakeLimitCounter(); }
        }

        public ILimitCounter MaxUnitTestTypes
        {
            get { return new FakeLimitCounter(); }
        }

        public ILimitCounter MaxHttpChecksNoBanner
        {
            get { return new FakeLimitCounter(); }
        }

        public ILimitCounter MaxUnitTests
        {
            get { return new FakeLimitCounter(); }
        }

        public ILimitCounter MaxMetrics
        {
            get { return new FakeLimitCounter(); }
        }

        public ILimitCounter UnitTestResultsPerDay
        {
            get { return new FakeLimitCounter(); }
        }

        public ILimitCounter UnitTestsSizePerDay
        {
            get { return new FakeLimitCounter(); }
        }

        public ILimitCounter LogSizePerDay
        {
            get { return new FakeLimitCounter(); }
        }

        public ILimitCounter EventsRequestsPerDay
        {
            get { return new FakeLimitCounter(); }
        }

        public ILimitCounter EventsSizePerDay
        {
            get { return new FakeLimitCounter(); }
        }

        public ILimitCounter MetricsRequestsPerDay
        {
            get { return new FakeLimitCounter(); }
        }

        public ILimitCounter MetricsSizePerDay
        {
            get { return new FakeLimitCounter(); }
        }

        public ILimitCounter StorageSize
        {
            get { return new FakeLimitCounter(); }
        }

        public void AddUnitTestResult(Guid unitTestId, long size)
        {
        }
    }
}
