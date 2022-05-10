using System;
using System.Collections.Generic;
using System.Linq;
using Zidium.Common;
using Zidium.Core.Api;
using Zidium.Core.Common;
using Zidium.Storage;

namespace Zidium.Core.Limits
{
    // TODO Rename to stats
    public class AccountLimitsChecker
    {
        #region Other

        public AccountLimitsChecker(ITimeService timeService)
        {
            _timeService = timeService;
            DataCurrent = GetNewCurrentDataRow();
        }

        private readonly ITimeService _timeService;

        #endregion

        #region PerDay

        public void AddUnitTestResultsPerDay(IStorage context, Guid unitTestId)
        {
            CheckForNewCurrentDataRow(context);
            var unitTestData = DataCurrent.GetUnitTestData(unitTestId);
            lock (this)
            {
                unitTestData.ResultsCount++;
                DataCurrent.UnitTestsRequests++;
            }
        }

        public void AddUnitTestsSizePerDay(IStorage context, Int64 size)
        {
            CheckForNewCurrentDataRow(context);
            lock (this)
            {
                DataCurrent.UnitTestsSize += size;
            }
        }

        public void AddLogSizePerDay(IStorage context, Int64 size)
        {
            CheckForNewCurrentDataRow(context);
            lock (this)
            {
                DataCurrent.LogSize += size;
            }
        }

        public void AddEventsRequestsPerDay(IStorage context)
        {
            CheckForNewCurrentDataRow(context);
            lock (this)
            {
                DataCurrent.EventsRequests++;
            }
        }

        public void AddEventsSizePerDay(IStorage context, Int64 size)
        {
            CheckForNewCurrentDataRow(context);
            lock (this)
            {
                DataCurrent.EventsSize += size;
            }
        }

        public void AddMetricsRequestsPerDay(IStorage context)
        {
            CheckForNewCurrentDataRow(context);
            lock (this)
            {
                DataCurrent.MetricsRequests++;
            }
        }

        public void AddMetricsSizePerDay(IStorage context, Int64 size)
        {
            CheckForNewCurrentDataRow(context);
            lock (this)
            {
                DataCurrent.MetricsSize += size;
            }
        }

        public void AddSmsPerDay(IStorage context)
        {
            CheckForNewCurrentDataRow(context);
            lock (this)
            {
                DataCurrent.SmsCount++;
            }
        }

        #endregion

        #region Data collection

        protected AccountUsedLimitsPerDayDataInfo OverallArchive;

        public const int LimitDataTimeStep = 5;

        private List<LimitDataArchiveItem> _dataArchive;

        internal List<LimitDataArchiveItem> GetDataArchive(IStorage storage)
        {
            if (_dataArchive == null)
            {
                // Загрузим историю за сегодня и вчера из базы
                var date = _timeService.Now().Date.AddDays(-1);
                _dataArchive = storage.LimitData.Find(date, LimitDataType.Per5Minutes)
                    .Select(t => new LimitDataArchiveItem()
                    {
                        Id = t.Id,
                        BeginDate = t.BeginDate,
                        EndDate = t.EndDate,
                        EventsRequests = t.EventsRequests,
                        EventsSize = t.EventsSize,
                        LogSize = t.LogSize,
                        MetricsRequests = t.MetricsRequests,
                        MetricsSize = t.MetricsSize,
                        UnitTestsRequests = t.UnitTestsRequests,
                        UnitTestsSize = t.UnitTestsSize,
                        SmsCount = t.SmsCount
                    })
                    .ToList();
            }
            return _dataArchive;
        }

        internal LimitDataArchiveItem DataTotal = new LimitDataArchiveItem();

        internal LimitDataArchiveItem DataCurrent;

        protected Object DataArchiveLockObject = new object();

        protected void CheckForNewCurrentDataRow(IStorage context)
        {
            var needRecalc = false;
            lock (DataArchiveLockObject)
            {
                // Загрузим архив, если его ещё нет
                if (_dataArchive == null)
                    RecalcDataTotal(context);

                // Проверим, не пора ли переместить текущую запись в архив и создать новую
                if (_timeService.Now() >= DataCurrent.EndDate)
                {
                    var dataArchive = GetDataArchive(context);
                    var newArchiveItem = new LimitDataArchiveItem()
                    {
                        Id = DataCurrent.Id,
                        BeginDate = DataCurrent.BeginDate,
                        EndDate = DataCurrent.EndDate,
                        EventsRequests = DataCurrent.EventsRequests,
                        EventsSize = DataCurrent.EventsSize,
                        LogSize = DataCurrent.LogSize,
                        MetricsRequests = DataCurrent.MetricsRequests,
                        MetricsSize = DataCurrent.MetricsSize,
                        UnitTestsRequests = DataCurrent.UnitTestsRequests,
                        UnitTestsSize = DataCurrent.UnitTestsSize,
                        UnitTestData = DataCurrent.UnitTestData,
                        SmsCount = DataCurrent.SmsCount
                    };
                    dataArchive.Add(newArchiveItem);
                    DataCurrent = GetNewCurrentDataRow();
                    needRecalc = true;
                }
            }

            if (needRecalc)
                RecalcDataTotal(context);
        }

        internal LimitDataArchiveItem GetNewCurrentDataRow()
        {
            var now = _timeService.Now();

            // Выравниваем дату по 5 минутам
            var beginDate = new DateTime(now.Year, now.Month, now.Day, now.Hour, (now.Minute / LimitDataTimeStep) * LimitDataTimeStep, 0);

            return new LimitDataArchiveItem()
            {
                Id = Guid.Empty,
                BeginDate = beginDate,
                EndDate = beginDate.AddMinutes(LimitDataTimeStep)
            };
        }

        protected void RecalcDataTotal(IStorage storage)
        {
            LimitDataArchiveItem[] rows;
            lock (DataArchiveLockObject)
            {
                var dataArchive = GetDataArchive(storage);
                rows = dataArchive.Where(t => t.BeginDate >= _timeService.Now().Date).ToArray();
            }

            // Рассчитаем суммарные значения по истории за сегодня
            var newDataTotal = new LimitDataArchiveItem();
            newDataTotal.EventsRequests = rows.Sum(t => t.EventsRequests);
            newDataTotal.EventsSize = rows.Sum(t => t.EventsSize);
            newDataTotal.UnitTestsRequests = rows.Sum(t => t.UnitTestsRequests);
            newDataTotal.UnitTestsSize = rows.Sum(t => t.UnitTestsSize);
            newDataTotal.MetricsRequests = rows.Sum(t => t.MetricsRequests);
            newDataTotal.MetricsSize = rows.Sum(t => t.MetricsSize);
            newDataTotal.LogSize = rows.Sum(t => t.LogSize);
            newDataTotal.SmsCount = rows.Sum(t => t.SmsCount);

            // Рассчитаем суммарные данные по проверкам
            // Сначала суммируем все несохранённые данные из архива в памяти
            foreach (var row in rows.Where(t => t.Id == Guid.Empty).ToArray())
                foreach (var unitTestData in row.UnitTestData)
                {
                    var totalData = newDataTotal.GetUnitTestData(unitTestData.Key);
                    totalData.ResultsCount += unitTestData.Value.ResultsCount;
                }

            // И добавляем к ним сохранённые данные из архива в базе
            var archiveFromDb = storage.LimitDataForUnitTest.GetGroupedByUnitTest(_timeService.Now().Date, LimitDataType.Per5Minutes);
            foreach (var row in archiveFromDb)
            {
                var totalData = newDataTotal.GetUnitTestData(row.Item1);
                totalData.ResultsCount += row.Item2;
            }

            DataTotal = newDataTotal;

            // Рассчитаем суммарные данные за всю историю
            OverallArchive = GetUsedLimitsTotalData(storage);
        }

        public int SaveData(IStorage storage)
        {
            CheckForNewCurrentDataRow(storage);

            List<LimitDataArchiveItem> localDataArchive;
            lock (DataArchiveLockObject)
            {
                var dataArchive = GetDataArchive(storage);
                localDataArchive = new List<LimitDataArchiveItem>(dataArchive);
            }

            // Сохраним все несохранённые записи
            var rows = localDataArchive.Where(t => t.Id == Guid.Empty).ToArray();
            foreach (var row in rows)
            {
                var limitData = new LimitDataForAdd()
                {
                    Id = Ulid.NewUlid(),
                    BeginDate = row.BeginDate,
                    EndDate = row.EndDate,
                    Type = LimitDataType.Per5Minutes,
                    EventsRequests = row.EventsRequests,
                    EventsSize = row.EventsSize,
                    LogSize = row.LogSize,
                    MetricsRequests = row.MetricsRequests,
                    MetricsSize = row.MetricsSize,
                    UnitTestsRequests = row.UnitTestsRequests,
                    UnitTestsSize = row.UnitTestsSize
                };

                var limitUnitTestData = row.UnitTestData.Select(t => new LimitDataForUnitTestForAdd()
                {
                    Id = Ulid.NewUlid(),
                    LimitDataId = limitData.Id,
                    UnitTestId = t.Key,
                    ResultsCount = t.Value.ResultsCount
                }).ToArray();

                storage.LimitData.Add(limitData);
                storage.LimitDataForUnitTest.Add(limitUnitTestData);

                row.Id = limitData.Id;
                row.UnitTestData = null;
            }

            // Проверим, есть ли запись за вчера с данными за целый день
            var yesterday = _timeService.Now().Date.AddDays(-1);
            var totalForYesterday = storage.LimitData.GetOneOrNullByDateAndType(yesterday, LimitDataType.Per1Day);
            if (totalForYesterday == null)
            {
                // Если записи за вчера нет, создадим её
                var totalForYesterdayForAdd = new LimitDataForAdd()
                {
                    Id = Ulid.NewUlid(),
                    BeginDate = yesterday,
                    EndDate = yesterday.AddDays(1),
                    Type = LimitDataType.Per1Day
                };

                // Заполним данными из архива за вчера
                var yesterdayArchive = localDataArchive.Where(t => t.BeginDate >= yesterday && t.BeginDate < yesterday.AddDays(1)).ToList();

                totalForYesterdayForAdd.EventsRequests = yesterdayArchive.Sum(t => t.EventsRequests);
                totalForYesterdayForAdd.EventsSize = yesterdayArchive.Sum(t => t.EventsSize);
                totalForYesterdayForAdd.UnitTestsRequests = yesterdayArchive.Sum(t => t.UnitTestsRequests);
                totalForYesterdayForAdd.UnitTestsSize = yesterdayArchive.Sum(t => t.UnitTestsSize);
                totalForYesterdayForAdd.MetricsRequests = yesterdayArchive.Sum(t => t.MetricsRequests);
                totalForYesterdayForAdd.MetricsSize = yesterdayArchive.Sum(t => t.MetricsSize);
                totalForYesterdayForAdd.LogSize = yesterdayArchive.Sum(t => t.LogSize);

                storage.LimitData.Add(totalForYesterdayForAdd);
            }

            // Удалим из архива в памяти те, которые старше 48 часов (сегодня + вчера)
            lock (DataArchiveLockObject)
            {
                var dataArchive = GetDataArchive(storage);
                dataArchive.RemoveAll(t => t.BeginDate < _timeService.Now().AddHours(-48));
            }

            // Удалим из базы те, которые старше старше 48 часов (сегодня + вчера)
            storage.LimitData.RemoveOld(_timeService.Now().AddHours(-48), LimitDataType.Per5Minutes);

            return rows.Length;
        }

        #endregion

        #region TariffLimit

        public AccountUsedLimitsTodayDataInfo GetUsedTodayTariffLimit(IStorage storage)
        {
            CheckForNewCurrentDataRow(storage);

            var result = new AccountUsedLimitsTodayDataInfo()
            {
                EventRequests = DataTotal.EventsRequests + DataCurrent.EventsRequests,
                EventsSize = DataTotal.EventsSize + DataCurrent.EventsSize,
                UnitTestsRequests = DataTotal.UnitTestsRequests + DataCurrent.UnitTestsRequests,
                UnitTestsSize = DataTotal.UnitTestsSize + DataCurrent.UnitTestsSize,
                LogSize = DataTotal.LogSize + DataCurrent.LogSize,
                MetricRequests = DataTotal.MetricsRequests + DataCurrent.MetricsRequests,
                MetricsSize = DataTotal.MetricsSize + DataCurrent.MetricsSize,
                SmsCount = DataTotal.SmsCount + DataCurrent.SmsCount
            };

            var tempData = new LimitDataArchiveItem();
            foreach (var data in DataTotal.UnitTestData.ToList())
            {
                tempData.GetUnitTestData(data.Key).ResultsCount += data.Value.ResultsCount;
            }
            foreach (var data in DataCurrent.UnitTestData.ToList())
            {
                tempData.GetUnitTestData(data.Key).ResultsCount += data.Value.ResultsCount;
            }

            result.UnitTestsResults = tempData.UnitTestData.Select(t => new AccountUsedLimitsUnitTestDataInfo()
            {
                UnitTestId = t.Key,
                ApiChecksResults = t.Value.ResultsCount
            }).ToArray();

            return result;
        }

        public AccountUsedLimitsOverallDataInfo GetUsedOverallTariffLimit(IStorage context, int archiveDays)
        {
            CheckForNewCurrentDataRow(context);

            var history = GetPerDayHistory(context);
            var total = GetUsedLimitsTotalData(context, history);
            var today = _timeService.Now().Date;
            var archive = history.Where(t => t.BeginDate >= today.AddDays(-archiveDays)).ToList();

            var result = new AccountUsedLimitsOverallDataInfo()
            {
                Total = new AccountUsedLimitsPerDayDataInfo()
                {
                    LogSize = total.LogSize + DataTotal.LogSize + DataCurrent.LogSize,
                    EventsSize = total.EventsSize + DataTotal.EventsSize + DataCurrent.EventsSize,
                    UnitTestsSize = total.UnitTestsSize + DataTotal.UnitTestsSize + DataCurrent.UnitTestsSize,
                    MetricsSize = total.MetricsSize + DataTotal.MetricsSize + DataCurrent.MetricsSize,
                    EventsRequests = total.EventsRequests + DataTotal.EventsRequests + DataCurrent.EventsRequests,
                    UnitTestsRequests = total.UnitTestsRequests + DataTotal.UnitTestsRequests + DataCurrent.UnitTestsRequests,
                    MetricsRequests = total.MetricsRequests + DataTotal.MetricsRequests + DataCurrent.MetricsRequests
                },
                Archive = archive.Select(t => new AccountUsedLimitsArchiveDataInfo()
                {
                    Date = t.BeginDate,
                    Info = new AccountUsedLimitsPerDayDataInfo()
                    {
                        LogSize = t.LogSize,
                        EventsSize = t.EventsSize,
                        UnitTestsSize = t.UnitTestsSize,
                        MetricsSize = t.MetricsSize,
                        EventsRequests = t.EventsRequests,
                        UnitTestsRequests = t.UnitTestsRequests,
                        MetricsRequests = t.MetricsRequests
                    }
                }).ToArray()
            };

            return result;
        }

        protected LimitDataForRead[] GetPerDayHistory(IStorage storage)
        {
            // Получим всю историю по дням
            var history = storage.LimitData.GetByType(LimitDataType.Per1Day);
            return history;
        }

        protected AccountUsedLimitsPerDayDataInfo GetUsedLimitsTotalData(IStorage context, LimitDataForRead[] history = null)
        {
            // Для каждого лимита посчитаем сумму за время хранения архива из тарифа
            var today = _timeService.Now().Date;

            if (history == null)
                history = GetPerDayHistory(context);

            var settingService = new LogicSettingsService();

            var logSize = history.Where(t => t.BeginDate >= today.AddDays(-settingService.LogMaxDays())).Sum(t => t.LogSize);

            var eventsSize = history.Where(t => t.BeginDate >= today.AddDays(-settingService.EventsMaxDays())).Sum(t => t.EventsSize);
            var eventsRequests = history.Where(t => t.BeginDate >= today.AddDays(-settingService.EventsMaxDays())).Sum(t => t.EventsRequests);

            var unitTestsSize = history.Where(t => t.BeginDate >= today.AddDays(-settingService.UnitTestsMaxDays())).Sum(t => t.UnitTestsSize);
            var unitTestsRequests = history.Where(t => t.BeginDate >= today.AddDays(-settingService.UnitTestsMaxDays())).Sum(t => t.UnitTestsRequests);

            var metricsSize = history.Where(t => t.BeginDate >= today.AddDays(-settingService.MetricsMaxDays())).Sum(t => t.MetricsSize);
            var metricsRequests = history.Where(t => t.BeginDate >= today.AddDays(-settingService.MetricsMaxDays())).Sum(t => t.MetricsRequests);

            return new AccountUsedLimitsPerDayDataInfo()
            {
                LogSize = logSize,
                EventsSize = eventsSize,
                UnitTestsSize = unitTestsSize,
                MetricsSize = metricsSize,
                EventsRequests = eventsRequests,
                UnitTestsRequests = unitTestsRequests,
                MetricsRequests = metricsRequests
            };
        }

        #endregion

    }
}
