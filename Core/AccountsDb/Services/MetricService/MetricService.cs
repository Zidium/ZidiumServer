using System;
using System.Collections.Generic;
using System.Linq;
using Zidium.Api.Dto;
using Zidium.Core.AccountsDb;
using Zidium.Core.Api;
using Zidium.Core.Caching;
using Zidium.Core.Common;
using Zidium.Core.Common.Helpers;
using Zidium.Core.DispatcherLayer;
using Zidium.Core.Limits;

namespace Zidium.Core.AccountsDb
{
    public class MetricService : IMetricService
    {
        protected DispatcherContext Context { get; set; }

        public MetricService(DispatcherContext dispatcherContext)
        {
            if (dispatcherContext == null)
            {
                throw new ArgumentNullException("dispatcherContext");
            }
            Context = dispatcherContext;
        }


        protected IMetricCacheReadObject GetActualMetricInternal(IMetricCacheReadObject metric, DateTime processDate)
        {
            if (metric == null)
            {
                throw new ArgumentNullException("metric");
            }

            // обновим галочку "включено"
            if (metric.Enable == false
                && metric.DisableToDate.HasValue
                && metric.DisableToDate < processDate)
            {
                using (var metricWrite = AllCaches.Metrics.Write(metric))
                {
                    metricWrite.Enable = true;
                    metricWrite.BeginSave();
                    metric = metricWrite;
                }
            }

            var statusService = Context.BulbService;
            var data = statusService.GetRaw(metric.AccountId, metric.StatusDataId);

            // если надо выключить
            if (metric.CanProcess == false && data.Status != MonitoringStatus.Disabled)
            {
                UpdateEnableOrDisableStatusData(metric);
                return metric;
            }

            // если надо включить
            if (metric.CanProcess && data.Status == MonitoringStatus.Disabled)
            {
                UpdateEnableOrDisableStatusData(metric);
                return metric;
            }

            // если значение актуально, вернем его сразу
            if (data.Actual(processDate))
            {
                return metric;
            }

            // значение метрики не актуальное
            // сохраняем пробел
            using (var metricWrite = AllCaches.Metrics.Write(metric))
            {
                var metricType = AllCaches.MetricTypes.Read(new AccountCacheRequest()
                {
                    AccountId = metric.AccountId,
                    ObjectId = metric.MetricTypeId
                });
                processDate = metricWrite.ActualDate; // чтобы НЕ было пробела
                var actualDate = EventHelper.InfiniteActualDate;
                var noSignalColor = metricWrite.NoSignalColor ?? metricType.NoSignalColor ?? ObjectColor.Red;
                var status = MonitoringStatusHelper.Get(noSignalColor);
                SetMetricValue(metricType, metricWrite, null, processDate, actualDate, status, "Нет сигнала", false);
                metricWrite.BeginSave();
                return metric;
            }
        }

        public IMetricTypeCacheReadObject GetOrCreateType(Guid accountId, string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            if (accountId == Guid.Empty)
            {
                throw new ArgumentException("accountId == Guid.Empty");
            }
            var metricTypeId = AllCaches.MetricTypes.GetOrCreateTypeId(accountId, name);
            var request = new AccountCacheRequest()
            {
                AccountId = accountId,
                ObjectId = metricTypeId
            };
            return AllCaches.MetricTypes.Find(request);
        }

        public IMetricCacheReadObject CreateMetric(Guid accountId, CreateMetricRequestData data)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }
            var meticType = GetOrCreateType(accountId, data.MetricName);
            var metricId = CreateMetric(accountId, data.ComponentId, meticType.Id);
            var cache = new AccountCache(accountId);
            using (var metric = cache.Metrics.Write(metricId))
            {
                metric.ActualTime = TimeSpanHelper.FromSeconds(data.ActualTimeSecs);
                metric.NoSignalColor = data.NoSignalColor;
                metric.ConditionRed = data.AlarmCondition;
                metric.ConditionYellow = data.WarningCondition;
                metric.ConditionGreen = data.SuccessCondition;
                metric.ElseColor = data.ElseColor;
                metric.BeginSave();
            }
            var metrikRead = cache.Metrics.Read(metricId);
            metrikRead.WaitSaveChanges();
            return metrikRead;
        }

        public void DeleteMetric(Guid accountId, DeleteMetricRequestData data)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }
            if (data.MetricId == null)
            {
                throw new ParameterRequiredException("MetricId");
            }

            // удаляем метрику
            Guid componentId;
            Guid statusDataId;
            Guid componentMetricsStatusDataId;

            var metricRequest = new AccountCacheRequest()
            {
                AccountId = accountId,
                ObjectId = data.MetricId.Value
            };
            using (var metric = AllCaches.Metrics.Write(metricRequest))
            {
                componentId = metric.ComponentId;
                statusDataId = metric.StatusDataId;
                metric.IsDeleted = true;
                metric.BeginSave();
                metric.WaitSaveChanges();
            }

            // Проверим лимиты
            var limitChecker = AccountLimitsCheckerManager.GetCheckerForAccount(accountId);
            limitChecker.RefreshMetricsCount();

            // удаляем колбасу метрики
            var statusDataRequest = new AccountCacheRequest()
            {
                AccountId = accountId,
                ObjectId = statusDataId
            };
            using (var statusData = AllCaches.StatusDatas.Write(statusDataRequest))
            {
                statusData.IsDeleted = true;
                statusData.BeginSave();
            }

            // удаляем ссылку на метрику в компоненте
            var componentRequest = new AccountCacheRequest()
            {
                AccountId = accountId,
                ObjectId = componentId
            };
            using (var component = AllCaches.Components.Write(componentRequest))
            {
                componentMetricsStatusDataId = component.MetricsStatusId;
                component.WriteChilds.Delete(data.MetricId.Value);
                component.BeginSave();
            }

            // обновим колбасу метрик компонента
            var componentMetricsStatusDataIds = new List<Guid>();
            var component1 = AllCaches.Components.Find(componentRequest);
            foreach (var metricRef in component1.Metrics.GetAll())
            {
                var metric = AllCaches.Metrics.Find(new AccountCacheRequest()
                {
                    AccountId = accountId,
                    ObjectId = metricRef.Id
                });
                if (metric == null)
                {
                    throw new Exception("metric == null");
                }
                if (metric.IsDeleted)
                {
                    continue;
                }
                componentMetricsStatusDataIds.Add(metric.StatusDataId);
            }

            Context.BulbService.CalculateByChilds(
                accountId,
                componentMetricsStatusDataId,
                componentMetricsStatusDataIds);
        }

        public void DeleteMetricType(Guid accountId, Guid metricTypeId)
        {
            var cache = new AccountCache(accountId);

            using (var metricType = cache.MetricTypes.Write(metricTypeId))
            {
                metricType.IsDeleted = true;
                metricType.BeginSave();
            }

            var metrics = cache.Metrics.GetAllLoaded()
                .Where(x => x.MetricTypeId == metricTypeId)
                .ToArray();

            foreach (var metric in metrics)
            {
                using (var writeMetric = cache.Metrics.Write(metric))
                {
                    writeMetric.IsDeleted = true;
                    writeMetric.BeginSave();
                }
            }

            // подождем сохранения всех изменений в БД
            AllCaches.MetricTypes.SaveChanges();
            AllCaches.Metrics.SaveChanges();

            // обновим статистику лимитов
            var limitChecker = AccountLimitsCheckerManager.GetCheckerForAccount(accountId);
            limitChecker.RefreshMetricsCount();

            // статусы НЕ обновляем, т.к. это может выполняться очень долго
        }

        protected IBulbCacheReadObject SetMetricValue(
            IMetricTypeCacheReadObject metricType,
            MetricCacheWriteObject metric,
            double? value,
            DateTime processDate,
            DateTime actualDate,
            MonitoringStatus status,
            string message,
            bool hasSignal)
        {
            // Обновим текущие значения
            var equal = (value == null && metric.Value == null) || (value == metric.Value);
            if (!equal)
            {
                metric.BeginDate = processDate;
            }
            metric.ActualDate = actualDate;
            metric.Value = value;

            // Обновим статус метрики
            var statusService = Context.BulbService;
            var noSignalColor = metric.NoSignalColor ?? metricType.NoSignalColor ?? ObjectColor.Red;
            var noSignalImportance = EventImportanceHelper.Get(noSignalColor);
            var signal = new BulbSignal()
            {
                AccountId = metric.AccountId,
                ActualDate = metric.ActualDate,
                StartDate = metric.BeginDate,
                IsSpace = !hasSignal,
                EventId = Guid.Empty,
                Message = message,
                NoSignalImportance = noSignalImportance,
                ProcessDate = processDate,
                Status = status
            };

            var statusData = statusService.SetSignal(metric.StatusDataId, signal);

            // сохраним историю
            var color = ObjectColorHelper.Get(status);
            var history = new MetricHistory()
            {
                ComponentId = metric.ComponentId,
                MetricTypeId = metric.MetricTypeId,
                Value = value,
                BeginDate = processDate,
                ActualDate = actualDate,
                Color = color,
                StatusEventId = statusData.StatusEventId,
                HasSignal = hasSignal
            };

            var accountDbContext = Context.GetAccountDbContext(metric.AccountId);
            var historyRepository = accountDbContext.GetMetricHistoryRepository();
            historyRepository.Add(history);

            Context.SaveChanges();
            return statusData;
        }

        protected ObjectColor GetColor(IMetricTypeCacheReadObject metricType, IMetricCacheReadObject metric, double? value)
        {
            var conditionRed = metric.ConditionRed ?? metricType.ConditionRed;
            var conditionYellow = metric.ConditionYellow ?? metricType.ConditionYellow;
            var conditionGreen = metric.ConditionGreen ?? metricType.ConditionGreen;

            var color = metric.ElseColor ?? metricType.ElseColor ?? ObjectColor.Gray;

            var parameters = new[] { new KeyValuePair<string, double?>("value", value) };

            if (ConditionChecker.Check(parameters, conditionRed))
                color = ObjectColor.Red;
            else if (ConditionChecker.Check(parameters, conditionYellow))
                color = ObjectColor.Yellow;
            else if (ConditionChecker.Check(parameters, conditionGreen))
                color = ObjectColor.Green;

            return color;
        }

        public void SaveMetrics(Guid accountId, List<SendMetricRequestData> rows)
        {
            foreach (var row in rows)
            {
                SaveMetric(accountId, row);
            }
        }

        public List<MetricHistory> GetMetricsHistory(Guid accountId, GetMetricsHistoryRequestData filter)
        {
            var accountDbContext = Context.GetAccountDbContext(accountId);
            var repository = accountDbContext.GetMetricHistoryRepository();
            Guid[] metricTypes = null;
            if (string.IsNullOrEmpty(filter.Name) == false)
            {
                metricTypes = new[]
                {
                    Guid.NewGuid()
                };
                var metricType = Context.GetAccountDbContext(accountId).GetMetricTypeRepository().GetOneOrNullByName(filter.Name);
                if (metricType != null)
                {
                    metricTypes = new[]
                    {
                        metricType.Id
                    };
                }
            }

            var maxCount = filter.MaxCount ?? 1000;
            if (maxCount > 1000)
                maxCount = 1000;

            var history = repository
                .GetByPeriod(filter.ComponentId.Value, filter.From, filter.To, metricTypes)
                .OrderByDescending(t => t.BeginDate)
                .Take(maxCount);

            return history.ToList();
        }

        public List<IMetricCacheReadObject> GetMetrics(Guid accountId, Guid componentId)
        {
            var component = AllCaches.Components.Find(new AccountCacheRequest()
            {
                AccountId = accountId,
                ObjectId = componentId
            });
            if (component == null)
            {
                throw new UnknownComponentIdException(componentId, accountId);
            }
            var metrics = new List<IMetricCacheReadObject>();
            foreach (var metricRef in component.Metrics.GetAll())
            {
                var metric = AllCaches.Metrics.Find(new AccountCacheRequest()
                {
                    AccountId = accountId,
                    ObjectId = metricRef.Id
                });
                if (metric == null)
                {
                    throw new Exception("metric == null");
                }
                if (metric.IsDeleted)
                {
                    continue;
                }
                metrics.Add(metric);
            }
            return metrics;
            //var accountDbContext = Context.GetAccountDbContext(accountId);
            //var componentRepository = accountDbContext.GetComponentRepository();
            //var component = componentRepository.GetById(accountId, componentId);
            //var metics = component.Metrics.Where(x => x.IsDeleted == false && x.MetricType.IsDeleted == false).ToList();
            //return metics;
        }

        public int UpdateMetrics(Guid accountId, int maxCount)
        {
            var accountDbContext = Context.GetAccountDbContext(accountId);
            var metricRepository = accountDbContext.GetMetricRepository();
            var metrics = metricRepository.GetNotActual(maxCount);
            var cache = new AccountCache(accountId);

            foreach (var metric in metrics)
            {
                var metricRead = cache.Metrics.Read(metric.Id);
                var processDate = DateTime.Now;
                GetActualMetricInternal(metricRead, processDate);
            }
            return metrics.Count;
        }

        public void EnableMetric(Guid accountId, Guid metricId)
        {
            var cache = new AccountCache(accountId);
            IMetricCacheReadObject metricRead = null;

            // изменим метрику
            using (var metric = cache.Metrics.Write(metricId))
            {
                metricRead = metric;
                metric.Enable = true;
                metric.BeginSave();
            }

            // обновим колбаски
            var statusService = Context.BulbService;
            var processDate = DateTime.Now;
            var unknownSignal = BulbSignal.CreateUnknown(accountId, processDate);
            var data = statusService.SetSignal(metricRead.StatusDataId, unknownSignal);
            Context.ComponentService.CalculateAllStatuses(accountId, metricRead.ComponentId);
            // return data;
        }

        protected IBulbCacheReadObject UpdateEnableOrDisableStatusData(IMetricCacheReadObject metric)
        {
            var statusService = Context.BulbService;
            IBulbCacheReadObject data = null;
            var processDate = DateTime.Now;
            if (metric.CanProcess)
            {
                var unknownSignal = BulbSignal.CreateUnknown(metric.AccountId, processDate);
                data = statusService.SetSignal(metric.StatusDataId, unknownSignal);
            }
            else
            {
                var disableSignal = BulbSignal.CreateDisable(metric.AccountId, processDate);
                data = statusService.SetSignal(metric.StatusDataId, disableSignal);
            }
            //Context.ComponentService.CalculateAllStatuses(metric.AccountId, metric.ComponentId);
            return data;
        }

        public void DisableMetric(Guid accountId, SetMetricDisableRequestData requestData)
        {
            if (requestData == null)
            {
                throw new ArgumentNullException("requestData");
            }
            var metricId = requestData.MetricId;
            var cache = new AccountCache(accountId);
            IMetricCacheReadObject metricRead = null;

            // изменим метрику
            using (var metric = cache.Metrics.Write(metricId))
            {
                metricRead = metric;
                metric.Enable = false;
                metric.DisableToDate = requestData.ToDate;
                metric.DisableComment = requestData.Comment;
                metric.BeginSave();
            }

            // обновим колбаски
            UpdateEnableOrDisableStatusData(metricRead);
        }

        public void UpdateMetricType(Guid accountId, UpdateMetricTypeRequestData data)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }
            if (string.IsNullOrWhiteSpace(data.SystemName))
            {
                throw new UserFriendlyException("Не указан SystemName");
            }
            if (string.IsNullOrWhiteSpace(data.DisplayName))
            {
                data.DisplayName = data.SystemName;
            }
            var cache = new AccountCache(accountId);
            using (var metricType = cache.MetricTypes.Write(data.MetricTypeId))
            {
                metricType.SystemName = data.SystemName;
                metricType.DisplayName = data.DisplayName;
                metricType.ConditionRed = data.AlarmCondition;
                metricType.ConditionYellow = data.WarningCondition;
                metricType.ConditionGreen = data.SuccessCondition;
                metricType.ElseColor = data.ElseColor;
                metricType.NoSignalColor = data.NoSignalColor;
                metricType.ActualTime = TimeSpanHelper.FromSeconds(data.ActualTimeSecs);
                metricType.BeginSave();
            }
            cache.MetricTypes.Read(data.MetricTypeId).WaitSaveChanges();
        }

        public IMetricTypeCacheReadObject CreateMetricType(Guid accountId, CreateMetricTypeRequestData data)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }
            if (string.IsNullOrWhiteSpace(data.SystemName))
            {
                throw new UserFriendlyException("Не укзан SystemName");
            }
            if (string.IsNullOrWhiteSpace(data.DisplayName))
            {
                data.DisplayName = data.SystemName;
            }
            var metricTypeId = AllCaches.MetricTypes.CreateMetricType(accountId, data.SystemName);
            var cache = new AccountCache(accountId);
            using (var metricType = cache.MetricTypes.Write(metricTypeId))
            {
                metricType.DisplayName = data.DisplayName;
                metricType.ConditionRed = data.AlarmCondition;
                metricType.ConditionYellow = data.WarningCondition;
                metricType.ConditionGreen = data.SuccessCondition;
                metricType.ElseColor = data.ElseColor;
                metricType.ActualTime = TimeSpanHelper.FromSeconds(data.ActualTimeSecs);
                metricType.NoSignalColor = data.NoSignalColor;
                metricType.BeginSave();
                metricType.WaitSaveChanges();
                return metricType;
            }
        }

        public void UpdateMetric(Guid accountId, UpdateMetricRequestData data)
        {
            var cache = new AccountCache(accountId);
            using (var metric = cache.Metrics.Write(data.MetricId))
            {
                metric.ConditionRed = data.AlarmCondition;
                metric.ConditionYellow = data.WarningCondition;
                metric.ConditionGreen = data.SuccessCondition;
                metric.ElseColor = data.ElseColor;
                metric.ActualTime = TimeSpanHelper.FromSeconds(data.ActualTimeSecs);
                metric.NoSignalColor = data.NoSignalColor;
                metric.BeginSave();
            }
            cache.Metrics.Read(data.MetricId).WaitSaveChanges();
        }

        public Guid CreateMetric(Guid accountId, Guid componentId, Guid metricTypeId)
        {
            // получаем компонент
            var componentRequest = new AccountCacheRequest()
            {
                AccountId = accountId,
                ObjectId = componentId
            };
            using (var component = AllCaches.Components.Write(componentRequest))
            {
                var child = component.Metrics.FindByMetricTypeId(metricTypeId);
                if (child != null)
                {
                    throw new UserFriendlyException("У компонента уже есть метрика данного типа");
                }

                // сохраняем метрику в БД
                using (var accountDbContext = AccountDbContext.CreateFromAccountIdLocalCache(accountId))
                {
                    // Проверим лимиты
                    var limitChecker = AccountLimitsCheckerManager.GetCheckerForAccount(accountId);
                    limitChecker.CheckMaxMetrics(accountDbContext);

                    var processDate = DateTime.Now;
                    var metricId = Guid.NewGuid();

                    var statusData = Context.BulbService.CreateBulb(
                        accountId,
                        processDate,
                        EventCategory.MetricStatus,
                        metricId);

                    // загрузим в новый контекст
                    statusData = accountDbContext.Bulbs.Find(statusData.Id);

                    var metric = new Metric()
                    {
                        Id = metricId,
                        BeginDate = processDate,
                        ActualDate = processDate.AddDays(1),
                        ComponentId = component.Id,
                        Enable = true,
                        IsDeleted = false,
                        MetricTypeId = metricTypeId,
                        ParentEnable = component.CanProcess,
                        StatusDataId = statusData.Id,
                        //StatusData = statusData,
                        CreateDate = DateTime.Now,
                        Value = null
                    };

                    statusData.MetricId = metric.Id;
                    statusData.Metric = metric;

                    var repository = accountDbContext.GetMetricRepository();
                    repository.Add(metric);
                    accountDbContext.SaveChanges();

                    // обновим статистику лимитов
                    limitChecker.RefreshMetricsCount();

                    // обновляем ссылки у компонента
                    var metricReference = new CacheObjectReference(metric.Id, metricTypeId.ToString());
                    component.WriteMetrics.Add(metricReference);

                    component.BeginSave();
                    return metric.Id;
                }
            }
        }

        protected IMetricCacheReadObject FindMetric(Guid accountId, Guid componentId, Guid metricTypeId)
        {
            // получим компонент
            var componentRequest = new AccountCacheRequest()
            {
                AccountId = accountId,
                ObjectId = componentId
            };
            var component = AllCaches.Components.Find(componentRequest);
            if (component == null)
            {
                throw new UnknownComponentIdException(componentId, accountId);
            }

            // получим ссылку на метрику
            var metricReference = component.Metrics.FindByMetricTypeId(metricTypeId);
            if (metricReference == null)
            {
                return null;
            }

            // получим метрику по ссылке
            var metricRequest = new AccountCacheRequest()
            {
                AccountId = accountId,
                ObjectId = metricReference.Id
            };
            var metric = AllCaches.Metrics.Find(metricRequest);
            if (metric == null)
            {
                // такого быть НЕ должно, т.к. у компонента есть ссылка на метрику
                throw new Exception("metric == null");
            }
            return metric;
        }

        public IMetricCacheReadObject SaveMetric(Guid accountId, SendMetricRequestData data)
        {
            if (data.Value.HasValue && (double.IsNaN(data.Value.Value) || double.IsInfinity(data.Value.Value)))
            {
                throw new ParameterErrorException("Metric value can't be Nan or Infinity");
            }

            var processDate = DateTime.Now;
            var accountDbContext = Context.GetAccountDbContext(accountId);

            // Проверим лимиты
            var limitChecker = AccountLimitsCheckerManager.GetCheckerForAccount(accountId);
            limitChecker.CheckMetricsRequestsPerDay(accountDbContext);

            // Проверим лимит размера хранилища
            var size = data.GetSize();
            limitChecker.CheckStorageSize(accountDbContext, size);

            // получим тип метрики
            var metricType = GetOrCreateType(accountId, data.Name);

            // получим метрику
            var componentId = data.ComponentId.Value;
            var metricInfo = FindMetric(accountId, componentId, metricType.Id);
            if (metricInfo == null)
            {
                var createMetricLock = LockObject.ForComponent(componentId);
                lock (createMetricLock)
                {
                    metricInfo = FindMetric(accountId, componentId, metricType.Id);
                    if (metricInfo == null)
                    {
                        CreateMetric(accountId, componentId, metricType.Id);
                        metricInfo = FindMetric(accountId, componentId, metricType.Id);
                        if (metricInfo == null)
                        {
                            throw new Exception("Не удалось создать метрику");
                        }
                    }
                }
            }
            using (var metric = AllCaches.Metrics.Write(metricInfo))
            {
                GetActualMetricInternal(metric, processDate);

                // если метрика выключена
                if (metric.CanProcess == false)
                {
                    throw new ResponseCodeException(Zidium.Api.ResponseCode.ObjectDisabled, "Метрика выключена");
                }

                // Рассчитаем цвет
                ObjectColor color;
                string errorMessage = null;
                try
                {
                    color = GetColor(metricType, metric, data.Value);
                }
                catch (Exception exception)
                {
                    color = ObjectColor.Red;
                    errorMessage = "Ошибка вычисления цвета метрики:" + exception.Message;
                }

                // Запишем метрику в хранилище
                var status = MonitoringStatusHelper.Get(color);

                // Время актуальности
                var actualTime =
                    metric.ActualTime
                    ?? metricType.ActualTime
                    ?? TimeSpanHelper.FromSeconds(data.ActualIntervalSecs)
                    ?? TimeSpan.FromHours(1);

                var actualDate = processDate + actualTime;
                var statusData = SetMetricValue(metricType, metric, data.Value, processDate, actualDate, status, errorMessage, true);

                limitChecker.AddMetricsRequestsPerDay(accountDbContext);
                limitChecker.AddMetricsSizePerDay(accountDbContext, size);

                metric.BeginSave();
                return metric;
            }
        }

        public IMetricCacheReadObject GetActualMetric(Guid accountId, Guid componentId, string name)
        {
            var type = GetOrCreateType(accountId, name);
            return GetActualMetric(accountId, componentId, type.Id);
        }

        public IMetricCacheReadObject GetActualMetric(Guid accountId, Guid componentId, Guid metricTypeId)
        {
            var metric = FindMetric(accountId, componentId, metricTypeId);
            if (metric == null)
            {
                throw new ParameterErrorException("Не удалось найти метрику");
            }
            var processDate = DateTime.Now;
            return GetActualMetricInternal(metric, processDate);
        }
    }
}
