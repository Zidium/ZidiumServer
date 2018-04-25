using System;
using System.Collections.Generic;
using System.Linq;
using Zidium.Api;
using Zidium.Api.Dto;
using Zidium.Core.Api;
using Zidium.Core.Caching;
using Zidium.Core.Common;
using Zidium.Core.Common.Helpers;
using Zidium.Core.DispatcherLayer;
using Zidium.Core.Limits;

namespace Zidium.Core.AccountsDb
{
    public class ComponentService : IComponentService
    {
        protected DispatcherContext Context { get; set; }

        public ComponentService(DispatcherContext dispatcherContext)
        {
            if (dispatcherContext == null)
            {
                throw new ArgumentNullException("dispatcherContext");
            }
            Context = dispatcherContext;
        }

        public IBulbCacheReadObject GetComponentExternalState(Guid accountId, Guid componentId, bool recalc = false)
        {
            // перед отправкой актуализируем все статусы
            if (recalc)
                CalculateAllStatuses(accountId, componentId);

            var component = GetComponentById(accountId, componentId);
            var request = new AccountCacheRequest()
            {
                AccountId = accountId,
                ObjectId = component.ExternalStatusId
            };
            return AllCaches.StatusDatas.Find(request);
        }

        public IBulbCacheReadObject GetComponentInternalState(Guid accountId, Guid componentId, bool recalc = false)
        {
            // перед отправкой актуализируем все статусы
            if (recalc)
                CalculateAllStatuses(accountId, componentId);

            var component = GetComponentById(accountId, componentId);
            var request = new AccountCacheRequest()
            {
                AccountId = accountId,
                ObjectId = component.InternalStatusId
            };
            return AllCaches.StatusDatas.Find(request);
        }

        public IComponentCacheReadObject CalculateAllStatuses(Guid accountId, Guid componentId)
        {
            var cache = new AccountCache(accountId);
            var component = cache.Components.Read(componentId);
            if (component.IsDeleted)
            {
                return component;
            }
            return RecalculateComponentStatuses(component);
        }

        public void CalculateEventsStatus(Guid accountId, Guid componentId)
        {
            var request = new AccountCacheRequest()
            {
                AccountId = accountId,
                ObjectId = componentId
            };
            var component = AllCaches.Components.Find(request);
            CalculateComponentEventsStatus(accountId, component.EventsStatusId);
        }

        protected void CalculateComponentEventsStatus(Guid accountId, Guid eventsStatusId)
        {
            var processDate = DateTime.Now;
            var eventService = Context.EventService;
            var statusService = Context.BulbService;
            var request = new AccountCacheRequest()
            {
                AccountId = accountId,
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
                    var lastEvent = eventService.GetEventCacheOrNullById(accountId, data.LastEventId.Value);
                    if (lastEvent != null)
                    {
                        bool isActual = EventImportanceHelper.Get(data.Status) == lastEvent.Importance
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
                    accountId,
                    data.ComponentId.Value,
                    processDate);

                if (nextEvent != null)
                {
                    var noSignalImportance = Api.EventImportance.Unknown;
                    var signal = BulbSignal.Create(processDate, nextEvent, noSignalImportance, accountId);
                    statusService.SetSignal(data.Id, signal);
                    return;
                }

                // нет актуальных событий
                var unknownSignal = BulbSignal.CreateUnknown(accountId, processDate);
                statusService.SetSignal(eventsStatusId, unknownSignal);
            }
        }

        public void ProcessEvent(Guid accountId, Guid componentId, IEventCacheReadObject eventObj)
        {
            var categories = new[]
            {
                Api.EventCategory.ApplicationError,
                Api.EventCategory.ComponentEvent
            };

            if (categories.Contains(eventObj.Category) == false)
            {
                return;
            }

            var accountDbContext = Context.GetAccountDbContext(accountId);
            var component = accountDbContext.GetComponentRepository().GetById(componentId);

            // если выключен, то ничего не делаем
            if (component.CanProcess == false)
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
            var statusService = Context.BulbService;
            var noSignalImportance = Api.EventImportance.Unknown;

            // получаем или создаем статус (колбаску)
            var request = new AccountCacheRequest()
            {
                AccountId = accountId,
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
                        CalculateComponentEventsStatus(accountId, component.EventsStatusId);
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
                    CalculateComponentEventsStatus(accountId, component.EventsStatusId);
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

            var cache = new AccountCache(component.AccountId);
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
                if (recursive && childRead!=null)
                {
                    UpdateParentEnableFlags(childRead, recursive);
                }
            }
        }

        public void DisableComponent(Guid accountId, Guid componentId, DateTime? toDate, string comment)
        {
            if (comment != null && comment.Length > 1000)
            {
                throw new ResponseCodeException(
                    Zidium.Api.ResponseCode.ParameterError,
                    "Максимальная длина комментария 1000 символов, а указано " + comment.Length);
            }

            if (toDate.HasValue && toDate < DateTime.Now)
            {
                throw new ResponseCodeException(
                    Zidium.Api.ResponseCode.ParameterError,
                    "Дата, до которой выключается компонент, должна быть больше текущего времени");
            }

            var cache = new AccountCache(accountId);

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
            var accountId = component.AccountId;
            var cache = new AccountCache(accountId);
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
            if (component.Enable == false
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

            var statusService = Context.BulbService;
            var allStatusesIds = component.GetAllStatusesIds();
            var allStatuses = allStatusesIds.Select(cache.StatusDatas.Read).ToList();
            
            // если надо выключить
            if (component.CanProcess == false && allStatuses.Any(x => x.Status != Api.MonitoringStatus.Disabled))
            {
                // выключим все колбаски
                // выжно выключить их в этом порядке, чтобы не создать лишних событий колбасок-родителей
                var disableSignal = BulbSignal.CreateDisable(accountId, DateTime.Now);
                statusService.SetSignal(component.ExternalStatusId, disableSignal);
                statusService.SetSignal(component.ChildComponentsStatusId, disableSignal);
                statusService.SetSignal(component.InternalStatusId, disableSignal);
                statusService.SetSignal(component.EventsStatusId, disableSignal);
                statusService.SetSignal(component.UnitTestsStatusId, disableSignal);
                statusService.SetSignal(component.MetricsStatusId, disableSignal);
            }

            // обновим тесты
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
                Context.UnitTestService.GetUnitTestResult(component.AccountId, unitTestRef.Id);
            }

            // обновим метрики
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
                Context.MetricService.GetActualMetric(component.AccountId, component.Id, metricTypeId);
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
                GetComponentExternalState(component.AccountId, childRef.Id);
            }

            if (component.CanProcess)
            {
                // обновим колбасу тестов явно (вдруг нет ни одного теста)
                statusService.CalculateByChilds(accountId, component.UnitTestsStatusId, allUnitTestsStatusDataIds);

                // обновим колбасу метрик явно (вдруг нет ни одной метрики)
                statusService.CalculateByChilds(accountId, component.MetricsStatusId, allMetricsStatusDataIds);

                // обновим колбасу событий
                CalculateComponentEventsStatus(accountId, component.EventsStatusId);

                // обновим внутренний статус
                var childsForInternal = new List<Guid>()
                {
                    component.EventsStatusId,
                    component.UnitTestsStatusId,
                    component.MetricsStatusId
                };
                statusService.CalculateByChilds(accountId, component.InternalStatusId, childsForInternal);

                // обновим колбасу дочерних
                statusService.CalculateByChilds(accountId, component.ChildComponentsStatusId, childComponentsStatusDataIds);

                // обновим внешний статус
                var childsForExternal = new List<Guid>()
                {
                    component.InternalStatusId,
                    component.ChildComponentsStatusId
                };
                statusService.CalculateByChilds(accountId, component.ExternalStatusId, childsForExternal);
            }
            return result;
        }

        public void EnableComponent(Guid accountId, Guid componentId)
        {
            var cache = new AccountCache(accountId);
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

        public Component GetRootComponent(Guid accountId)
        {
            var accountDbContext = Context.GetAccountDbContext(accountId);
            var repository = accountDbContext.GetComponentRepository();
            var root = repository.GetRoot();
            if (root == null)
            {
                throw new Exception("Не удалось найти корневой компонент");
            }
            return root;
        }

        public IComponentCacheReadObject GetComponentById(Guid accountId, Guid id)
        {
            var cache = new AccountCache(accountId);
            var component = cache.Components.Read(id);

            if (component == null)
                throw new UnknownComponentIdException(id, accountId);

            return component;
        }

        public Component GetComponentByIdNoCache(Guid accountId, Guid id)
        {
            var accountDbContext = Context.GetAccountDbContext(accountId);
            var repository = accountDbContext.GetComponentRepository();
            var component = repository.GetByIdOrNull(id);

            if (component == null)
                throw new UnknownComponentIdException(id, accountId);

            return component;
        }

        public Component GetComponentBySystemName(Guid accountId, Guid parentId, string systemName)
        {
            var accountDbContext = Context.GetAccountDbContext(accountId);
            var repository = accountDbContext.GetComponentRepository();
            var parent = repository.GetById(parentId);
            var component = FindBySystemName(parent, systemName);

            if (component == null)
                throw new ParameterErrorException("Component not found");

            return component;
        }

        public Component CreateComponent(Guid accountId, CreateComponentRequestData data)
        {
            if (data == null)
            {
                throw new ParameterRequiredException("Request.Data");
            }
            var data2 = new GetOrCreateComponentRequestData()
            {
                DisplayName = data.DisplayName,
                SystemName = data.SystemName,
                TypeId = data.TypeId,
                ParentComponentId = data.ParentComponentId,
                Version = data.Version,
                Properties = data.Properties
            };
            return GetOrCreateComponentInternal(accountId, data2, true);
        }

        protected Component FindBySystemName(Component parent, string systemName)
        {
            var childs = parent.Childs.Where(x => x.IsDeleted == false).ToList();
            var component = childs.FirstOrDefault(x => string.Equals(x.SystemName, systemName, StringComparison.InvariantCultureIgnoreCase));
            if (component != null)
            {
                return component;
            }
            foreach (var child in childs)
            {
                if (child.IsFolder)
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

        public void CreateRoot(Guid accountId, Guid rootId)
        {
            var root = new Component()
            {
                Id = rootId,
                DisplayName = "Root",
                SystemName = "Root",
                ComponentTypeId = SystemComponentTypes.Root.Id,
                CreatedDate = DateTime.Now,
                Enable = true,
                ParentEnable = true
            };
            Add(root, accountId);
        }

        protected Component Add(Guid accountId, Guid componentId, Guid parentId, string displayName, string systemName, Guid componentTypeId)
        {
            var accountDbContext = Context.GetAccountDbContext(accountId);
            var repository = accountDbContext.GetComponentRepository();
            var parent = repository.GetById(parentId);
            var type = accountDbContext.ComponentTypes.Single(x => x.Id == componentTypeId);
            
            var component = new Component()
            {
                Id = componentId,
                Parent = parent,
                ParentId = parentId,
                SystemName = systemName,
                DisplayName = displayName,
                ComponentTypeId = componentTypeId,
                ComponentType = type,
                Enable = true
            };
            return Add(component, accountId);
        }

        protected Bulb CreateStatusData(Component component, Api.EventCategory eventCategory, Guid accountId)
        {
            return Context.BulbService.CreateBulb(
                accountId,
                component.CreatedDate,
                eventCategory,
                component.Id);
        }

        protected Component Add(Component component, Guid accountId)
        {
            if (component == null)
            {
                throw new ArgumentNullException("component");
            }
            component.CreatedDate = DateTime.Now;
            if (component.Parent != null)
            {
                if (component.ComponentTypeId == SystemComponentTypes.Root.Id)
                {
                    throw new ResponseCodeException(
                        ResponseCode.UnknownComponentTypeId,
                        "Нельзя создавать экземпляры компонентов с типом Root");
                }
                component.ParentId = component.Parent.Id;
                component.ParentEnable = component.Parent.CanProcess;
                var parentRequest = new AccountCacheRequest()
                {
                    AccountId = accountId,
                    ObjectId = component.Parent.Id
                };
                var parent = AllCaches.Components.Find(parentRequest);
                if (parent == null)
                {
                    throw new Exception("parent == null");
                }
                var child = parent.Childs.FindByName(component.SystemName);
                if (child != null)
                {
                    throw new Exception("У родителя уже есть компонент с таким SystemName");
                }
            }

            component.ExternalStatus = CreateStatusData(component, Api.EventCategory.ComponentExternalStatus, accountId);
            component.InternalStatus = CreateStatusData(component, Api.EventCategory.ComponentInternalStatus, accountId);
            component.EventsStatus = CreateStatusData(component, Api.EventCategory.ComponentEventsStatus, accountId);
            component.UnitTestsStatus = CreateStatusData(component, Api.EventCategory.ComponentUnitTestsStatus, accountId);
            component.MetricsStatus = CreateStatusData(component, Api.EventCategory.ComponentMetricsStatus, accountId);
            component.ChildComponentsStatus = CreateStatusData(component, Api.EventCategory.ComponentChildsStatus, accountId);

            // обновим ссылку на компонент
            var statuses = component.GetAllStatuses();
            foreach (var statusData in statuses)
            {
                statusData.ComponentId = component.Id;
                statusData.Component = component;
            }

            component.LogConfig = new LogConfig()
            {
                ComponentId = component.Id,
                Component = component,
                Enabled = true,
                IsFatalEnabled = true,
                IsErrorEnabled = true,
                IsWarningEnabled = true,
                IsInfoEnabled = true,
                IsDebugEnabled = true,
                IsTraceEnabled = true,
                LastUpdateDate = component.CreatedDate
            };
            var accountDbContext = Context.GetAccountDbContext(accountId);
            accountDbContext.Components.Add(component);
            accountDbContext.SaveChanges();

            // обновим в кэше ссылки на дочерние компоненты у родителя 
            if (component.Parent != null)
            {
                var parentRequest = new AccountCacheRequest()
                {
                    AccountId = accountId,
                    ObjectId = component.Parent.Id
                };
                using (var parent = AllCaches.Components.Write(parentRequest))
                {
                    var componentRef = new CacheObjectReference(component.Id, component.SystemName);
                    parent.WriteChilds.Add(componentRef);
                    parent.BeginSave();
                }
            }

            return component;
        }

        private Guid GetRootId(Guid accountId)
        {
            var account = Context.AccountService.GetOneById(accountId);
            return account.RootId;
        }

        public Component GetOrCreateComponentInternal(Guid accountId, GetOrCreateComponentRequestData data, bool createNew)
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
                data.ParentComponentId = GetRootId(accountId);
            }

            var componentId = data.NewId ?? Guid.NewGuid();
            var systemName = data.SystemName;

            var properties = ApiConverter.GetComponentProperties(data.Properties);

            var accountDbContext = Context.GetAccountDbContext(accountId);
            var componentRepository = accountDbContext.GetComponentRepository();
            var lockObj = LockObject.ForComponent(systemName);
            lock (lockObj)
            {
                // ищем в детях
                var component = componentRepository.GetChild(data.ParentComponentId.Value, systemName);
                bool isExists = true;
                if (component == null)
                {
                    // ищем в папках
                    var parent = componentRepository.GetById(data.ParentComponentId.Value);
                    component = FindBySystemName(parent, systemName);
                    if (component == null)
                    {
                        // Проверим лимит
                        var checker = AccountLimitsCheckerManager.GetCheckerForAccount(accountId);
                        var limitCheckResult = checker.CheckMaxComponents(accountDbContext);
                        if (!limitCheckResult.Success)
                            throw new OverLimitException(limitCheckResult.Message);

                        // создаем новый
                        component = Add(
                            accountId,
                            componentId,
                            data.ParentComponentId.Value,
                            data.DisplayName ?? systemName,
                            systemName,
                            data.TypeId.Value);

                        isExists = false;
                        checker.RefreshComponentsCount();
                    }
                }

                if (isExists && createNew)
                {
                    throw new ResponseCodeException(
                        ResponseCode.ParameterError,
                        "Компонент с таким системным именем уже существует");
                }

                // обновим версию
                if (string.IsNullOrEmpty(data.Version) == false)
                {
                    component.Version = data.Version;
                }

                // обновим свойства
                foreach (var property in properties)
                {
                    component.SetProperty(property);
                }
                accountDbContext.SaveChanges();
                return component;
            }
        }

        public Component GetOrCreateComponent(Guid accountId, GetOrCreateComponentRequestData data)
        {
            if (data == null)
            {
                throw new ParameterRequiredException("Request.Data");
            }
            return GetOrCreateComponentInternal(accountId, data, false);
        }

        protected void UpdateComponentProperies(Component component, List<Api.ExtentionPropertyDto> properties, Guid accountId)
        {
            var accountDbContext = Context.GetAccountDbContext(accountId);

            // удаляем
            var forDelete = component.Properties.Where(x => properties.All(y => y.Name != x.Name)).ToArray();
            foreach (var componentProperty in forDelete)
            {
                component.Properties.Remove(componentProperty);
                accountDbContext.ComponentProperties.Remove(componentProperty);
            }

            // обновляем или вставляем
            foreach (var property in properties)
            {
                var componentProperty = component.Properties.FirstOrDefault(x => x.Name == property.Name);
                if (componentProperty == null)
                {
                    // вставляем
                    componentProperty = ApiConverter.GetComponentProperty(property);
                    componentProperty.Component = component;
                    component.Properties.Add(componentProperty);
                }
                else
                {
                    // обновляем
                    componentProperty.DataType = property.Type;
                    componentProperty.Value = property.Value;
                }
            }
        }

        public Component UpdateComponent(Guid accountId, UpdateComponentRequestData data)
        {
            // проверим существование компонента
            var oldComponentRequest = new AccountCacheRequest()
            {
                AccountId = accountId,
                ObjectId = data.Id.Value
            };
            var oldComponent = AllCaches.Components.Find(oldComponentRequest);
            if (oldComponent == null)
            {
                throw new UnknownComponentIdException(data.Id.Value, accountId);
            }

            // проверим нового родителя
            if (data.ParentId.HasValue)
            {
                var parent = AllCaches.Components.Find(new AccountCacheRequest()
                {
                    AccountId = accountId,
                    ObjectId = data.ParentId.Value
                });
                if (parent == null)
                {
                    throw new UnknownComponentIdException(data.ParentId.Value, accountId);
                }
            }

            // проверим уникальность системного имени
            if (!string.IsNullOrEmpty(data.SystemName))
            {
                var parentId = data.ParentId ?? oldComponent.ParentId.Value;
                var parent = AllCaches.Components.Find(new AccountCacheRequest()
                {
                    AccountId = accountId,
                    ObjectId = parentId
                });
                if (parent == null)
                {
                    throw new UnknownComponentIdException(parentId, accountId);
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
                if (!Context.ComponentTypeService.Contains(accountId, data.TypeId.Value))
                {
                    throw new ParameterErrorException("TypeId");
                }
            }

            var request = new AccountCacheRequest()
            {
                AccountId = accountId,
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

            var accountDbContext = Context.GetAccountDbContext(accountId);
            var repository = accountDbContext.GetComponentRepository();
            var componentObj = repository.GetById(data.Id.Value);

            if (data.Properties != null && data.Properties.Count > 0)
            {
                UpdateComponentProperies(componentObj, data.Properties, accountId); //todo если передать пустой список, то все сотрется...
            }
            accountDbContext.SaveChanges();
            return componentObj;
        }

        public List<Component> GetChildComponents(Guid accountId, Guid parentComponentId)
        {
            var parent = GetComponentById(accountId, parentComponentId);
            var accountDbContext = Context.GetAccountDbContext(accountId);
            var repository = accountDbContext.GetComponentRepository();
            return repository.GetChildComponents(parent.Id);
        }

        public Component GetRootComponent(Guid accountId, Guid rootId)
        {
            var accountDbContext = Context.GetAccountDbContext(accountId);
            var repository = accountDbContext.GetComponentRepository();
            return repository.GetById(rootId);
        }

        public LogConfig GetLogConfig(Guid accountId, Guid componentId)
        {
            var accountDbContext = Context.GetAccountDbContext(accountId);
            //var lockObject = LockObject.ForComponent(componentId);
            //lock (lockObject)
            {
                var repository = accountDbContext.GetLogConfigRepository();
                var config = repository.GetByComponentId(componentId);
                if (config == null)
                {
                    config = new LogConfig()
                    {
                        Enabled = true,
                        ComponentId = componentId,
                        IsDebugEnabled = true,
                        IsTraceEnabled = true,
                        IsInfoEnabled = true,
                        IsWarningEnabled = true,
                        IsErrorEnabled = true,
                        IsFatalEnabled = true
                    };
                    repository.Add(config);
                }
                return config;
            }
        }

        public void DeleteComponent(Guid accountId, Guid componentId)
        {
            var cache = new AccountCache(accountId);
            IComponentCacheReadObject component = null;
            ComponentCacheWriteObject componentWrite = null;

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
            componentWrite.WaitSaveChanges(TimeSpan.FromSeconds(30));
            component = componentWrite;

            var checker = AccountLimitsCheckerManager.GetCheckerForAccount(accountId);
            checker.RefreshComponentsCount();

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
                DeleteComponent(accountId, child.Id);
            }

            // Если есть родитель, обновим его статус
            if (component.ParentId.HasValue)
                CalculateAllStatuses(accountId, component.ParentId.Value);
        }

        public int UpdateEventsStatuses(Guid accountId, int maxCount)
        {
            var accountDbContext = Context.GetAccountDbContext(accountId);
            var componentRepository = accountDbContext.GetComponentRepository();
            var statusIds = componentRepository
                .GetWhereNotActualEventsStatus()
                .OrderBy(x => x.EventsStatus.ActualDate)
                .Take(maxCount)
                .Select(t => t.EventsStatusId)
                .ToArray();
            foreach (var statusId in statusIds)
            {
                CalculateComponentEventsStatus(accountId, statusId);
            }
            return statusIds.Length;
        }

        public Guid[] GetComponentAndChildIds(Guid accountId, Guid componentId)
        {
            var accountDbContext = Context.GetAccountDbContext(accountId);
            var componentRepository = accountDbContext.GetComponentRepository();
            var allComponents = componentRepository.QueryAll()
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
