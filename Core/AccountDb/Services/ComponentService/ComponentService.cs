using System;
using System.Collections.Generic;
using System.Linq;
using Zidium.Api.Dto;
using Zidium.Common;
using Zidium.Core.Api;
using Zidium.Core.Caching;
using Zidium.Core.Common;
using Zidium.Core.Common.Helpers;
using Zidium.Storage;

namespace Zidium.Core.AccountsDb
{
    public class ComponentService : IComponentService
    {
        public ComponentService(IStorage storage)
        {
            _storage = storage;
        }

        private readonly IStorage _storage;

        public IBulbCacheReadObject GetComponentExternalState(Guid componentId, bool recalc = false)
        {
            // перед отправкой актуализируем все статусы
            if (recalc)
                CalculateAllStatuses(componentId);

            var component = GetComponentById(componentId);
            var request = new AccountCacheRequest()
            {
                ObjectId = component.ExternalStatusId
            };
            return AllCaches.StatusDatas.Find(request);
        }

        public IBulbCacheReadObject GetComponentInternalState(Guid componentId, bool recalc = false)
        {
            // перед отправкой актуализируем все статусы
            if (recalc)
                CalculateAllStatuses(componentId);

            var component = GetComponentById(componentId);
            var request = new AccountCacheRequest()
            {
                ObjectId = component.InternalStatusId
            };
            return AllCaches.StatusDatas.Find(request);
        }

        public IComponentCacheReadObject CalculateAllStatuses(Guid componentId)
        {
            var cache = new AccountCache();
            var component = cache.Components.Read(componentId);
            if (component.IsDeleted)
            {
                return component;
            }
            return RecalculateComponentStatuses(component);
        }

        public void CalculateEventsStatus(Guid componentId)
        {
            var request = new AccountCacheRequest()
            {
                ObjectId = componentId
            };
            var component = AllCaches.Components.Find(request);
            CalculateComponentEventsStatus(component.EventsStatusId);
        }

        protected void CalculateComponentEventsStatus(Guid eventsStatusId)
        {
            var processDate = DateTime.Now;
            var eventService = new EventService(_storage);
            var statusService = new BulbService(_storage);
            var request = new AccountCacheRequest()
            {
                ObjectId = eventsStatusId
            };

            // получаем объект для записи, чтобы заблокировать доступ, но ничего не пишем
            using (var data = AllCaches.StatusDatas.Write(request))
            {
                if (data.Actual(processDate))
                {
                    // событий не было вообще
                    if (data.LastEventId == null)
                    {
                        return;
                    }

                    // проверим, что событие актуально
                    var lastEvent = eventService.GetEventCacheOrNullById(data.LastEventId.Value);
                    if (lastEvent != null)
                    {
                        var isActual = EventImportanceHelper.Get(data.Status) == lastEvent.Importance
                           && lastEvent.ActualDate >= processDate;

                        if (isActual)
                        {
                            // ничего не изменилось, статус актуален
                            return;
                        }
                    }
                }

                // ищем событие, которое пересекается со статусом
                var nextEvent = eventService.GetDangerousEvent(
                    data.ComponentId.Value,
                    processDate);

                if (nextEvent != null)
                {
                    var noSignalImportance = EventImportance.Unknown;
                    var signal = BulbSignal.Create(processDate, nextEvent, noSignalImportance);
                    statusService.SetSignal(data.Id, signal);
                    return;
                }

                // нет актуальных событий
                var unknownSignal = BulbSignal.CreateUnknown(processDate);
                statusService.SetSignal(eventsStatusId, unknownSignal);
            }
        }

        public void ProcessEvent(Guid componentId, IEventCacheReadObject eventObj)
        {
            var categories = new[]
            {
                EventCategory.ApplicationError,
                EventCategory.ComponentEvent
            };

            if (categories.Contains(eventObj.Category) == false)
            {
                return;
            }

            var component = _storage.Components.GetOneById(componentId);

            // если выключен, то ничего не делаем
            if (!component.CanProcess())
            {
                return;
            }

            // отфильтруем события, которые не могут влиять на статус

            // если событие НЕ длится
            if (eventObj.StartDate == eventObj.ActualDate)
            {
                // возможно такие события могут продлить EndDate до eventObj.StartDate
                // пока будем их игнорировать, чтобы сделать алгоритм проще
                return;
            }
            var statusId = component.EventsStatusId;
            var statusService = new BulbService(_storage);
            var noSignalImportance = EventImportance.Unknown;

            // получаем или создаем статус (колбаску)
            var request = new AccountCacheRequest()
            {
                ObjectId = statusId
            };
            using (var data = AllCaches.StatusDatas.Write(request))
            {
                DateTime processDate = DateTime.Now;

                // событие из будущего (завершается через 5 минут)
                if (eventObj.EndDate > processDate + TimeSpan.FromMinutes(5))
                {
                    // пусть будет, эту проблему должен решать отправитель
                    //return UpdateStatusResult.Canceled();
                }

                // события из прошлого игнорируем, иначе будет ерунда при обновлении родительской колбасы
                // т.е. будет несоответствие наложения дочерних колбасок на родительскую
                if (eventObj.ActualDate <= processDate)
                {
                    return;
                }

                // если событие протухло
                if (eventObj.ActualDate <= data.EndDate)
                {
                    return;
                }

                var status = MonitoringStatusHelper.Get(eventObj.Importance);
                var signal = BulbSignal.Create(processDate, eventObj, noSignalImportance);

                // 0. если это первый сигнал после пробела
                if (data.HasSignal == false)
                {
                    statusService.SetSignal(data.Id, signal);
                    return;
                }

                // 1. событие менее важное, чем текущий статус
                if (status < data.Status)
                {
                    // событие поменяло важность, нодо расчитать занова!
                    if (data.LastEventId == eventObj.Id)
                    {
                        CalculateComponentEventsStatus(component.EventsStatusId);
                        return;
                    }

                    // текущий статус актуальней
                    if (data.ActualDate >= eventObj.ActualDate)
                    {
                        return;
                    }

                    // на актуальный статус менее важные события не влияют
                    if (data.Actual(processDate))
                    {
                        return;
                    }

                    // статус протух, а событие актуальное
                    // нужно расчитать, т.к. могут быть более важные актуальные события
                    CalculateComponentEventsStatus(component.EventsStatusId);
                    return;
                }

                // 2. событие такой же важности как статус
                if (status == data.Status)
                {
                    // продлеваем
                    statusService.SetSignal(data.Id, signal);
                    return;
                }

                // 3. событие более важное, чем текущий статус
                statusService.SetSignal(data.Id, signal);
            }
        }

        protected void UpdateParentEnableFlags(IComponentCacheReadObject component, bool recursive)
        {
            if (component == null)
            {
                throw new ArgumentNullException("component");
            }

            var cache = new AccountCache();
            var canProcess = component.CanProcess;

            // обновим проверки
            foreach (var unitTestRef in component.UnitTests.GetAll())
            {
                using (var unitTest = cache.UnitTests.Write(unitTestRef.Id))
                {
                    if (unitTest.IsDeleted)
                    {
                        continue;
                    }
                    if (unitTest.ParentEnable != canProcess)
                    {
                        unitTest.ParentEnable = canProcess;
                        unitTest.BeginSave();
                    }
                }
            }

            // обновим метрики
            foreach (var metricRef in component.Metrics.GetAll())
            {
                using (var metric = cache.Metrics.Write(metricRef.Id))
                {
                    if (metric.IsDeleted)
                    {
                        continue;
                    }
                    if (metric.ParentEnable != canProcess)
                    {
                        metric.ParentEnable = canProcess;
                        metric.BeginSave();
                    }
                }
            }

            // обновим детей
            foreach (var childRef in component.Childs.GetAll())
            {
                IComponentCacheReadObject childRead = null;
                using (var child = cache.Components.Write(childRef.Id))
                {
                    if (child.IsDeleted)
                    {
                        continue;
                    }
                    if (child.ParentEnable != canProcess)
                    {
                        childRead = child;
                        child.ParentEnable = canProcess;
                        child.BeginSave();
                    }
                }
                // если childRead is null, значит у ребенка уже актуальный флаг, его не надо апдейтить
                if (recursive && childRead != null)
                {
                    UpdateParentEnableFlags(childRead, recursive);
                }
            }
        }

        public void DisableComponent(Guid componentId, DateTime? toDate, string comment)
        {
            if (comment != null && comment.Length > 1000)
            {
                throw new ResponseCodeException(
                    ResponseCode.ParameterError,
                    "Максимальная длина комментария 1000 символов, а указано " + comment.Length);
            }

            if (toDate.HasValue && toDate < DateTime.Now)
            {
                throw new ResponseCodeException(
                    ResponseCode.ParameterError,
                    "Дата, до которой выключается компонент, должна быть больше текущего времени");
            }

            var cache = new AccountCache();

            // обновим компонент
            IComponentCacheReadObject componentCacheRead = null;
            using (var component = cache.Components.Write(componentId))
            {
                // настройки выключения
                component.Enable = false;
                component.DisableComment = comment;
                component.DisableToDate = toDate;

                componentCacheRead = component;
                component.BeginSave();
            }

            UpdateParentEnableFlags(componentCacheRead, true);

            RecalculateComponentStatuses(componentCacheRead);

            // обновим родительский компонент
            if (componentCacheRead.ParentId != null)
            {
                var parent = cache.Components.Read(componentCacheRead.ParentId.Value);
                RecalculateComponentStatuses(parent);
            }
        }

        protected IComponentCacheReadObject RecalculateComponentStatuses(IComponentCacheReadObject component)
        {
            var cache = new AccountCache();
            IComponentCacheReadObject result = component;

            // обновим ParentEnable
            if (component.ParentId.HasValue)
            {
                var parent = cache.Components.Read(component.ParentId.Value);
                if (parent.CanProcess != component.ParentEnable)
                {
                    using (var componentWrite = cache.Components.Write(component))
                    {
                        componentWrite.ParentEnable = parent.CanProcess;
                        componentWrite.BeginSave();
                        component = componentWrite;
                    }
                    UpdateParentEnableFlags(component, true);
                }
            }

            // если время выключения истекло, то включим компонент
            if (!component.Enable
                && component.DisableToDate.HasValue
                && component.DisableToDate < DateTime.Now)
            {
                using (var componentWrite = cache.Components.Write(component))
                {
                    componentWrite.Enable = true;
                    componentWrite.BeginSave();
                    component = componentWrite;
                    result = component;
                }
                UpdateParentEnableFlags(component, true);
            }

            var statusService = new BulbService(_storage);
            var allStatusesIds = component.GetAllStatusesIds();
            var allStatuses = allStatusesIds.Select(cache.StatusDatas.Read).ToList();

            // если надо выключить
            if (component.CanProcess == false && allStatuses.Any(x => x.Status != MonitoringStatus.Disabled))
            {
                // выключим все колбаски
                // выжно выключить их в этом порядке, чтобы не создать лишних событий колбасок-родителей
                var disableSignal = BulbSignal.CreateDisable(DateTime.Now);
                statusService.SetSignal(component.ExternalStatusId, disableSignal);
                statusService.SetSignal(component.ChildComponentsStatusId, disableSignal);
                statusService.SetSignal(component.InternalStatusId, disableSignal);
                statusService.SetSignal(component.EventsStatusId, disableSignal);
                statusService.SetSignal(component.UnitTestsStatusId, disableSignal);
                statusService.SetSignal(component.MetricsStatusId, disableSignal);
            }

            // обновим тесты
            var unitTestService = new UnitTestService(_storage);
            var allUnitTestsStatusDataIds = new List<Guid>();
            foreach (var unitTestRef in component.UnitTests.GetAll())
            {
                using (var unitTest = cache.UnitTests.Write(unitTestRef.Id))
                {
                    if (unitTest.IsDeleted)
                    {
                        continue;
                    }
                    allUnitTestsStatusDataIds.Add(unitTest.StatusDataId);
                    if (unitTest.ParentEnable != component.CanProcess)
                    {
                        unitTest.ParentEnable = component.CanProcess;
                        unitTest.BeginSave();
                    }
                }
                unitTestService.GetUnitTestResult(unitTestRef.Id);
            }

            // обновим метрики
            var metricService = new MetricService(_storage);
            var allMetricsStatusDataIds = new List<Guid>();
            Guid metricTypeId;
            foreach (var metricRef in component.Metrics.GetAll())
            {
                using (var metric = cache.Metrics.Write(metricRef.Id))
                {
                    if (metric.IsDeleted)
                    {
                        continue;
                    }
                    metricTypeId = metric.MetricTypeId;
                    allMetricsStatusDataIds.Add(metric.StatusDataId);
                    if (metric.ParentEnable != component.CanProcess)
                    {
                        metric.ParentEnable = component.CanProcess;
                        metric.BeginSave();
                    }
                }
                metricService.GetActualMetric(component.Id, metricTypeId);
            }

            // проверим, нужно ли обновить детей
            var childComponentsStatusDataIds = new List<Guid>();
            foreach (var childRef in component.Childs.GetAll())
            {
                using (var child = cache.Components.Write(childRef.Id))
                {
                    if (child.IsDeleted)
                    {
                        continue;
                    }
                    childComponentsStatusDataIds.Add(child.ExternalStatusId);
                    if (child.ParentEnable != component.CanProcess)
                    {
                        child.ParentEnable = component.CanProcess;
                        child.BeginSave();
                    }
                }
                GetComponentExternalState(childRef.Id);
            }

            if (component.CanProcess)
            {
                // обновим колбасу тестов явно (вдруг нет ни одного теста)
                statusService.CalculateByChilds(component.UnitTestsStatusId, allUnitTestsStatusDataIds.ToArray());

                // обновим колбасу метрик явно (вдруг нет ни одной метрики)
                statusService.CalculateByChilds(component.MetricsStatusId, allMetricsStatusDataIds.ToArray());

                // обновим колбасу событий
                CalculateComponentEventsStatus(component.EventsStatusId);

                // обновим внутренний статус
                var childsForInternal = new[]
                {
                    component.EventsStatusId,
                    component.UnitTestsStatusId,
                    component.MetricsStatusId
                };
                statusService.CalculateByChilds(component.InternalStatusId, childsForInternal);

                // обновим колбасу дочерних
                statusService.CalculateByChilds(component.ChildComponentsStatusId, childComponentsStatusDataIds.ToArray());

                // обновим внешний статус
                var childsForExternal = new[]
                {
                    component.InternalStatusId,
                    component.ChildComponentsStatusId
                };
                statusService.CalculateByChilds(component.ExternalStatusId, childsForExternal);
            }
            return result;
        }

        public void EnableComponent(Guid componentId)
        {
            var cache = new AccountCache();
            IComponentCacheReadObject componentCacheRead = null;
            using (var component = cache.Components.Write(componentId))
            {
                componentCacheRead = component;
                component.Enable = true;
                component.BeginSave();
            }
            UpdateParentEnableFlags(componentCacheRead, true);
            RecalculateComponentStatuses(componentCacheRead);
        }

        public ComponentForRead GetRootComponent(Guid accountId)
        {
            var root = _storage.Components.GetRoot();
            if (root == null)
            {
                throw new Exception("Не удалось найти корневой компонент");
            }
            return root;
        }

        public IComponentCacheReadObject GetComponentById(Guid id)
        {
            var cache = new AccountCache();
            var component = cache.Components.Read(id);

            if (component == null)
                throw new UnknownComponentIdException(id);

            return component;
        }

        // TODO Remove trivial method
        public ComponentForRead GetComponentByIdNoCache(Guid id)
        {
            return _storage.Components.GetOneById(id);
        }

        public ComponentForRead GetComponentBySystemName(Guid parentId, string systemName)
        {
            var parent = _storage.Components.GetOneById(parentId);
            var component = FindBySystemName(parent, systemName);

            if (component == null)
                throw new ParameterErrorException("Component not found");

            return component;
        }

        public ComponentForRead CreateComponent(CreateComponentRequestData data)
        {
            if (data == null)
            {
                throw new ParameterRequiredException("Request.Data");
            }
            var data2 = new GetOrCreateComponentRequestDataDto()
            {
                DisplayName = data.DisplayName,
                SystemName = data.SystemName,
                TypeId = data.TypeId,
                ParentComponentId = data.ParentComponentId,
                Version = data.Version,
                Properties = data.Properties
            };
            return GetOrCreateComponentInternal(data2, true);
        }

        protected ComponentForRead FindBySystemName(ComponentForRead parent, string systemName)
        {
            var childs = _storage.Components.GetChilds(parent.Id);
            var component = childs.FirstOrDefault(x => string.Equals(x.SystemName, systemName, StringComparison.InvariantCultureIgnoreCase));
            if (component != null)
            {
                return component;
            }
            foreach (var child in childs)
            {
                if (child.IsFolder())
                {
                    component = FindBySystemName(child, systemName);
                    if (component != null)
                    {
                        return component;
                    }
                }
            }
            return null;
        }

        public void CreateRoot(Guid rootId)
        {
            var root = new ComponentForAdd()
            {
                Id = rootId,
                DisplayName = "Root",
                SystemName = "Root",
                ComponentTypeId = SystemComponentType.Root.Id,
                CreatedDate = DateTime.Now,
                Enable = true,
                ParentEnable = true
            };
            Add(root);
        }

        private void Add(Guid componentId, Guid parentId, string displayName, string systemName, Guid componentTypeId)
        {
            var parent = _storage.Components.GetOneById(parentId);
            var componentType = _storage.ComponentTypes.GetOneById(componentTypeId);

            var component = new ComponentForAdd()
            {
                Id = componentId,
                ParentId = parent.Id,
                SystemName = systemName,
                DisplayName = displayName,
                ComponentTypeId = componentType.Id,
                Enable = true
            };
            Add(component);
        }

        private Guid CreateStatusData(ComponentForAdd component, EventCategory eventCategory)
        {
            var bulbService = new BulbService(_storage);
            return bulbService.CreateBulb(
                component.CreatedDate,
                eventCategory,
                component.Id,
                "Нет данных");
        }

        protected void Add(ComponentForAdd componentForAdd)
        {
            if (componentForAdd == null)
            {
                throw new ArgumentNullException("componentForAdd");
            }

            componentForAdd.CreatedDate = DateTime.Now;

            if (componentForAdd.ParentId != null)
            {
                if (componentForAdd.ComponentTypeId == SystemComponentType.Root.Id)
                {
                    throw new ResponseCodeException(
                        ResponseCode.UnknownComponentTypeId,
                        "Нельзя создавать экземпляры компонентов с типом Root");
                }

                var parentRequest = new AccountCacheRequest()
                {
                    ObjectId = componentForAdd.ParentId.Value
                };
                var parent = AllCaches.Components.Find(parentRequest);
                if (parent == null)
                {
                    throw new Exception("parent == null");
                }
                componentForAdd.ParentEnable = parent.CanProcess;
                var child = parent.Childs.FindByName(componentForAdd.SystemName);
                if (child != null)
                {
                    throw new Exception("У родителя уже есть компонент с таким SystemName");
                }
            }

            componentForAdd.ExternalStatusId = CreateStatusData(componentForAdd, EventCategory.ComponentExternalStatus);
            componentForAdd.InternalStatusId = CreateStatusData(componentForAdd, EventCategory.ComponentInternalStatus);
            componentForAdd.EventsStatusId = CreateStatusData(componentForAdd, EventCategory.ComponentEventsStatus);
            componentForAdd.UnitTestsStatusId = CreateStatusData(componentForAdd, EventCategory.ComponentUnitTestsStatus);
            componentForAdd.MetricsStatusId = CreateStatusData(componentForAdd, EventCategory.ComponentMetricsStatus);
            componentForAdd.ChildComponentsStatusId = CreateStatusData(componentForAdd, EventCategory.ComponentChildsStatus);

            using (var transaction = _storage.BeginTransaction())
            {

                _storage.Components.Add(componentForAdd);

                // обновим ссылку на компонент у статусов
                var component = _storage.Components.GetOneById(componentForAdd.Id);
                var statusIds = component.GetAllStatuses();
                foreach (var statusDataId in statusIds)
                {
                    var statusDataForUpdate = new BulbForUpdate(statusDataId);
                    statusDataForUpdate.ComponentId.Set(componentForAdd.Id);
                    _storage.Bulbs.Update(statusDataForUpdate);
                }

                var logConfig = new LogConfigForAdd()
                {
                    ComponentId = componentForAdd.Id,
                    Enabled = true,
                    IsFatalEnabled = true,
                    IsErrorEnabled = true,
                    IsWarningEnabled = true,
                    IsInfoEnabled = true,
                    IsDebugEnabled = false,
                    IsTraceEnabled = false,
                    LastUpdateDate = componentForAdd.CreatedDate
                };
                _storage.LogConfigs.Add(logConfig);

                transaction.Commit();
            }

            // обновим в кэше ссылки на дочерние компоненты у родителя 
            if (componentForAdd.ParentId != null)
            {
                var parentRequest = new AccountCacheRequest()
                {
                    ObjectId = componentForAdd.ParentId.Value
                };
                using (var parent = AllCaches.Components.Write(parentRequest))
                {
                    var componentRef = new CacheObjectReference(componentForAdd.Id, componentForAdd.SystemName);
                    parent.WriteChilds.Add(componentRef);
                    parent.BeginSave();
                }
            }
        }

        public ComponentForRead GetOrCreateComponentInternal(GetOrCreateComponentRequestDataDto data, bool createNew)
        {
            if (data == null)
            {
                throw new ParameterRequiredException("Request.Data");
            }
            if (data.TypeId == null)
            {
                throw new ParameterRequiredException("Request.Data.TypeId");
            }
            if (string.IsNullOrEmpty(data.SystemName))
            {
                throw new ParameterRequiredException("data.SystemName");
            }

            if (data.ParentComponentId == null)
            {
                data.ParentComponentId = _storage.Components.GetRoot().Id;
            }

            var componentId = data.NewId ?? Ulid.NewUlid();
            var systemName = data.SystemName;

            var lockObj = LockObject.ForComponent(systemName);
            lock (lockObj)
            {
                // ищем в детях
                var component = _storage.Components.GetChild(data.ParentComponentId.Value, systemName);
                bool isExists = true;
                if (component == null)
                {
                    // ищем в папках
                    var parent = _storage.Components.GetOneById(data.ParentComponentId.Value);
                    component = FindBySystemName(parent, systemName);
                    if (component == null)
                    {
                        // создаем новый
                        Add(componentId,
                            data.ParentComponentId.Value,
                            data.DisplayName ?? systemName,
                            systemName,
                            data.TypeId.Value);

                        isExists = false;

                        component = _storage.Components.GetOneById(componentId);
                    }
                }

                if (isExists && createNew)
                {
                    throw new ResponseCodeException(
                        ResponseCode.ParameterError,
                        "Компонент с таким системным именем уже существует");
                }

                // обновим версию
                if (!string.IsNullOrEmpty(data.Version))
                {
                    var componentForUpdate = component.GetForUpdate();
                    componentForUpdate.Version.Set(data.Version);
                    _storage.Components.Update(componentForUpdate);
                    component = _storage.Components.GetOneById(component.Id);
                }

                // обновим свойства
                // TODO if properties not changed, data.Properties should be null
                if (data.Properties != null && data.Properties.Count > 0)
                {
                    UpdateComponentProperies(component.Id, data.Properties);
                }

                return component;
            }
        }

        public ComponentForRead GetOrCreateComponent(GetOrCreateComponentRequestDataDto data)
        {
            if (data == null)
            {
                throw new ParameterRequiredException("Request.Data");
            }
            return GetOrCreateComponentInternal(data, false);
        }

        private void UpdateComponentProperies(Guid componentId, List<ExtentionPropertyDto> properties)
        {
            var componentProperties = _storage.ComponentProperties.GetByComponentId(componentId);

            // удаляем
            var forDelete = componentProperties.Where(x => properties.All(y => !string.Equals(y.Name, x.Name, StringComparison.Ordinal))).ToArray();
            foreach (var componentProperty in forDelete)
            {
                _storage.ComponentProperties.Delete(componentProperty.Id);
            }

            // обновляем или вставляем
            foreach (var property in properties)
            {
                var componentProperty = componentProperties.FirstOrDefault(t => string.Equals(t.Name, property.Name, StringComparison.Ordinal));

                if (componentProperty == null)
                {
                    _storage.ComponentProperties.Add(new ComponentPropertyForAdd()
                    {
                        Id = Ulid.NewUlid(),
                        ComponentId = componentId,
                        Name = property.Name,
                        Value = property.Value,
                        DataType = property.Type
                    });
                }
                else
                {
                    var componentPropertyForUpdate = componentProperty.GetForUpdate();
                    componentPropertyForUpdate.Value.Set(property.Value);
                    componentPropertyForUpdate.DataType.Set(property.Type);
                    _storage.ComponentProperties.Update(componentPropertyForUpdate);
                }
            }
        }

        public void UpdateComponent(UpdateComponentRequestDataDto data)
        {
            // проверим существование компонента
            var oldComponentRequest = new AccountCacheRequest()
            {
                ObjectId = data.Id.Value
            };
            var oldComponent = AllCaches.Components.Find(oldComponentRequest);
            if (oldComponent == null)
            {
                throw new UnknownComponentIdException(data.Id.Value);
            }

            // проверим нового родителя
            if (data.ParentId.HasValue)
            {
                var parent = AllCaches.Components.Find(new AccountCacheRequest()
                {
                    ObjectId = data.ParentId.Value
                });
                if (parent == null)
                {
                    throw new UnknownComponentIdException(data.ParentId.Value);
                }
            }

            // проверим уникальность системного имени
            if (!string.IsNullOrEmpty(data.SystemName))
            {
                var parentId = data.ParentId ?? oldComponent.ParentId.Value;
                var parent = AllCaches.Components.Find(new AccountCacheRequest()
                {
                    ObjectId = parentId
                });
                if (parent == null)
                {
                    throw new UnknownComponentIdException(parentId);
                }
                var child = parent.Childs.FindByName(data.SystemName);
                if (child != null && child.Id != data.Id)
                {
                    throw new ParameterErrorException("Системное имя должно быть уникальным");
                }
            }

            // проверим тип
            if (data.TypeId.HasValue)
            {
                if (_storage.ComponentTypes.GetOneOrNullById(data.TypeId.Value) == null)
                {
                    throw new ParameterErrorException("TypeId");
                }
            }

            var request = new AccountCacheRequest()
            {
                ObjectId = data.Id.Value
            };

            using (var component = AllCaches.Components.Write(request))
            {
                if (!string.IsNullOrEmpty(data.DisplayName))
                {
                    component.DisplayName = data.DisplayName;
                }

                if (!string.IsNullOrEmpty(data.SystemName))
                {
                    component.SystemName = data.SystemName;
                }

                if (data.ParentId.HasValue)
                {
                    component.ParentId = data.ParentId.Value;
                }

                if (data.TypeId.HasValue)
                {
                    component.ComponentTypeId = data.TypeId.Value;
                }

                if (!string.IsNullOrEmpty(data.Version))
                {
                    component.Version = data.Version;
                }

                component.BeginSave();
            }

            AllCaches.Components.SaveChanges();

            if (data.Properties != null && data.Properties.Count > 0)
            {
                UpdateComponentProperies(data.Id.Value, data.Properties);
            }
        }

        // TODO Remove trivial method
        public ComponentForRead[] GetChildComponents(Guid parentComponentId)
        {
            return _storage.Components.GetChilds(parentComponentId);
        }

        public void DeleteComponent(Guid componentId)
        {
            var cache = new AccountCache();
            ComponentCacheWriteObject componentWrite;

            // Удаление компонента
            using (componentWrite = cache.Components.Write(componentId))
            {
                if (componentWrite.IsDeleted)
                {
                    return;
                }
                if (componentWrite.IsRoot)
                {
                    throw new UserFriendlyException("Нельзя удалить root компонент");
                }
                componentWrite.IsDeleted = true;
                componentWrite.BeginSave();
            }
            componentWrite.WaitSaveChanges();
            IComponentCacheReadObject component = componentWrite;

            // удаляем колбаски
            foreach (var statusDataId in component.GetAllStatusesIds())
            {
                using (var statusData = cache.StatusDatas.Write(statusDataId))
                {
                    statusData.IsDeleted = true;
                    statusData.BeginSave();
                }
            }

            // Удаление метрик
            foreach (var meticRef in component.Metrics.GetAll())
            {
                using (var metric = cache.Metrics.Write(meticRef.Id))
                {
                    metric.IsDeleted = true;
                    metric.BeginSave();

                    // удаление колбаски метрики
                    using (var metricStatusData = cache.StatusDatas.Write(metric.StatusDataId))
                    {
                        metricStatusData.IsDeleted = true;
                        metricStatusData.BeginSave();
                    }
                }
            }

            // Удаление проверок
            foreach (var unitTestRef in component.UnitTests.GetAll())
            {
                using (var unitTest = cache.UnitTests.Write(unitTestRef.Id))
                {
                    unitTest.IsDeleted = true;
                    unitTest.BeginSave();

                    // удаление колбаски проверки
                    using (var metricStatusData = cache.StatusDatas.Write(unitTest.StatusDataId))
                    {
                        metricStatusData.IsDeleted = true;
                        metricStatusData.BeginSave();
                    }
                }
            }

            // Удаление вложенных компонентов
            foreach (var child in component.Childs.GetAll())
            {
                DeleteComponent(child.Id);
            }

            // Если есть родитель, обновим его статус
            if (component.ParentId.HasValue)
                CalculateAllStatuses(component.ParentId.Value);
        }

        public int UpdateEventsStatuses(int maxCount)
        {
            var statusIds = _storage.Components.GetNotActualEventsStatusIds(DateTime.Now, maxCount);
            foreach (var statusId in statusIds)
            {
                CalculateComponentEventsStatus(statusId);
            }
            return statusIds.Length;
        }

        public Guid[] GetComponentAndChildIds(Guid componentId)
        {
            var allComponents = _storage.Components.GetAllIdsWithParents()
                .Select(t => new SimplifiedComponentInfo()
                {
                    Id = t.Id,
                    ParentId = t.ParentId
                }).ToDictionary(t => t.Id, t => t);

            foreach (var component in allComponents.Values)
            {
                if (component.ParentId != null)
                    allComponents[component.ParentId.Value].Childs.Add(component);
            }

            var result = new List<Guid>();
            AddComponentAndChildIdsToList(allComponents[componentId], result);

            return result.ToArray();
        }

        public string GetFullDisplayName(ComponentForRead component)
        {
            var list = new List<string>();
            while (component != null)
            {
                list.Add(component.DisplayName);
                component = component.ParentId.HasValue ? _storage.Components.GetOneById(component.ParentId.Value) : null;
            }
            list.Reverse();
            return string.Join(" / ", list);
        }

        private void AddComponentAndChildIdsToList(SimplifiedComponentInfo component, List<Guid> list)
        {
            list.Add(component.Id);

            foreach (var child in component.Childs)
            {
                AddComponentAndChildIdsToList(child, list);
            }
        }

        private class SimplifiedComponentInfo
        {
            public Guid Id { get; set; }

            public Guid? ParentId { get; set; }

            public List<SimplifiedComponentInfo> Childs { get; }

            public SimplifiedComponentInfo()
            {
                Childs = new List<SimplifiedComponentInfo>();
            }
        }
    }
}
