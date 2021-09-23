using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Zidium.Api.Dto;
using Zidium.Common;
using Zidium.Core.Api;
using Zidium.Core.Caching;
using Zidium.Core.Common;
using Zidium.Core.Common.Helpers;
using Zidium.Core.Limits;
using Zidium.Storage;

namespace Zidium.Core.AccountsDb
{
    public class MetricService : IMetricService
    {
        public MetricService(IStorage storage)
        {
            _storage = storage;
            _logger = DependencyInjection.GetLogger<MetricService>();
        }

        private readonly IStorage _storage;
        private readonly ILogger _logger;

        private IMetricCacheReadObject GetActualMetricInternal(IMetricCacheReadObject metric, DateTime processDate)
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

            var statusService = new BulbService(_storage);
            var data = statusService.GetRaw(metric.StatusDataId);

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
            if (data.Actual(processDate) && metric.ActualDate > processDate)
            {
                return metric;
            }

            // значение метрики не актуальное
            // сохраняем пробел
            SaveNoSignalColor(metric, processDate);

            // Перечитаем новые данные метрики
            var metricRequest = new AccountCacheRequest()
            {
                ObjectId = metric.Id
            };
            metric = AllCaches.Metrics.Find(metricRequest);

            return metric;
        }

        private void SaveNoSignalColor(IMetricCacheReadObject metric, DateTime processDate)
        {
            using (var metricWrite = AllCaches.Metrics.Write(metric))
            {
                var metricType = AllCaches.MetricTypes.Read(new AccountCacheRequest()
                {
                    ObjectId = metric.MetricTypeId
                });

                var actualDate = DateTimeHelper.InfiniteActualDate;
                var noSignalColor = metricWrite.NoSignalColor ?? metricType.NoSignalColor ?? ObjectColor.Red;
                var status = MonitoringStatusHelper.Get(noSignalColor);
                SetMetricValue(metricType, metricWrite, null, processDate, actualDate, status, "Нет сигнала", false);
                metricWrite.BeginSave();
            }
        }

        public IMetricTypeCacheReadObject GetOrCreateType(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            var metricTypeId = AllCaches.MetricTypes.GetOrCreateTypeId(name, _storage);
            var request = new AccountCacheRequest()
            {
                ObjectId = metricTypeId
            };
            return AllCaches.MetricTypes.Find(request);
        }

        public IMetricCacheReadObject CreateMetric(CreateMetricRequestData data)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }
            var meticType = GetOrCreateType(data.MetricName);
            var metricId = CreateMetric(data.ComponentId, meticType.Id);
            var cache = new AccountCache();
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
            var metricRead = cache.Metrics.Read(metricId);
            metricRead.WaitSaveChanges();
            return metricRead;
        }

        public void DeleteMetric(DeleteMetricRequestData data)
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

            // удаляем колбасу метрики
            var statusDataRequest = new AccountCacheRequest()
            {
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

            var bulbService = new BulbService(_storage);
            bulbService.CalculateByChilds(
                componentMetricsStatusDataId,
                componentMetricsStatusDataIds.ToArray());
        }

        public void DeleteMetricType(Guid metricTypeId)
        {
            var cache = new AccountCache();

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
            var equal = (value == null && metric.Value == null) || (value != null && metric.Value != null && Math.Abs(value.Value - metric.Value.Value) <= double.Epsilon);
            if (!equal)
            {
                metric.BeginDate = processDate;
            }
            metric.ActualDate = actualDate;
            metric.Value = value;

            // Обновим статус метрики
            var statusService = new BulbService(_storage);
            var noSignalColor = metric.NoSignalColor ?? metricType.NoSignalColor ?? ObjectColor.Red;
            var noSignalImportance = EventImportanceHelper.Get(noSignalColor);
            var signal = new BulbSignal()
            {
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
            var history = new MetricHistoryForAdd()
            {
                Id = Ulid.NewUlid(),
                ComponentId = metric.ComponentId,
                MetricTypeId = metric.MetricTypeId,
                Value = value,
                BeginDate = processDate,
                ActualDate = actualDate,
                Color = color,
                StatusEventId = statusData.StatusEventId,
                HasSignal = hasSignal
            };

            _storage.MetricHistory.Add(history);

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

        public void SaveMetrics(List<SendMetricRequestDataDto> rows)
        {
            foreach (var row in rows)
            {
                SaveMetric(row);
            }
        }

        public MetricHistoryForRead[] GetMetricsHistory(GetMetricsHistoryRequestDataDto filter)
        {
            Guid[] metricTypes = null;
            if (string.IsNullOrEmpty(filter.Name) == false)
            {
                metricTypes = new[]
                {
                    Ulid.NewUlid()
                };
                var metricType = _storage.MetricTypes.GetOneOrNullBySystemName(filter.Name);
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

            var history = _storage.MetricHistory
                .GetByPeriod(filter.ComponentId.Value, filter.From, filter.To, metricTypes, maxCount);

            return history;
        }

        public List<IMetricCacheReadObject> GetMetrics(Guid componentId)
        {
            var component = AllCaches.Components.Find(new AccountCacheRequest()
            {
                ObjectId = componentId
            });
            if (component == null)
            {
                throw new UnknownComponentIdException(componentId);
            }
            var metrics = new List<IMetricCacheReadObject>();
            foreach (var metricRef in component.Metrics.GetAll())
            {
                var metric = AllCaches.Metrics.Find(new AccountCacheRequest()
                {
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
        }

        public int UpdateMetrics(int maxCount)
        {
            var processDate = DateTime.Now;
            var metricIds = _storage.Metrics.GetNotActualIds(maxCount, processDate);
            var cache = new AccountCache();

            foreach (var metricId in metricIds)
            {
                try
                {
                    var metricRead = cache.Metrics.Read(metricId);
                    GetActualMetricInternal(metricRead, processDate);
                }
                catch (Exception exception)
                {
                    _logger.LogError(exception, exception.Message);
                }
            }
            return metricIds.Length;
        }

        public void EnableMetric(Guid metricId)
        {
            var cache = new AccountCache();
            IMetricCacheReadObject metricRead = null;

            // изменим метрику
            using (var metric = cache.Metrics.Write(metricId))
            {
                metricRead = metric;
                metric.Enable = true;
                metric.BeginSave();
            }

            // обновим колбаски
            var statusService = new BulbService(_storage);
            var processDate = DateTime.Now;
            var unknownSignal = BulbSignal.CreateUnknown(processDate);
            statusService.SetSignal(metricRead.StatusDataId, unknownSignal);
            var componentService = new ComponentService(_storage);
            componentService.CalculateAllStatuses(metricRead.ComponentId);
        }

        protected IBulbCacheReadObject UpdateEnableOrDisableStatusData(IMetricCacheReadObject metric)
        {
            var statusService = new BulbService(_storage);
            IBulbCacheReadObject data = null;
            var processDate = DateTime.Now;
            if (metric.CanProcess)
            {
                var unknownSignal = BulbSignal.CreateUnknown(processDate);
                data = statusService.SetSignal(metric.StatusDataId, unknownSignal);
            }
            else
            {
                var disableSignal = BulbSignal.CreateDisable(processDate);
                data = statusService.SetSignal(metric.StatusDataId, disableSignal);
            }
            return data;
        }

        public void DisableMetric(SetMetricDisableRequestData requestData)
        {
            if (requestData == null)
            {
                throw new ArgumentNullException("requestData");
            }
            var metricId = requestData.MetricId;
            var cache = new AccountCache();
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

        public void UpdateMetricType(UpdateMetricTypeRequestData data)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }
            if (string.IsNullOrWhiteSpace(data.SystemName))
            {
                throw new ArgumentNullException("data.SystemName");
            }
            if (string.IsNullOrWhiteSpace(data.DisplayName))
            {
                data.DisplayName = data.SystemName;
            }

            bool noSignalColorChanged;
            var cache = new AccountCache();
            using (var metricTypeWrite = cache.MetricTypes.Write(data.MetricTypeId))
            {
                noSignalColorChanged = metricTypeWrite.NoSignalColor != data.NoSignalColor;
                metricTypeWrite.SystemName = data.SystemName;
                metricTypeWrite.DisplayName = data.DisplayName;
                metricTypeWrite.ConditionRed = data.AlarmCondition;
                metricTypeWrite.ConditionYellow = data.WarningCondition;
                metricTypeWrite.ConditionGreen = data.SuccessCondition;
                metricTypeWrite.ElseColor = data.ElseColor;
                metricTypeWrite.NoSignalColor = data.NoSignalColor;
                metricTypeWrite.ActualTime = TimeSpanHelper.FromSeconds(data.ActualTimeSecs);
                metricTypeWrite.BeginSave();
            }
            var metricType = cache.MetricTypes.Read(data.MetricTypeId);
            metricType.WaitSaveChanges();

            // При изменении "Цвет если нет сигнала" нужно обновить цвет всех метрик этого типа
            if (noSignalColorChanged)
            {
                var metricIds = _storage.Metrics.GetByMetricTypeId(metricType.Id);

                foreach (var metricId in metricIds)
                {
                    var metric = cache.Metrics.Read(metricId);
                    UpdateNoSignalColor(metric);
                }
            }
        }

        public IMetricTypeCacheReadObject CreateMetricType(CreateMetricTypeRequestData data)
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
            var metricTypeId = AllCaches.MetricTypes.CreateMetricType(data.SystemName, _storage);
            var cache = new AccountCache();
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

        public void UpdateMetric(UpdateMetricRequestData data)
        {
            var noSignalcolorChanged = false;
            var cache = new AccountCache();
            using (var metricWrite = cache.Metrics.Write(data.MetricId))
            {
                noSignalcolorChanged = metricWrite.NoSignalColor != data.NoSignalColor;
                metricWrite.ConditionRed = data.AlarmCondition;
                metricWrite.ConditionYellow = data.WarningCondition;
                metricWrite.ConditionGreen = data.SuccessCondition;
                metricWrite.ElseColor = data.ElseColor;
                metricWrite.ActualTime = TimeSpanHelper.FromSeconds(data.ActualTimeSecs);
                metricWrite.NoSignalColor = data.NoSignalColor;
                metricWrite.BeginSave();
            }

            var metric = cache.Metrics.Read(data.MetricId);
            metric.WaitSaveChanges();

            // При изменении "Цвет если нет сигнала" нужно обновить цвет метрики
            if (noSignalcolorChanged)
            {
                UpdateNoSignalColor(metric);
            }
        }

        public void UpdateNoSignalColor(IMetricCacheReadObject metric)
        {
            var statusService = new BulbService(_storage);
            var statusData = statusService.GetRaw(metric.StatusDataId);
            if (!statusData.HasSignal)
            {
                SaveNoSignalColor(metric, DateTime.Now);
            }
        }

        public Guid CreateMetric(Guid componentId, Guid metricTypeId)
        {
            // получаем компонент
            var componentRequest = new AccountCacheRequest()
            {
                ObjectId = componentId
            };
            using (var component = AllCaches.Components.Write(componentRequest))
            {
                var child = component.Metrics.FindByMetricTypeId(metricTypeId);
                if (child != null)
                {
                    throw new UserFriendlyException("У компонента уже есть метрика данного типа");
                }

                var processDate = DateTime.Now;
                var metricId = Ulid.NewUlid();

                var bulbService = new BulbService(_storage);
                var statusDataId = bulbService.CreateBulb(
                    processDate,
                    EventCategory.MetricStatus,
                    metricId,
                    "Метрика создана, отправьте значение метрики");

                using (var transaction = _storage.BeginTransaction())
                {
                    var metric = new MetricForAdd()
                    {
                        Id = metricId,
                        BeginDate = processDate,
                        ActualDate = processDate.AddDays(1),
                        ComponentId = component.Id,
                        Enable = true,
                        IsDeleted = false,
                        MetricTypeId = metricTypeId,
                        ParentEnable = component.CanProcess,
                        StatusDataId = statusDataId,
                        CreateDate = DateTime.Now,
                        Value = null
                    };

                    _storage.Metrics.Add(metric);

                    var statusDataForUpdate = new BulbForUpdate(statusDataId);
                    statusDataForUpdate.MetricId.Set(metric.Id);
                    _storage.Bulbs.Update(statusDataForUpdate);

                    transaction.Commit();
                }

                // обновляем ссылки у компонента
                var metricReference = new CacheObjectReference(metricId, metricTypeId.ToString());
                component.WriteMetrics.Add(metricReference);

                component.BeginSave();
                return metricId;
            }
        }

        protected IMetricCacheReadObject FindMetric(Guid componentId, Guid metricTypeId)
        {
            // получим компонент
            var componentRequest = new AccountCacheRequest()
            {
                ObjectId = componentId
            };
            var component = AllCaches.Components.Find(componentRequest);
            if (component == null)
            {
                throw new UnknownComponentIdException(componentId);
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

        public IMetricCacheReadObject SaveMetric(SendMetricRequestDataDto data)
        {
            if (data.Value.HasValue && (double.IsNaN(data.Value.Value) || double.IsInfinity(data.Value.Value)))
            {
                throw new ParameterErrorException("Metric value can't be Nan or Infinity");
            }

            if (data.ActualIntervalSecs.HasValue && data.ActualIntervalSecs < 0)
            {
                throw new ParameterErrorException("ActualIntervalSecs must be >= 0");
            }

            var processDate = DateTime.Now;

            var limitChecker = AccountLimitsCheckerManager.GetChecker();
            var size = data.GetSize();

            try
            {
                try
                {
                    // получим тип метрики
                    var metricType = GetOrCreateType(data.Name);

                    // получим метрику
                    var componentId = data.ComponentId.Value;
                    var metricInfo = FindMetric(componentId, metricType.Id);
                    if (metricInfo == null)
                    {
                        var createMetricLock = LockObject.ForComponent(componentId);
                        lock (createMetricLock)
                        {
                            metricInfo = FindMetric(componentId, metricType.Id);
                            if (metricInfo == null)
                            {
                                CreateMetric(componentId, metricType.Id);
                                metricInfo = FindMetric(componentId, metricType.Id);
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
                            throw new ResponseCodeException(ResponseCode.ObjectDisabled, "Метрика выключена");
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
                        var actualInterval =
                            metric.ActualTime
                            ?? metricType.ActualTime
                            ?? TimeSpanHelper.FromSeconds(data.ActualIntervalSecs)
                            ?? TimeSpan.FromHours(1);

                        DateTime actualDate;
                        try
                        {
                            actualDate = processDate + actualInterval;
                        }
                        catch
                        {
                            actualDate = DateTimeHelper.InfiniteActualDate;
                        }

                        SetMetricValue(metricType, metric, data.Value, processDate, actualDate, status, errorMessage, true);

                        metric.BeginSave();
                        return metric;
                    }
                }
                finally
                {
                    limitChecker.AddMetricsSizePerDay(_storage, size);
                }
            }
            finally
            {
                limitChecker.AddMetricsRequestsPerDay(_storage);
            }
        }

        public IMetricCacheReadObject GetActualMetric(Guid componentId, string name)
        {
            var type = GetOrCreateType(name);
            return GetActualMetric(componentId, type.Id);
        }

        public IMetricCacheReadObject GetActualMetric(Guid componentId, Guid metricTypeId)
        {
            var metric = FindMetric(componentId, metricTypeId);
            if (metric == null)
            {
                throw new ParameterErrorException("Не удалось найти метрику");
            }
            var processDate = DateTime.Now;
            return GetActualMetricInternal(metric, processDate);
        }

        public string GetFullDisplayName(MetricForRead metric)
        {
            var component = _storage.Components.GetOneById(metric.ComponentId);
            var metricType = _storage.MetricTypes.GetOneById(metric.MetricTypeId);
            var componentService = new ComponentService(_storage);
            return componentService.GetFullDisplayName(component) + " / " + metricType.DisplayName;
        }
    }
}
