/*
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Entity;
using Zidium.Api;
using Zidium.Core.AccountsDb;

namespace Zidium.Core.Api.Limits.Ver2
{
    internal class RealAccountLimitChecker : IAccountLimitChecker
    {
        private class UnitTestStatData
        {
            //public Guid RowId;
            public Guid UnitTestId;
            public long Count;
        }

        private class LimitDataRow
        {
            public LimitDataRow ParentRow;

            public Guid Id;
            public Guid AccountId;
            public LimitDataType Type;

            public DateTime StartDate;
            public DateTime EndDate;

            public ILimitCounter MaxComponents;
            public ILimitCounter MaxComponentTypes;
            public ILimitCounter MaxUnitTestTypes;
            public ILimitCounter MaxHttpChecksNoBanner;
            public ILimitCounter MaxUnitTests;
            public ILimitCounter MaxMetrics;
            public ILimitCounter UnitTestResultsPerDay;
            public ILimitCounter UnitTestsSizePerDay;
            public ILimitCounter LogSizePerDay;
            public ILimitCounter EventsRequestsPerDay;
            public ILimitCounter EventsSizePerDay;
            public ILimitCounter MetricsRequestsPerDay;
            public ILimitCounter MetricsSizePerDay;
            public ILimitCounter StorageSize;

            private ConcurrentDictionary<Guid, UnitTestStatData> _unitTestResults = new ConcurrentDictionary<Guid, UnitTestStatData>();

            public void AddUnitTestResult(Guid unitTestId)
            {
                // обновляем статистику данной проверки
                _unitTestResults.AddOrUpdate(
                    unitTestId, 
                    id => new UnitTestStatData()
                    {
                        Count = 1,
                        UnitTestId = unitTestId
                    },
                    (id, data) =>
                    {
                        data.Count++;
                        return data;
                    });

                // обновляем статистику всех проверок
            }

            public bool IsActual(DateTime date)
            {
                return EndDate > date;
            }

            public bool IsNew;

            public void Save(AccountDbContext accountDbContext)
            {
                var limitData = new LimitData()
                {
                    Id = Id,
                    AccountId = AccountId,
                    BeginDate = StartDate,
                    EndDate = EndDate,
                    EventsRequests = (int)EventsRequestsPerDay.Value,
                    EventsSize = EventsSizePerDay.Value,
                    LogSize = LogSizePerDay.Value,
                    MetricsRequests = (int)MetricsRequestsPerDay.Value,
                    MetricsSize = MetricsSizePerDay.Value,
                    UnitTestsRequests = (int)UnitTestResultsPerDay.Value,
                    UnitTestsSize = UnitTestsSizePerDay.Value,
                    Type = Type
                };
                if (IsNew)
                {
                    // вставка новой строки
                    accountDbContext.LimitDatas.Add(limitData);
                }
                else
                {
                    //апдейт существующей строки
                    accountDbContext.LimitDatas.Attach(limitData);
                    var entity = accountDbContext.Entry(limitData).State == EntityState.Modified;
                }

                // сохраняем родителя в той же транзакции, чтобы данные в БД были согласованы
                if (ParentRow != null)
                {
                    ParentRow.Save(accountDbContext);
                }
            }
        }

        private LimitDataRow CurrentHourData = new LimitDataRow();
        private LimitDataRow CurrentDayData = new LimitDataRow();

        private List<LimitDataRow> ArchiveData = new List<LimitDataRow>(); 

        public Guid AccountId { get; private set; }

        public void SetAccountOverlimitSignal()
        {
            throw new NotImplementedException();
        }

        public void SaveData()
        {
            throw new NotImplementedException();
        }

        public void RefreshTariffLimit()
        {
            throw new NotImplementedException();
        }

        public IComponentControl ComponentControl { get; private set; }

        public ILimitCounter MaxComponents { get; private set; }
        public ILimitCounter MaxComponentTypes { get; private set; }
        public ILimitCounter MaxUnitTestTypes { get; private set; }
        public ILimitCounter MaxHttpChecksNoBanner { get; private set; }
        public ILimitCounter MaxUnitTests { get; private set; }
        public ILimitCounter MaxMetrics { get; private set; }
        public ILimitCounter UnitTestResultsPerDay { get; private set; }
        public ILimitCounter UnitTestsSizePerDay { get; private set; }
        public ILimitCounter LogSizePerDay { get; private set; }
        public ILimitCounter EventsRequestsPerDay { get; private set; }
        public ILimitCounter EventsSizePerDay { get; private set; }
        public ILimitCounter MetricsRequestsPerDay { get; private set; }
        public ILimitCounter MetricsSizePerDay { get; private set; }
        public ILimitCounter StorageSize { get; private set; }

        public void AddUnitTestResult(Guid unitTestId, long size)
        {
            throw new NotImplementedException();
        }
    }
}
*/