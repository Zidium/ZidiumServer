using System;
using System.Collections.Generic;
using System.Linq;
using Zidium.Core.AccountsDb;
using Zidium.Core.Api;
using Zidium.Core.ConfigDb;

namespace Zidium.Core.Limits
{
    public class AccountLimitsChecker
    {
        #region Other

        public AccountLimitsChecker(Guid accountId, DateTime? nowOverride = null)
        {
            AccountId = accountId;
            NowOverride = nowOverride;
            DataCurrent = GetNewCurrentDataRow();
        }

        public Guid AccountId;

        // Текущее время - для юнит-тестов
        public DateTime? NowOverride;

        protected void SetAccountOverlimitSignal()
        {
            ConfigDbServicesHelper.GetAccountService().Update(new UpdateAccountRequestData()
            {
                Id = AccountId,
                LastOverLimitDate = Now
            });
        }

        #endregion

        #region Components

        public LimitCheckResult CheckMaxComponents(AccountDbContext context)
        {
            var count = GetComponentsCount(context);

            var softLimit = GetSoftTariffLimit(context);
            if (!(count < softLimit.ComponentsMax))
                SetAccountOverlimitSignal();

            var hardLimit = GetHardTariffLimit(context);
            var result = new LimitCheckResult()
            {
                Success = count < hardLimit.ComponentsMax
            };
            if (!result.Success)
                result.Message = "Достигнут лимит на количество компонентов (максимум " + hardLimit.ComponentsMax + ")";
            return result;
        }

        public void RefreshComponentsCount()
        {
            _componentsCount = null;
        }

        private int? _componentsCount;

        protected int GetComponentsCount(AccountDbContext context)
        {
            var count = _componentsCount;
            if (!count.HasValue)
            {
                var componentRepository = context.GetComponentRepository();
                count = componentRepository.QueryAll().Count(t => t.ComponentTypeId != SystemComponentTypes.Root.Id);
                _componentsCount = count;
            }
            return count.Value;
        }

        #endregion

        #region ComponentTypes

        public LimitCheckResult CheckMaxComponentTypes(AccountDbContext context)
        {
            var count = GetComponentTypesCount(context);

            var softLimit = GetSoftTariffLimit(context);
            if (!(count < softLimit.ComponentTypesMax))
                SetAccountOverlimitSignal();

            var hardLimit = GetHardTariffLimit(context);
            var result = new LimitCheckResult()
            {
                Success = count < hardLimit.ComponentTypesMax
            };
            if (!result.Success)
                result.Message = "Достигнут лимит на количество типов компонентов (максимум " + hardLimit.ComponentTypesMax + ")";
            return result;
        }

        public void RefreshComponentTypesCount()
        {
            _componentTypesCount = null;
        }

        private int? _componentTypesCount;

        protected int GetComponentTypesCount(AccountDbContext context)
        {
            var count = _componentTypesCount;
            if (!count.HasValue)
            {
                var componentTypeRepository = context.GetComponentTypeRepository();
                count = componentTypeRepository.QueryAll().Count(t => !t.IsSystem);
                _componentTypesCount = count;
            }
            return count.Value;
        }

        #endregion

        #region UnitTestTypes

        public LimitCheckResult CheckMaxUnitTestTypes(AccountDbContext context)
        {
            var count = GetUnitTestTypesCount(context);

            var softLimit = GetSoftTariffLimit(context);
            if (!(count < softLimit.UnitTestTypesMax))
                SetAccountOverlimitSignal();

            var hardLimit = GetHardTariffLimit(context);
            var result = new LimitCheckResult()
            {
                Success = count < hardLimit.UnitTestTypesMax
            };
            if (!result.Success)
                result.Message = "Достигнут лимит на количество типов проверок (максимум " + hardLimit.ComponentTypesMax + ")";
            return result;
        }

        public void RefreshUnitTestTypesCount()
        {
            _unitTestTypesCount = null;
        }

        private int? _unitTestTypesCount;

        protected int GetUnitTestTypesCount(AccountDbContext context)
        {
            var count = _unitTestTypesCount;
            if (!count.HasValue)
            {
                var unitTestTypeRepository = context.GetUnitTestTypeRepository();
                count = unitTestTypeRepository.QueryAll().Count(t => !t.IsSystem);
                _unitTestTypesCount = count;
            }
            return count.Value;
        }

        #endregion

        #region UnitTests

        public LimitCheckResult CheckMaxHttpChecksNoBanner(AccountDbContext context)
        {
            var count = GetHttpChecksNoBannerCount(context);

            var softLimit = GetSoftTariffLimit(context);
            if (!(count < softLimit.HttpUnitTestsMaxNoBanner))
                SetAccountOverlimitSignal();

            var hardLimit = GetHardTariffLimit(context);
            var result = new LimitCheckResult()
            {
                Success = count <= hardLimit.HttpUnitTestsMaxNoBanner
            };
            if (!result.Success)
                result.Message = "Достигнут лимит на количество проверок http без баннера (максимум " + hardLimit.HttpUnitTestsMaxNoBanner + ")";
            return result;
        }

        public void RefreshHttpChecksNoBannerCount()
        {
            _httpChecksNoBannerCount = null;
        }

        private int? _httpChecksNoBannerCount;

        protected int GetHttpChecksNoBannerCount(AccountDbContext context)
        {
            var count = _httpChecksNoBannerCount;
            if (!count.HasValue)
            {
                var repository = context.GetUnitTestRepository();

                count = repository
                    .QueryAll()
                    .Count(t => t.TypeId == SystemUnitTestTypes.HttpUnitTestType.Id
                                && t.Enable
                                && t.HttpRequestUnitTest.LastBannerCheck.HasValue
                                && t.HttpRequestUnitTest.HasBanner == false);

                _httpChecksNoBannerCount = count;
            }
            return count.Value;
        }

        /// <summary>
        /// Количество юнит-тестов
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public LimitCheckResult CheckMaxUnitTestsCount(AccountDbContext context)
        {
            var count = GetUnitTestsCount(context);

            var softLimit = GetSoftTariffLimit(context);
            if (!(count < softLimit.UnitTestsMax))
                SetAccountOverlimitSignal();

            var hardLimit = GetHardTariffLimit(context);
            var result = new LimitCheckResult()
            {
                Success = count < hardLimit.UnitTestsMax
            };
            if (!result.Success)
                result.Message = "Достигнут лимит на количество проверок (максимум " + hardLimit.UnitTestsMax + ")";
            return result;
        }

        public void RefreshApiChecksCount()
        {
            _unitTestsCount = null;
        }

        private int? _unitTestsCount;

        protected int GetUnitTestsCount(AccountDbContext context)
        {
            var count = _unitTestsCount;
            if (!count.HasValue)
            {
                var repository = context.GetUnitTestRepository();
                count = repository.QueryAll().Count();
                _unitTestsCount = count;
            }
            return count.Value;
        }

        #endregion

        #region Metrics

        public LimitCheckResult CheckMaxMetrics(AccountDbContext context)
        {
            var count = GetMetricsCount(context);

            var softLimit = GetSoftTariffLimit(context);
            if (!(count < softLimit.MetricsMax))
                SetAccountOverlimitSignal();

            var hardLimit = GetHardTariffLimit(context);
            var result = new LimitCheckResult()
            {
                Success = count < hardLimit.MetricsMax
            };
            if (!result.Success)
                result.Message = "Достигнут лимит на количество метрик (максимум " + hardLimit.MetricsMax + ")";
            return result;
        }

        public void RefreshMetricsCount()
        {
            _metricsCount = null;
        }

        private int? _metricsCount;

        protected int GetMetricsCount(AccountDbContext context)
        {
            var count = _metricsCount;
            if (!count.HasValue)
            {
                var metricTypeRepository = context.GetMetricTypeRepository();
                count = metricTypeRepository.QueryAll().Count();
                _metricsCount = count;
            }
            return count.Value;
        }

        #endregion

        #region PerDay

        public void CheckUnitTestResultsPerDay(AccountDbContext context)
        {
            CheckForNewCurrentDataRow(context);

            var count = DataTotal.UnitTestsRequests + DataCurrent.UnitTestsRequests;

            var softLimit = GetSoftTariffLimit(context);
            if (count >= softLimit.UnitTestsRequestsPerDay)
                SetAccountOverlimitSignal();

            var hardLimit = GetHardTariffLimit(context);
            if (count >= hardLimit.UnitTestsRequestsPerDay)
                throw new OverLimitException("Достигнут лимит на количество результатов проверок в день (максимум " + hardLimit.UnitTestsRequestsPerDay + ")");
        }

        public void AddUnitTestResultsPerDay(AccountDbContext context, Guid unitTestId)
        {
            CheckForNewCurrentDataRow(context);
            var unitTestData = DataCurrent.GetUnitTestData(unitTestId);
            lock (this)
            {
                unitTestData.ResultsCount++;
                DataCurrent.UnitTestsRequests++;
            }
        }

        public void AddUnitTestsSizePerDay(AccountDbContext context, Int64 size)
        {
            CheckForNewCurrentDataRow(context);
            lock (this)
            {
                DataCurrent.UnitTestsSize += size;
            }
        }

        public void CheckLogSizePerDay(AccountDbContext context, Int64 size)
        {
            CheckForNewCurrentDataRow(context);

            lock (this)
            {
                var newSize = DataTotal.LogSize + DataCurrent.LogSize + size;

                var softLimit = GetSoftTariffLimit(context);
                if (newSize > softLimit.LogSizePerDay)
                    SetAccountOverlimitSignal();

                var hardLimit = GetHardTariffLimit(context);
                if (newSize > hardLimit.LogSizePerDay)
                    throw new OverLimitException("Достигнут лимит на размер лога в день (максимум " + hardLimit.LogSizePerDay + " байт)");
            }
        }

        public void AddLogSizePerDay(AccountDbContext context, Int64 size)
        {
            CheckForNewCurrentDataRow(context);
            lock (this)
            {
                DataCurrent.LogSize += size;
            }
        }

        public void CheckEventsRequestsPerDay(AccountDbContext context)
        {
            CheckForNewCurrentDataRow(context);

            lock (this)
            {
                var count = DataTotal.EventsRequests + DataCurrent.EventsRequests;

                var softLimit = GetSoftTariffLimit(context);
                if (count >= softLimit.EventsRequestsPerDay)
                    SetAccountOverlimitSignal();

                var hardLimit = GetHardTariffLimit(context);
                if (count >= hardLimit.EventsRequestsPerDay)
                    throw new OverLimitException("Достигнут лимит на количество вызовов api событий в день (максимум " + hardLimit.EventsRequestsPerDay + ")");
            }
        }

        public void AddEventsRequestsPerDay(AccountDbContext context)
        {
            CheckForNewCurrentDataRow(context);
            lock (this)
            {
                DataCurrent.EventsRequests++;
            }
        }

        public void AddEventsSizePerDay(AccountDbContext context, Int64 size)
        {
            CheckForNewCurrentDataRow(context);
            lock (this)
            {
                DataCurrent.EventsSize += size;
            }
        }

        public void CheckMetricsRequestsPerDay(AccountDbContext context)
        {
            CheckForNewCurrentDataRow(context);

            var count = DataTotal.MetricsRequests + DataCurrent.MetricsRequests;

            var softLimit = GetSoftTariffLimit(context);
            if (count >= softLimit.MetricsRequestsPerDay)
                SetAccountOverlimitSignal();

            var hardLimit = GetHardTariffLimit(context);
            if (count >= hardLimit.MetricsRequestsPerDay)
                throw new OverLimitException("Достигнут лимит на количество вызовов api метрик в день (максимум " + hardLimit.MetricsRequestsPerDay + ")");
        }

        public void AddMetricsRequestsPerDay(AccountDbContext context)
        {
            CheckForNewCurrentDataRow(context);
            lock (this)
            {
                DataCurrent.MetricsRequests++;
            }
        }

        public void AddMetricsSizePerDay(AccountDbContext context, Int64 size)
        {
            CheckForNewCurrentDataRow(context);
            lock (this)
            {
                DataCurrent.MetricsSize += size;
            }
        }

        public void CheckSmsPerDay(AccountDbContext context)
        {
            CheckForNewCurrentDataRow(context);

            lock (this)
            {
                var count = DataTotal.SmsCount + DataCurrent.SmsCount;

                // Для sms о превышении лимита не предупреждаем
                /*
                var softLimit = GetSoftTariffLimit(context);
                if (count >= softLimit.SmsPerDay)
                    SetAccountOverlimitSignal();
                */

                var hardLimit = GetHardTariffLimit(context);
                if (count >= hardLimit.SmsPerDay)
                    throw new OverLimitException("Достигнут лимит на количество SMS в день (максимум " + hardLimit.SmsPerDay + ")");
            }
        }

        public void AddSmsPerDay(AccountDbContext context)
        {
            CheckForNewCurrentDataRow(context);
            lock (this)
            {
                DataCurrent.SmsCount++;
            }
        }

        #endregion

        #region Storage

        protected AccountUsedLimitsPerDayDataInfo OverallArchive;

        public void CheckStorageSize(AccountDbContext context, Int64 size)
        {
            CheckForNewCurrentDataRow(context);

            var newSize = DataCurrent.StorageSize + DataTotal.StorageSize + OverallArchive.StorageSize + size;

            var softLimit = GetSoftTariffLimit(context);
            if (newSize > softLimit.StorageSizeMax)
                SetAccountOverlimitSignal();

            var hardLimit = GetHardTariffLimit(context);
            if (newSize > hardLimit.StorageSizeMax)
                throw new OverLimitException("Достигнут лимит на размер хранилища (максимум " + hardLimit.StorageSizeMax + " байт)");
        }

        #endregion

        #region Data collection

        public const int LimitDataTimeStep = 5;

        private List<LimitDataArchiveItem> _dataArchive;

        internal List<LimitDataArchiveItem> GetDataArchive(AccountDbContext context)
        {
            if (_dataArchive == null)
            {
                // Загрузим историю за сегодня и вчера из базы
                var repository = context.GetLimitDataRepository();
                var date = Now.Date.AddDays(-1);
                _dataArchive = repository
                    .QueryAll()
                    .Where(t => t.BeginDate >= date && t.Type == LimitDataType.Per5Minutes)
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

        protected void CheckForNewCurrentDataRow(AccountDbContext context)
        {
            var needRecalc = false;
            lock (DataArchiveLockObject)
            {
                // Загрузим архив, если его ещё нет
                if (_dataArchive == null)
                    RecalcDataTotal(context);

                // Проверим, не пора ли переместить текущую запись в архив и создать новую
                if (Now >= DataCurrent.EndDate)
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
            var now = Now;

            // Выравниваем дату по 5 минутам
            var beginDate = new DateTime(now.Year, now.Month, now.Day, now.Hour, (now.Minute / LimitDataTimeStep) * LimitDataTimeStep, 0);

            return new LimitDataArchiveItem()
            {
                Id = Guid.Empty,
                BeginDate = beginDate,
                EndDate = beginDate.AddMinutes(LimitDataTimeStep)
            };
        }

        protected void RecalcDataTotal(AccountDbContext context)
        {
            LimitDataArchiveItem[] rows;
            lock (DataArchiveLockObject)
            {
                var dataArchive = GetDataArchive(context);
                rows = dataArchive.Where(t => t.BeginDate >= Now.Date).ToArray();
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
            var repository = context.GetLimitDataForUnitTestRepository();
            var archiveFromDb = repository.GetGroupedByUnitTest(Now.Date);
            foreach (var row in archiveFromDb)
            {
                var totalData = newDataTotal.GetUnitTestData(row.Item1);
                totalData.ResultsCount += row.Item2;
            }

            DataTotal = newDataTotal;

            // Рассчитаем суммарные данные за всю историю
            OverallArchive = GetUsedLimitsTotalData(context);
        }

        public int SaveData(AccountDbContext context)
        {
            CheckForNewCurrentDataRow(context);

            List<LimitDataArchiveItem> localDataArchive;
            lock (DataArchiveLockObject)
            {
                var dataArchive = GetDataArchive(context);
                localDataArchive = new List<LimitDataArchiveItem>(dataArchive);
            }

            // Сохраним все несохранённые записи
            var repository = context.GetLimitDataRepository();
            var rows = localDataArchive.Where(t => t.Id == Guid.Empty).ToArray();
            foreach (var row in rows)
            {
                var limitData = new LimitData()
                {
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
                limitData.UnitTestData = row.UnitTestData.Select(t => new LimitDataForUnitTest()
                {
                    Id = Guid.NewGuid(),
                    LimitData = limitData,
                    UnitTestId = t.Key,
                    ResultsCount = t.Value.ResultsCount
                }).ToList();
                repository.Add(limitData);

                context.SaveChanges();

                row.Id = limitData.Id;
                row.UnitTestData = null;
            }

            // Проверим, есть ли запись за вчера с данными за целый день
            var yesterday = Now.Date.AddDays(-1);
            var totalForYesterday = repository.QueryAll().FirstOrDefault(t => t.Type == LimitDataType.Per1Day && t.BeginDate == yesterday);
            if (totalForYesterday == null)
            {
                // Если записи за вчера нет, создадим её
                totalForYesterday = new LimitData()
                {
                    BeginDate = yesterday,
                    EndDate = yesterday.AddDays(1),
                    Type = LimitDataType.Per1Day
                };

                // Заполним данными из архива за вчера
                var yesterdayArchive = localDataArchive.Where(t => t.BeginDate >= yesterday && t.BeginDate < yesterday.AddDays(1)).ToList();

                totalForYesterday.EventsRequests = yesterdayArchive.Sum(t => t.EventsRequests);
                totalForYesterday.EventsSize = yesterdayArchive.Sum(t => t.EventsSize);
                totalForYesterday.UnitTestsRequests = yesterdayArchive.Sum(t => t.UnitTestsRequests);
                totalForYesterday.UnitTestsSize = yesterdayArchive.Sum(t => t.UnitTestsSize);
                totalForYesterday.MetricsRequests = yesterdayArchive.Sum(t => t.MetricsRequests);
                totalForYesterday.MetricsSize = yesterdayArchive.Sum(t => t.MetricsSize);
                totalForYesterday.LogSize = yesterdayArchive.Sum(t => t.LogSize);

                repository.Add(totalForYesterday);
                context.SaveChanges();
            }

            // Удалим из архива в памяти те, которые старше 48 часов (сегодня + вчера)
            lock (DataArchiveLockObject)
            {
                var dataArchive = GetDataArchive(context);
                dataArchive.RemoveAll(t => t.BeginDate < Now.AddHours(-48));
            }

            // Удалим из базы те, которые старше старше 48 часов (сегодня + вчера)
            repository.RemoveOld(Now.AddHours(-48), LimitDataType.Per5Minutes);

            return rows.Length;
        }

        protected DateTime Now
        {
            get { return NowOverride ?? DateTime.Now; }
        }

        #endregion

        #region TariffLimit

        public void RefreshTariffLimit()
        {
            _hardTariffLimit = null;
            _softTariffLimit = null;
        }

        private TariffLimit _hardTariffLimit;

        public TariffLimit GetHardTariffLimit(AccountDbContext context)
        {
            var limit = _hardTariffLimit;
            if (limit == null)
            {
                var accountTariffRepository = context.GetAccountTariffRepository();
                limit = accountTariffRepository.GetHardTariffLimit();
                _hardTariffLimit = limit;
            }
            return limit;
        }

        private TariffLimit _softTariffLimit;

        public TariffLimit GetSoftTariffLimit(AccountDbContext context)
        {
            var limit = _softTariffLimit;
            if (limit == null)
            {
                var accountTariffRepository = context.GetAccountTariffRepository();
                limit = accountTariffRepository.GetSoftTariffLimit();
                _softTariffLimit = limit;
            }
            return limit;
        }

        public AccountUsedLimitsTodayDataInfo GetUsedTodayTariffLimit(AccountDbContext context)
        {
            CheckForNewCurrentDataRow(context);

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

        public AccountUsedLimitsInstantDataInfo GetUsedInstantTariffLimit(AccountDbContext context)
        {
            var result = new AccountUsedLimitsInstantDataInfo()
            {
                ComponentsCount = GetComponentsCount(context),
                ComponentTypesCount = GetComponentTypesCount(context),
                UnitTestTypesCount = GetUnitTestTypesCount(context),
                HttpUnitTestsNoBannerCount = GetHttpChecksNoBannerCount(context),
                UnitTestsCount = GetUnitTestsCount(context),
                MetricsCount = GetMetricsCount(context)
            };
            return result;
        }

        public AccountUsedLimitsOverallDataInfo GetUsedOverallTariffLimit(AccountDbContext context, int archiveDays)
        {
            CheckForNewCurrentDataRow(context);

            var history = GetPerDayHistory(context);
            var total = GetUsedLimitsTotalData(context, history);
            var today = Now.Date;
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

        protected List<LimitData> GetPerDayHistory(AccountDbContext context)
        {
            // Получим всю историю по дням
            var limitDataRepository = context.GetLimitDataRepository();
            var history = limitDataRepository.QueryAll().Where(t => t.Type == LimitDataType.Per1Day).ToList();
            return history;
        }

        protected AccountUsedLimitsPerDayDataInfo GetUsedLimitsTotalData(AccountDbContext context, List<LimitData> history = null)
        {
            // Для каждого лимита посчитаем сумму за время хранения архива из тарифа
            var today = Now.Date;

            if (history == null)
                history = GetPerDayHistory(context);

            var tariffLimit = GetHardTariffLimit(context);

            var logSize = history.Where(t => t.BeginDate >= today.AddDays(-tariffLimit.LogMaxDays)).Sum(t => t.LogSize);

            var eventsSize = history.Where(t => t.BeginDate >= today.AddDays(-tariffLimit.EventsMaxDays)).Sum(t => t.EventsSize);
            var eventsRequests = history.Where(t => t.BeginDate >= today.AddDays(-tariffLimit.EventsMaxDays)).Sum(t => t.EventsRequests);

            var unitTestsSize = history.Where(t => t.BeginDate >= today.AddDays(-tariffLimit.UnitTestsMaxDays)).Sum(t => t.UnitTestsSize);
            var unitTestsRequests = history.Where(t => t.BeginDate >= today.AddDays(-tariffLimit.UnitTestsMaxDays)).Sum(t => t.UnitTestsRequests);

            var metricsSize = history.Where(t => t.BeginDate >= today.AddDays(-tariffLimit.MetricsMaxDays)).Sum(t => t.MetricsSize);
            var metricsRequests = history.Where(t => t.BeginDate >= today.AddDays(-tariffLimit.MetricsMaxDays)).Sum(t => t.MetricsRequests);

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
