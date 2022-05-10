using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Zidium.Api;
using Zidium.Api.Dto;
using Zidium.Common;
using Zidium.Core.AccountsDb;
using Zidium.Core.Api;
using Zidium.Core.Caching;
using Zidium.Core.Common;
using Zidium.Core.Common.Helpers;
using Zidium.Core.InternalLogger;
using Zidium.Core.Limits;
using Zidium.Storage;

namespace Zidium.Core
{
    /// <summary>
    /// Обработка логики диспетчера
    /// </summary>
    public class DispatcherService : IDispatcherService
    {
        #region Служебные

        public IComponentControl Control { get; private set; }

        private static DispatcherWrapper _wrapper;

        private DispatcherService(IComponentControl control, ILogger logger, ITimeService timeService)
        {
            _accountStorageFactory = DependencyInjection.GetServicePersistent<IStorageFactory>();
            Control = control;
            StaticControl = control;
            _logger = logger;
            _timeService = timeService;
        }

        private readonly ILogger _logger;

        private readonly IStorageFactory _accountStorageFactory;

        public static IComponentControl StaticControl = new FakeComponentControl();

        private static readonly object _lockObject = new object();

        // Refactor to normal DI
        public static DispatcherWrapper Wrapper
        {
            get
            {
                if (_wrapper == null)
                {
                    lock (_lockObject)
                    {
                        if (_wrapper == null)
                        {
                            var realControl = GetRealControl();

                            var realLogger = DependencyInjection.GetLogger("Dispatcher");
                            var mapping = DependencyInjection.GetServicePersistent<InternalLoggerComponentMapping>();
                            mapping.MapLoggerToComponent("Dispatcher", realControl.Info.Id);

                            var timeService = DependencyInjection.GetServicePersistent<ITimeService>();
                            var realService = new DispatcherService(realControl, realLogger, timeService);
                            var wrapper = new DispatcherWrapper(realService, realControl, realLogger);
                            AllCaches.SetControl(realControl);
                            _wrapper = wrapper;
                        }
                    }
                }
                return _wrapper;
            }
        }

        public static string Version { get; set; }

        private static IComponentControl GetRealControl()
        {
            try
            {
                // делаем цепочку вызовов:
                // контрол => внутренний диспетчер => фейковый контрол
                var fakeControl = GetFakeControl();
                var timeService = DependencyInjection.GetServicePersistent<ITimeService>();
                IDispatcherService internalDispatcher = new DispatcherService(fakeControl, NullLogger.Instance, timeService); // обслуживает только запросы самого диспетчера (через контрол АПИ)
                internalDispatcher = new DispatcherWrapper(internalDispatcher, fakeControl, NullLogger.Instance);
                var apiService = new ApiService(internalDispatcher);
                var client = Client.Instance;
                var token = SystemAccountHelper.GetApiToken();
                client.AccessToken.SecretKey = token.SecretKey;
                client.SetApiService(apiService);

                // Создадим компонент
                var debugConfiguration = DependencyInjection.GetServicePersistent<IDebugConfiguration>();
                var folder = !debugConfiguration.DebugMode ? 
                    client.GetRootComponentControl().GetOrCreateChildFolderControl("Zidium") : 
                    client.GetRootComponentControl().GetOrCreateChildFolderControl("DEBUG");
                var componentType = client.GetOrCreateComponentTypeControl(!debugConfiguration.DebugMode ? "Dispatcher" : DebugHelper.DebugComponentType);
                var version = Version ?? typeof(DispatcherService).Assembly.GetName().Version.ToString();
                var componentControl = folder.GetOrCreateChildComponentControl(componentType, "Dispatcher", version);

                // Присвоим Id компонента по умолчанию, чтобы адаптер логирования мог его использовать
                client.Config.DefaultComponent.Id = componentControl.Info?.Id;

                return componentControl;
            }
            catch
            {
                return GetFakeControl();
            }
        }

        private static IComponentControl GetFakeControl()
        {
            return new FakeComponentControl("FakeDispatcher");
        }

        protected delegate T ExecuteMethodDelegate<T>();

        protected void CheckRequest(RequestDto request)
        {
            if (request == null)
            {
                throw new ParameterRequiredException("Request");
            }
            if (request.Token == null)
            {
                throw new ParameterRequiredException("Request.Token");
            }
            if (request.Token.SecretKey == null)
            {
                throw new ParameterRequiredException("Request.Token.SecretKey");
            }
        }

        // TODO Remove trivial method
        protected IStorage GetStorage()
        {
            return _accountStorageFactory.GetStorage();
        }

        protected void AuthRequest(RequestDto request)
        {
            if (request.Token.SecretKey != SecretKey)
            {
                throw new ResponseCodeException(ResponseCode.InvalidSecretKey, "Неверный SecretKey");
            }
        }

        private static string SecretKey
        {
            get
            {
                if (_secretKey == null)
                {
                    var accessConfiguration = DependencyInjection.GetServicePersistent<IAccessConfiguration>();
                    _secretKey = accessConfiguration.SecretKey;
                }

                return _secretKey;
            }
        }

        private static string _secretKey;
        private readonly ITimeService _timeService;

        #endregion

        #region Разное

        public GetEchoResponseDto GetEcho(GetEchoRequestDto request)
        {
            CheckRequest(request);
            if (request.Data == null)
            {
                throw new ParameterRequiredException("Request.Data");
            }

            AuthRequest(request); // чтобы убедиться, что AccountId и SecretKey верные

            return new GetEchoResponseDto()
            {
                Code = ResponseCode.Success,
                Data = request.Data.Message
            };
        }

        public GetServerTimeResponseDto GetServerTime()
        {
            return new GetServerTimeResponseDto()
            {
                Code = ResponseCode.Success,
                Data = new GetServerTimeResponseDataDto()
                {
                    Date = _timeService.Now()
                }
            };
        }

        public SaveAllCachesResponse SaveAllCaches(SaveAllCachesRequest request)
        {
            CheckRequest(request);
            AllCaches.SaveChanges();
            return new SaveAllCachesResponse()
            {
                Code = ResponseCode.Success
            };
        }

        public GetLogicSettingsResponse GetLogicSettings(GetLogicSettingsRequest request)
        {
            CheckRequest(request);
            AuthRequest(request);

            var logicSettingsService = new LogicSettingsService();

            return new GetLogicSettingsResponse()
            {
                Code = ResponseCode.Success,
                Data = new GetLogicSettingsResponseData()
                {
                    AccountWebSite = logicSettingsService.WebSite(),
                    EventsMaxDays = logicSettingsService.EventsMaxDays(),
                    UnitTestsMaxDays = logicSettingsService.UnitTestsMaxDays(),
                    MetricsMaxDays = logicSettingsService.MetricsMaxDays(),
                    LogMaxDays = logicSettingsService.LogMaxDays(),
                    MasterPassword = logicSettingsService.MasterPassword()
                }
            };
        }

        #endregion

        #region Компоненты и папки

        public GetRootControlDataResponseDto GetRootControlData(GetRootControlDataRequestDto request)
        {
            CheckRequest(request);

            AuthRequest(request);

            var storage = GetStorage();

            var root = storage.Components.GetRoot();
            var componentType = storage.ComponentTypes.GetOneById(root.ComponentTypeId);
            var properties = storage.ComponentProperties.GetByComponentId(root.Id);

            var logService = new LogService(storage, _timeService);
            var logConfig = logService.GetLogConfig(root.Id);

            return new GetRootControlDataResponseDto()
            {
                Code = ResponseCode.Success,
                Data = new ComponentControlDataDto()
                {
                    Component = ApiConverter.GetComponentInfo(root, properties, componentType),
                    WebLogConfig = ApiConverter.GetLogConfig(logConfig)
                }
            };
        }

        public CreateComponentResponse CreateComponent(CreateComponentRequest request)
        {
            CheckRequest(request);
            if (request.Data == null)
            {
                throw new ParameterRequiredException("Request.Data");
            }

            AuthRequest(request);

            // создаём компонент
            var storage = GetStorage();
            var componentService = new ComponentService(storage, _timeService);
            var component = componentService.CreateComponent(request.Data);
            var componentType = storage.ComponentTypes.GetOneById(component.ComponentTypeId);
            var properties = storage.ComponentProperties.GetByComponentId(component.Id);
            var componentInfo = ApiConverter.GetComponentInfo(component, properties, componentType);

            // получаем конфиг веб-лога
            var logService = new LogService(storage, _timeService);
            var logConfig = logService.GetLogConfig(component.Id);
            var logConfigDto = ApiConverter.GetLogConfig(logConfig);

            return new CreateComponentResponse()
            {
                Code = ResponseCode.Success,
                Data = new ComponentControlDataDto()
                {
                    Component = componentInfo,
                    WebLogConfig = logConfigDto
                }
            };

        }

        public GetOrCreateComponentResponseDto GetOrCreateComponent(GetOrCreateComponentRequestDto request)
        {
            CheckRequest(request);
            if (request.Data == null)
            {
                throw new ParameterRequiredException("Request.Data");
            }

            AuthRequest(request);

            // получаем или создаём компонент
            var storage = GetStorage();
            var componentService = new ComponentService(storage, _timeService);
            var component = componentService.GetOrCreateComponent(request.Data);
            var componentType = storage.ComponentTypes.GetOneById(component.ComponentTypeId);
            var properties = storage.ComponentProperties.GetByComponentId(component.Id);
            var componentInfo = ApiConverter.GetComponentInfo(component, properties, componentType);

            // получаем конфиг веб-лога
            var logService = new LogService(storage, _timeService);
            var logConfig = logService.GetLogConfig(component.Id);
            var logConfigDto = ApiConverter.GetLogConfig(logConfig);

            return new GetOrCreateComponentResponseDto()
            {
                Code = ResponseCode.Success,
                Data = new ComponentControlDataDto()
                {
                    Component = componentInfo,
                    WebLogConfig = logConfigDto
                }
            };
        }

        public GetComponentControlByIdResponseDto GetComponentControlById(GetComponentControlByIdRequestDto request)
        {
            CheckRequest(request);
            if (request.Data == null)
            {
                throw new ParameterRequiredException("Request.Data");
            }
            if (request.Data.ComponentId == null)
            {
                throw new ParameterRequiredException("Request.Data.ComponentId");
            }

            AuthRequest(request);

            var componentId = request.Data.ComponentId.Value;

            // получаем или создаём компонент
            var storage = GetStorage();
            var componentService = new ComponentService(storage, _timeService);

            var component = componentService.GetComponentByIdNoCache(componentId);
            if (component.IsDeleted)
            {
                throw new UnknownComponentIdException(componentId);
            }

            var componentType = storage.ComponentTypes.GetOneById(component.ComponentTypeId);
            var properties = storage.ComponentProperties.GetByComponentId(component.Id);
            var componentInfo = ApiConverter.GetComponentInfo(component, properties, componentType);

            // получаем конфиг веб-лога
            var logService = new LogService(storage, _timeService);
            var logConfig = logService.GetLogConfig(component.Id);
            var logConfigDto = ApiConverter.GetLogConfig(logConfig);

            return new GetComponentControlByIdResponseDto()
            {
                Code = ResponseCode.Success,
                Data = new ComponentControlDataDto()
                {
                    Component = componentInfo,
                    WebLogConfig = logConfigDto
                }
            };
        }

        public GetOrAddComponentResponseDto GetOrAddComponent(GetOrAddComponentRequestDto request)
        {
            CheckRequest(request);
            if (request.Data == null)
            {
                throw new ParameterRequiredException("Request.Data");
            }

            AuthRequest(request);

            // получаем или создаём компонент
            var storage = GetStorage();
            var componentService = new ComponentService(storage, _timeService);
            var component = componentService.GetOrCreateComponent(request.Data);
            var componentType = storage.ComponentTypes.GetOneById(component.ComponentTypeId);
            var properties = storage.ComponentProperties.GetByComponentId(component.Id);
            var componentInfo = ApiConverter.GetComponentInfo(component, properties, componentType);

            return new GetOrAddComponentResponseDto()
            {
                Code = ResponseCode.Success,
                Data = componentInfo
            };

        }

        public GetRootComponentResponseDto GetRootComponent(GetRootComponentRequestDto request)
        {
            CheckRequest(request);

            AuthRequest(request);

            var storage = GetStorage();
            var root = storage.Components.GetRoot();
            var componentType = storage.ComponentTypes.GetOneById(root.ComponentTypeId);
            var properties = storage.ComponentProperties.GetByComponentId(root.Id);
            var componentInfo = ApiConverter.GetComponentInfo(root, properties, componentType);

            return new GetRootComponentResponseDto()
            {
                Code = ResponseCode.Success,
                Data = componentInfo
            };

        }

        public GetComponentByIdResponseDto GetComponentById(GetComponentByIdRequestDto request)
        {
            CheckRequest(request);
            if (request.Data == null)
            {
                throw new ParameterRequiredException("Request.Data");
            }
            if (request.Data.ComponentId == null)
            {
                throw new ParameterRequiredException("Request.Data.ComponentId");
            }

            AuthRequest(request);

            var componentId = request.Data.ComponentId.Value;

            var storage = GetStorage();
            var componentService = new ComponentService(storage, _timeService);
            var component = componentService.GetComponentByIdNoCache(componentId);
            var componentType = storage.ComponentTypes.GetOneById(component.ComponentTypeId);
            var properties = storage.ComponentProperties.GetByComponentId(component.Id);
            var componentInfo = ApiConverter.GetComponentInfo(component, properties, componentType);

            return new GetComponentByIdResponseDto()
            {
                Code = ResponseCode.Success,
                Data = componentInfo
            };

        }

        public GetComponentBySystemNameResponseDto GetComponentBySystemName(GetComponentBySystemNameRequestDto request)
        {
            CheckRequest(request);
            if (request.Data == null)
            {
                throw new ParameterRequiredException("Request.Data");
            }
            if (request.Data.ParentId == null)
            {
                throw new ParameterRequiredException("Request.Data.ParentId");
            }
            if (request.Data.SystemName == null)
            {
                throw new ParameterRequiredException("Request.Data.SystemName");
            }

            AuthRequest(request);

            var storage = GetStorage();
            var componentService = new ComponentService(storage, _timeService);
            var component = componentService.GetComponentBySystemName(request.Data.ParentId.Value, request.Data.SystemName);
            var componentType = storage.ComponentTypes.GetOneById(component.ComponentTypeId);
            var properties = storage.ComponentProperties.GetByComponentId(component.Id);
            var componentInfo = ApiConverter.GetComponentInfo(component, properties, componentType);

            return new GetComponentBySystemNameResponseDto()
            {
                Code = ResponseCode.Success,
                Data = componentInfo
            };
        }

        public UpdateComponentResponseDto UpdateComponent(UpdateComponentRequestDto request)
        {
            CheckRequest(request);
            if (request.Data == null)
            {
                throw new ParameterRequiredException("Request.Data");
            }
            if (request.Data.Id == null)
            {
                throw new ParameterRequiredException("Request.Data.Id");
            }

            AuthRequest(request);

            var storage = GetStorage();
            var componentService = new ComponentService(storage, _timeService);
            componentService.UpdateComponent(request.Data);
            var component = storage.Components.GetOneById(request.Data.Id.Value);
            var componentType = storage.ComponentTypes.GetOneById(component.ComponentTypeId);
            var properties = storage.ComponentProperties.GetByComponentId(component.Id);
            var componentInfo = ApiConverter.GetComponentInfo(component, properties, componentType);

            return new UpdateComponentResponseDto()
            {
                Code = ResponseCode.Success,
                Data = componentInfo
            };
        }

        public GetChildComponentsResponseDto GetChildComponents(GetChildComponentsRequestDto request)
        {
            CheckRequest(request);
            if (request.Data == null)
            {
                throw new ParameterRequiredException("Request.Data");
            }
            if (request.Data.ComponentId == null)
            {
                throw new ParameterRequiredException("Request.Data.ComponentId");
            }

            AuthRequest(request);

            var componentId = request.Data.ComponentId.Value;

            var storage = GetStorage();
            var componentService = new ComponentService(storage, _timeService);
            var components = componentService.GetChildComponents(componentId);
            var componentInfos = components.Select(component =>
            {
                var componentType = storage.ComponentTypes.GetOneById(component.ComponentTypeId);
                var properties = storage.ComponentProperties.GetByComponentId(component.Id);
                return ApiConverter.GetComponentInfo(component, properties, componentType);
            }).ToArray();

            return new GetChildComponentsResponseDto()
            {
                Code = ResponseCode.Success,
                Data = componentInfos
            };
        }

        public SetComponentEnableResponseDto SetComponentEnable(SetComponentEnableRequestDto request)
        {
            CheckRequest(request);
            if (request.Data == null)
            {
                throw new ParameterRequiredException("Request.Data");
            }
            if (request.Data.ComponentId == null)
            {
                throw new ParameterRequiredException("Request.Data.ComponentId");
            }

            AuthRequest(request);

            var storage = GetStorage();
            var componentService = new ComponentService(storage, _timeService);
            componentService.EnableComponent(request.Data.ComponentId.Value);

            return new SetComponentEnableResponseDto()
            {
                Code = ResponseCode.Success
            };
        }

        public SetComponentDisableResponseDto SetComponentDisable(SetComponentDisableRequestDto request)
        {
            CheckRequest(request);
            if (request.Data == null)
            {
                throw new ParameterRequiredException("Request.Data");
            }
            if (request.Data.ComponentId == null)
            {
                throw new ParameterRequiredException("Request.Data.ComponentId");
            }

            AuthRequest(request);

            var storage = GetStorage();
            var componentService = new ComponentService(storage, _timeService);
            componentService.DisableComponent(
                    request.Data.ComponentId.Value,
                    request.Data.ToDate,
                    request.Data.Comment);

            return new SetComponentDisableResponseDto()
            {
                Code = ResponseCode.Success
            };
        }

        public DeleteComponentResponseDto DeleteComponent(DeleteComponentRequestDto request)
        {
            CheckRequest(request);
            if (request.Data == null)
            {
                throw new ParameterRequiredException("Request.Data");
            }
            if (request.Data.ComponentId == null)
            {
                throw new ParameterRequiredException("Request.Data.ComponentId");
            }

            AuthRequest(request);

            var componentId = request.Data.ComponentId.Value;

            var storage = GetStorage();
            var componentService = new ComponentService(storage, _timeService);
            componentService.DeleteComponent(componentId);

            return new DeleteComponentResponseDto()
            {
                Code = ResponseCode.Success
            };
        }

        public GetComponentAndChildIdsResponse GetComponentAndChildIds(GetComponentAndChildIdsRequest request)
        {
            CheckRequest(request);

            if (request.Data == null)
            {
                throw new ParameterRequiredException("Request.Data");
            }
            if (request.Data.ComponentId == null)
            {
                throw new ParameterRequiredException("Request.Data.ComponentId");
            }

            AuthRequest(request);

            var componentId = request.Data.ComponentId.Value;

            var storage = GetStorage();
            var componentService = new ComponentService(storage, _timeService);
            var result = componentService.GetComponentAndChildIds(componentId);

            return new GetComponentAndChildIdsResponse()
            {
                Code = ResponseCode.Success,
                Data = result
            };
        }

        public UpdateEventsStatusesResponse UpdateEventsStatuses(UpdateEventsStatusesRequest request)
        {
            CheckRequest(request);

            AuthRequest(request);

            var storage = GetStorage();
            var componentService = new ComponentService(storage, _timeService);
            var updated = componentService.UpdateEventsStatuses(request.Data.MaxCount);

            return new UpdateEventsStatusesResponse()
            {
                Code = ResponseCode.Success,
                Data = new UpdateEventsStatusesResponseData()
                {
                    UpdateCount = updated
                }
            };
        }

        #endregion

        #region События

        public SendEventResponseDto SendEvent(SendEventRequestDto request)
        {
            CheckRequest(request);
            if (request.Data == null)
            {
                throw new ParameterRequiredException("Request.Data");
            }

            AuthRequest(request);

            try
            {
                var componentId = request.Data.ComponentId.Value;

                var storage = GetStorage();
                var service = new EventService(storage, _timeService);

                var eventData = service.SendEvent(
                    componentId,
                    request.Data);

                return new SendEventResponseDto()
                {
                    Code = ResponseCode.Success,
                    Data = new SendEventResponseDataDto()
                    {
                        EventId = eventData.Id,
                        EventTypeId = eventData.EventTypeId
                    }
                };
            }
            catch (ResponseCodeException responseException)
            {
                return new SendEventResponseDto()
                {
                    Code = responseException.Code,
                    ErrorMessage = responseException.Message
                };
            }
        }

        public JoinEventsResponseDto JoinEvents(JoinEventsRequestDto request)
        {
            CheckRequest(request);
            if (request.Data == null)
            {
                throw new ParameterRequiredException("Request.Data");
            }

            AuthRequest(request);

            try
            {
                var storage = GetStorage();
                var service = new EventService(storage, _timeService);

                foreach (var joinData in request.Data)
                {
                    service.JoinEvent(joinData.ComponentId.Value, joinData);
                }
            }
            catch (Exception)
            {
                // TODO Process exception
            }
            return new JoinEventsResponseDto()
            {
                Code = ResponseCode.Success
            };
        }

        public UpdateEventTypeResponse UpdateEventType(UpdateEventTypeRequest request)
        {
            CheckRequest(request);
            if (request.Data == null)
            {
                throw new ParameterRequiredException("Request.Data");
            }

            AuthRequest(request);

            var storage = GetStorage();
            var service = new EventTypeService(storage, _timeService);
            service.Update(request.Data);

            return new UpdateEventTypeResponse()
            {
                Code = ResponseCode.Success,
            };
        }

        public GetEventByIdResponseDto GetEventById(GetEventByIdRequestDto request)
        {
            CheckRequest(request);
            if (request.Data == null)
            {
                throw new ParameterRequiredException("Request.Data");
            }
            if (request.Data.EventId == null)
            {
                throw new ParameterRequiredException("Request.Data.EventId");
            }

            AuthRequest(request);

            var storage = GetStorage();
            var eventData = storage.Events.GetOneById(request.Data.EventId.Value);
            var type = storage.EventTypes.GetOneById(eventData.EventTypeId);
            var properties = storage.EventProperties.GetByEventId(eventData.Id);
            var eventInfo = ApiConverter.GetEventInfo(eventData, type, properties);

            return new GetEventByIdResponseDto()
            {
                Code = ResponseCode.Success,
                Data = eventInfo
            };
        }

        public GetEventsResponseDto GetEvents(GetEventsRequestDto request)
        {
            CheckRequest(request);
            if (request.Data == null)
            {
                throw new ParameterRequiredException("Request.Data");
            }
            if (request.Data.OwnerId == null)
            {
                throw new ParameterRequiredException("Request.Data.OwnerId");
            }

            AuthRequest(request);

            var storage = GetStorage();
            var service = new EventService(storage, _timeService);
            var events = service.GetEvents(request.Data);

            var eventInfos = events.Select(eventObj =>
            {
                var eventType = storage.EventTypes.GetOneById(eventObj.EventTypeId);
                var properties = storage.EventProperties.GetByEventId(eventObj.Id);
                var eventInfo = ApiConverter.GetEventInfo(eventObj, eventType, properties);
                return eventInfo;
            }).ToList();

            return new GetEventsResponseDto()
            {
                Code = ResponseCode.Success,
                Data = eventInfos
            };
        }

        #endregion

        #region Метрики

        public CreateMetricResponse CreateMetric(CreateMetricRequest request)
        {
            CheckRequest(request);
            if (request.Data == null)
            {
                throw new ParameterRequiredException("Request.Data");
            }

            AuthRequest(request);

            var storage = GetStorage();
            var service = new MetricService(storage, _timeService);
            var metric = service.CreateMetric(request.Data);
            var responseData = new CreateMetricResponseData()
            {
                MetricId = metric.Id,
                MetricTypeId = metric.MetricTypeId
            };

            return new CreateMetricResponse()
            {
                Code = ResponseCode.Success,
                Data = responseData
            };
        }

        public GetMetricsResponseDto GetMetrics(GetMetricsRequestDto request)
        {
            CheckRequest(request);
            if (request.Data == null)
            {
                throw new ParameterRequiredException("Request.Data");
            }
            if (request.Data.ComponentId == null)
            {
                throw new ParameterRequiredException("Request.Data.ComponentId");
            }

            AuthRequest(request);

            var componentId = request.Data.ComponentId.Value;

            var storage = GetStorage();
            var service = new MetricService(storage, _timeService);
            var metrics = service.GetMetrics(componentId);
            var metricInfos = new List<MetricDto>(metrics.Count);
            foreach (var metric in metrics)
            {
                var metricType = AllCaches.MetricTypes.Find(new AccountCacheRequest()
                {
                    ObjectId = metric.MetricTypeId
                });
                if (metricType == null)
                {
                    throw new Exception("metricType == null");
                }
                var statusData = AllCaches.StatusDatas.Find(new AccountCacheRequest()
                {
                    ObjectId = metric.StatusDataId
                });
                if (statusData == null)
                {
                    throw new Exception("statusData == null");
                }
                var metricInfo = ApiConverter.GetMetricInfo(metric, metricType, statusData);
                metricInfos.Add(metricInfo);
            }

            var response = new GetMetricsResponseDto
            {
                Code = ResponseCode.Success,
                Data = metricInfos
            };
            return response;
        }

        public GetMetricsHistoryResponseDto GetMetricsHistory(GetMetricsHistoryRequestDto request)
        {
            CheckRequest(request);
            if (request.Data == null)
            {
                throw new ParameterRequiredException("Request.Data");
            }
            if (request.Data.ComponentId == null)
            {
                throw new ParameterRequiredException("Request.Data.ComponentId");
            }

            AuthRequest(request);

            var storage = GetStorage();
            var service = new MetricService(storage, _timeService);
            var rows = service.GetMetricsHistory(request.Data);

            // конвертируем историю в MetricInfo
            var metricInfos = rows.Select(row =>
            {
                var type = storage.MetricTypes.GetOneById(row.MetricTypeId);
                var info = new MetricDto()
                {
                    ActualDate = row.ActualDate,
                    BeginDate = row.BeginDate,
                    ComponentId = row.ComponentId,
                    Name = type.SystemName,
                    Status = MonitoringStatusHelper.Get(row.Color),
                    Value = row.Value
                };
                return info;
            }).ToList();

            return new GetMetricsHistoryResponseDto()
            {
                Code = ResponseCode.Success,
                Data = metricInfos
            };
        }

        public DeleteMetricTypeResponse DeleteMetricType(DeleteMetricTypeRequest typeRequest)
        {
            CheckRequest(typeRequest);
            if (typeRequest.Data == null)
            {
                throw new ParameterRequiredException("Request.Data");
            }
            if (typeRequest.Data.MetricTypeId == null)
            {
                throw new ParameterRequiredException("Request.Data.MetricTypeId");
            }

            AuthRequest(typeRequest);

            var storage = GetStorage();
            var service = new MetricService(storage, _timeService);
            service.DeleteMetricType(typeRequest.Data.MetricTypeId);

            return new DeleteMetricTypeResponse()
            {
                Code = ResponseCode.Success
            };
        }

        public SendMetricsResponseDto SendMetrics(SendMetricsRequestDto request)
        {
            CheckRequest(request);
            if (request.Data == null)
            {
                throw new ParameterRequiredException("Request.Data");
            }

            AuthRequest(request);

            var storage = GetStorage();
            var service = new MetricService(storage, _timeService);
            service.SaveMetrics(request.Data);
            return new SendMetricsResponseDto()
            {
                Code = ResponseCode.Success
            };
        }

        public UpdateMetricsResponse UpdateMetrics(UpdateMetricsRequest request)
        {
            CheckRequest(request);

            AuthRequest(request);

            var storage = GetStorage();
            var service = new MetricService(storage, _timeService);
            var updated = service.UpdateMetrics(request.Data.MaxCount);

            return new UpdateMetricsResponse()
            {
                Code = ResponseCode.Success,
                Data = new UpdateMetricsResponseData()
                {
                    UpdateCount = updated
                }
            };
        }

        public SetMetricEnableResponse SetMetricEnable(SetMetricEnableRequest request)
        {
            CheckRequest(request);
            if (request.Data == null)
            {
                throw new ParameterRequiredException("Request.Data");
            }

            AuthRequest(request);

            var storage = GetStorage();
            var service = new MetricService(storage, _timeService);
            service.EnableMetric(request.Data.MetricId);

            return new SetMetricEnableResponse()
            {
                Code = ResponseCode.Success
            };
        }

        public SetMetricDisableResponse SetMetricDisable(SetMetricDisableRequest request)
        {
            CheckRequest(request);
            if (request.Data == null)
            {
                throw new ParameterRequiredException("Request.Data");
            }

            AuthRequest(request);

            var storage = GetStorage();
            var service = new MetricService(storage, _timeService);
            service.DisableMetric(request.Data);

            return new SetMetricDisableResponse()
            {
                Code = ResponseCode.Success
            };
        }

        public UpdateMetricResponse UpdateMetric(UpdateMetricRequest request)
        {
            CheckRequest(request);
            if (request.Data == null)
            {
                throw new ParameterRequiredException("Request.Data");
            }

            AuthRequest(request);

            var storage = GetStorage();
            var service = new MetricService(storage, _timeService);
            service.UpdateMetric(request.Data);

            return new UpdateMetricResponse()
            {
                Code = ResponseCode.Success
            };
        }

        public UpdateMetricTypeResponse UpdateMetricType(UpdateMetricTypeRequest request)
        {
            CheckRequest(request);
            if (request.Data == null)
            {
                throw new ParameterRequiredException("Request.Data");
            }

            AuthRequest(request);

            var storage = GetStorage();
            var service = new MetricService(storage, _timeService);
            service.UpdateMetricType(request.Data);

            return new UpdateMetricTypeResponse()
            {
                Code = ResponseCode.Success
            };
        }

        public CreateMetricTypeResponse CreateMetricType(CreateMetricTypeRequest request)
        {
            CheckRequest(request);
            if (request.Data == null)
            {
                throw new ParameterRequiredException("Request.Data");
            }

            AuthRequest(request);

            var storage = GetStorage();
            var service = new MetricService(storage, _timeService);
            var metricType = service.CreateMetricType(request.Data);
            var responseData = new CreateMetricTypeResponseData()
            {
                MetricTypeId = metricType.Id
            };

            return new CreateMetricTypeResponse()
            {
                Code = ResponseCode.Success,
                Data = responseData
            };
        }

        public SendMetricResponseDto SendMetric(SendMetricRequestDto request)
        {
            CheckRequest(request);
            if (request.Data == null)
            {
                throw new ParameterRequiredException("Request.Data");
            }
            if (request.Data.ComponentId == null)
            {
                throw new ParameterRequiredException("Request.Data.ComponentId");
            }

            AuthRequest(request);

            var storage = GetStorage();
            var service = new MetricService(storage, _timeService);
            var metric = service.SaveMetric(request.Data);

            var metricType = AllCaches.MetricTypes.Find(
                new AccountCacheRequest()
                {
                    ObjectId = metric.MetricTypeId
                });

            var status = AllCaches.StatusDatas.Find(
                new AccountCacheRequest()
                {
                    ObjectId = metric.StatusDataId
                });

            return new SendMetricResponseDto()
            {
                Code = ResponseCode.Success,
                Data = ApiConverter.GetMetricInfo(metric, metricType, status)
            };
        }

        public GetMetricResponseDto GetMetric(GetMetricRequestDto request)
        {
            CheckRequest(request);
            if (request.Data == null)
            {
                throw new ParameterRequiredException("Request.Data");
            }
            if (request.Data.ComponentId == null)
            {
                throw new ParameterRequiredException("Request.Data.ComponentId");
            }
            if (string.IsNullOrEmpty(request.Data.Name))
            {
                throw new ParameterRequiredException("Request.Data.Name");
            }

            AuthRequest(request);

            var storage = GetStorage();
            var service = new MetricService(storage, _timeService);
            var metric = service.GetActualMetric(request.Data.ComponentId.Value, request.Data.Name);
            var statusData = AllCaches.StatusDatas.Find(new AccountCacheRequest()
            {
                ObjectId = metric.StatusDataId
            });
            var metricType = AllCaches.MetricTypes.Find(new AccountCacheRequest()
            {
                ObjectId = metric.MetricTypeId
            });

            return new GetMetricResponseDto()
            {
                Code = ResponseCode.Success,
                Data = ApiConverter.GetMetricInfo(metric, metricType, statusData)
            };
        }

        public DeleteMetricResponse DeleteMetric(DeleteMetricRequest request)
        {
            CheckRequest(request);

            AuthRequest(request);

            var storage = GetStorage();
            var service = new MetricService(storage, _timeService);
            service.DeleteMetric(request.Data);

            var checker = AccountLimitsCheckerManager.GetChecker();

            return new DeleteMetricResponse()
            {
                Code = ResponseCode.Success
            };
        }

        #endregion

        #region Лог

        /// <summary>
        /// Запись лога для указанного компонента 
        /// </summary>
        /// <returns></returns>
        public SendLogResponseDto SendLog(SendLogRequestDto request)
        {
            CheckRequest(request);
            if (request.Data == null)
            {
                throw new ParameterRequiredException("Request.Data");
            }
            if (request.Data.ComponentId == null)
            {
                throw new ParameterRequiredException("Request.Data.ComponentId");
            }

            AuthRequest(request);

            var componentId = request.Data.ComponentId.Value;

            var storage = GetStorage();
            var service = new LogService(storage, _timeService);
            service.SaveLogMessage(componentId, request.Data);

            return new SendLogResponseDto()
            {
                Code = ResponseCode.Success
            };
        }

        public GetLogsResponseDto GetLogs(GetLogsRequestDto request)
        {
            CheckRequest(request);
            if (request.Data == null)
            {
                throw new ParameterRequiredException("Request.Data");
            }
            if (request.Data.ComponentId == null)
            {
                throw new ParameterRequiredException("Request.Data.ComponentId");
            }

            AuthRequest(request);

            var componentId = request.Data.ComponentId.Value;

            var storage = GetStorage();
            var service = new LogService(storage, _timeService);
            var rows = service.GetLogs(componentId, request.Data);
            var rowsDto = rows.Select(log =>
            {
                var properties = storage.LogProperties.GetByLogId(log.Id);
                return ApiConverter.GetLogRow(log, properties);
            }).ToList();

            return new GetLogsResponseDto()
            {
                Code = ResponseCode.Success,
                Data = rowsDto
            };
        }

        public GetLogConfigResponseDto GetLogConfig(GetLogConfigRequestDto request)
        {
            CheckRequest(request);
            if (request.Data == null)
            {
                throw new ParameterRequiredException("Request.Data");
            }
            if (request.Data.ComponentId == null)
            {
                throw new ParameterRequiredException("Request.Data.ComponentId");
            }

            AuthRequest(request);

            var componentId = request.Data.ComponentId.Value;

            var storage = GetStorage();
            var service = new LogService(storage, _timeService);
            var logConfig = service.GetLogConfig(componentId);
            var logConfigDto = ApiConverter.GetLogConfig(logConfig);

            return new GetLogConfigResponseDto()
            {
                Code = ResponseCode.Success,
                Data = logConfigDto
            };
        }

        public SendLogsResponseDto SendLogs(SendLogsRequestDto request)
        {
            CheckRequest(request);
            if (request.Data == null)
            {
                throw new ParameterRequiredException("Request.Data");
            }

            AuthRequest(request);

            var storage = GetStorage();
            var service = new LogService(storage, _timeService);

            service.SaveLogMessages(request.Data);

            return new SendLogsResponseDto()
            {
                Code = ResponseCode.Success
            };
        }

        public GetChangedWebLogConfigsResponseDto GetChangedWebLogConfigs(GetChangedWebLogConfigsRequestDto request)
        {
            CheckRequest(request);
            if (request.Data == null)
            {
                throw new ParameterRequiredException("Request.Data");
            }
            if (request.Data.LastUpdateDate == null)
            {
                throw new ParameterRequiredException("Request.Data.LastUpdateDate");
            }

            AuthRequest(request);

            var storage = GetStorage();
            var service = new LogService(storage, _timeService);

            var configs = service.GetChangedConfigs(
                request.Data.LastUpdateDate.Value,
                request.Data.ComponentIds);

            var response = new GetChangedWebLogConfigsResponseDto()
            {
                Code = ResponseCode.Success,
                Data = configs.Select(ApiConverter.GetLogConfig).ToList()
            };

            return response;
        }

        #endregion

        #region Типы компонентов

        public GetComponentTypeResponseDto GetComponentType(GetComponentTypeRequestDto request)
        {
            CheckRequest(request);
            if (request.Data == null)
            {
                throw new ParameterRequiredException("Request.Data");
            }
            if (request.Data.SystemName == null && request.Data.Id == null)
            {
                throw new ParameterRequiredException("Request.Data.SystemName or Request.Data.Id");
            }

            AuthRequest(request);

            var storage = GetStorage();

            ComponentTypeForRead type;
            if (request.Data.SystemName != null)
            {
                type = storage.ComponentTypes.GetOneOrNullBySystemName(request.Data.SystemName);
            }
            else
            {
                type = storage.ComponentTypes.GetOneById(request.Data.Id.Value);
            }
            var typeDto = ApiConverter.GetComponentTypeInfo(type);

            return new GetComponentTypeResponseDto()
            {
                Code = ResponseCode.Success,
                Data = typeDto
            };
        }

        public GetOrCreateComponentTypeResponseDto GetOrCreateComponentType(GetOrCreateComponentTypeRequestDto request)
        {
            CheckRequest(request);
            if (request.Data == null)
            {
                throw new ParameterRequiredException("Request.Data");
            }

            AuthRequest(request);

            var storage = GetStorage();
            var service = new ComponentTypeService(storage, _timeService);

            var type = service.GetOrCreateComponentType(request.Data);
            var typeDto = ApiConverter.GetComponentTypeInfo(type);

            return new GetOrCreateComponentTypeResponseDto()
            {
                Code = ResponseCode.Success,
                Data = typeDto
            };
        }

        public UpdateComponentTypeResponseDto UpdateComponentType(UpdateComponentTypeRequestDto request)
        {
            CheckRequest(request);
            if (request.Data == null)
            {
                throw new ParameterRequiredException("Request.Data");
            }
            if (request.Data.Id == null)
            {
                throw new ParameterRequiredException("Request.Data.ComponentTypeId");
            }

            AuthRequest(request);

            var storage = GetStorage();
            var service = new ComponentTypeService(storage, _timeService);

            service.UpdateComponentType(request.Data);
            var type = storage.ComponentTypes.GetOneById(request.Data.Id.Value);

            var typeDto = ApiConverter.GetComponentTypeInfo(type);
            return new UpdateComponentTypeResponseDto()
            {
                Code = ResponseCode.Success,
                Data = typeDto
            };
        }

        public DeleteComponentTypeResponse DeleteComponentType(DeleteComponentTypeRequest request)
        {
            CheckRequest(request);
            if (request.Data == null)
            {
                throw new ParameterRequiredException("Request.Data");
            }
            if (request.Data.ComponentTypeId == null)
            {
                throw new ParameterRequiredException("Request.Data.ComponentTypeId");
            }

            AuthRequest(request);

            var storage = GetStorage();
            var service = new ComponentTypeService(storage, _timeService);
            service.Delete(request.Data.ComponentTypeId.Value);

            return new DeleteComponentTypeResponse()
            {
                Code = ResponseCode.Success
            };
        }

        #endregion

        #region Проверки

        public SetUnitTestNextTimeResponse SetUnitTestNextTime(SetUnitTestNextTimeRequest request)
        {
            CheckRequest(request);

            if (request.Data == null)
            {
                throw new ParameterRequiredException("Request.Data");
            }
            if (request.Data.UnitTestId == null)
            {
                throw new ParameterRequiredException("Request.Data.UnitTestId");
            }

            AuthRequest(request);

            var storage = GetStorage();
            var service = new UnitTestService(storage, _timeService);
            service.SetUnitTestNextTime(request.Data);

            return new SetUnitTestNextTimeResponse()
            {
                Code = ResponseCode.Success
            };
        }

        public SetUnitTestNextStepProcessTimeResponse SetUnitTestNextStepProcessTime(SetUnitTestNextStepProcessTimeRequest request)
        {
            CheckRequest(request);

            if (request.Data == null)
            {
                throw new ParameterRequiredException("Request.Data");
            }
            if (request.Data.UnitTestId == null)
            {
                throw new ParameterRequiredException("Request.Data.UnitTestId");
            }
            if (request.Data.NextStepProcessTime == null)
            {
                throw new ParameterRequiredException("Request.Data.NextStepProcessTime");
            }

            AuthRequest(request);

            var storage = GetStorage();
            var service = new UnitTestService(storage, _timeService);
            service.SetUnitTestNextStepProcessTime(request.Data);

            return new SetUnitTestNextStepProcessTimeResponse()
            {
                Code = ResponseCode.Success
            };
        }

        public GetOrCreateUnitTestTypeResponseDto GetOrCreateUnitTestType(GetOrCreateUnitTestTypeRequestDto request)
        {
            CheckRequest(request);
            if (request.Data == null)
            {
                throw new ParameterRequiredException("Request.Data");
            }
            if (string.IsNullOrEmpty(request.Data.SystemName))
            {
                throw new ParameterRequiredException("Request.Data.SystemName");
            }

            AuthRequest(request);

            var storage = GetStorage();
            var service = new UnitTestTypeService(storage, _timeService);

            var unitTestType = service.GetOrCreateUnitTestType(request.Data);
            var typeDto = ApiConverter.GetUnitTestTypeInfo(unitTestType);

            return new GetOrCreateUnitTestTypeResponseDto()
            {
                Code = ResponseCode.Success,
                Data = typeDto
            };
        }

        public GetUnitTestTypeByIdResponse GetUnitTestTypeById(GetUnitTestTypeByIdRequest request)
        {
            CheckRequest(request);
            if (request.Data == null)
            {
                throw new ParameterRequiredException("Request.Data");
            }
            if (request.Data.Id == null)
            {
                throw new ParameterRequiredException("Request.Data.Id");
            }

            AuthRequest(request);

            var storage = GetStorage();
            var service = new UnitTestTypeService(storage, _timeService);
            var unitTestType = service.GetUnitTestTypeById(request.Data.Id.Value);
            var typeDto = ApiConverter.GetUnitTestTypeInfo(unitTestType);

            return new GetUnitTestTypeByIdResponse()
            {
                Code = ResponseCode.Success,
                Data = typeDto
            };
        }

        public UpdateUnitTestTypeResponse UpdateUnitTestType(UpdateUnitTestTypeRequest request)
        {
            CheckRequest(request);
            if (request.Data == null)
            {
                throw new ParameterRequiredException("Request.Data");
            }
            if (request.Data.UnitTestTypeId == null)
            {
                throw new ParameterRequiredException("Request.Data.UnitTestTypeId");
            }

            AuthRequest(request);

            var storage = GetStorage();
            var service = new UnitTestTypeService(storage, _timeService);
            service.UpdateUnitTestType(request.Data);
            var unitTestType = service.GetUnitTestTypeById(request.Data.UnitTestTypeId.Value);
            var typeDto = ApiConverter.GetUnitTestTypeInfo(unitTestType);

            return new UpdateUnitTestTypeResponse()
            {
                Code = ResponseCode.Success,
                Data = typeDto
            };
        }

        public DeleteUnitTestTypeResponse DeleteUnitTestType(DeleteUnitTestTypeRequest request)
        {
            CheckRequest(request);
            if (request.Data == null)
            {
                throw new ParameterRequiredException("Request.Data");
            }
            if (request.Data.UnitTestTypeId == null)
            {
                throw new ParameterRequiredException("Request.Data.UnitTestTypeId");
            }

            AuthRequest(request);

            var storage = GetStorage();
            var service = new UnitTestTypeService(storage, _timeService);
            service.DeleteUnitTestType(request.Data.UnitTestTypeId.Value);

            return new DeleteUnitTestTypeResponse()
            {
                Code = ResponseCode.Success
            };
        }

        public GetOrCreateUnitTestResponseDto GetOrCreateUnitTest(GetOrCreateUnitTestRequestDto request)
        {
            CheckRequest(request);
            if (request.Data == null)
            {
                throw new ParameterRequiredException("Request.Data");
            }
            if (request.Data.ComponentId == null)
            {
                throw new ParameterRequiredException("Request.Data.ComponentId");
            }

            AuthRequest(request);

            var storage = GetStorage();
            var service = new UnitTestService(storage, _timeService);
            var result = service.GetOrCreateUnitTest(request.Data);

            return new GetOrCreateUnitTestResponseDto()
            {
                Code = ResponseCode.Success,
                Data = ApiConverter.GetOrCreateUnitTestResponseData(result)
            };
        }

        public UpdateUnitTestResponse UpdateUnitTest(UpdateUnitTestRequest request)
        {
            CheckRequest(request);
            if (request.Data == null)
            {
                throw new ParameterRequiredException("Request.Data");
            }
            if (request.Data.UnitTestId == null)
            {
                throw new ParameterRequiredException("Request.Data.UnitTestId");
            }

            AuthRequest(request);

            var storage = GetStorage();
            var service = new UnitTestService(storage, _timeService);
            service.UpdateUnitTest(request.Data);

            return new UpdateUnitTestResponse()
            {
                Code = ResponseCode.Success
            };
        }

        public DeleteUnitTestResponse DeleteUnitTest(DeleteUnitTestRequest request)
        {
            CheckRequest(request);
            if (request.Data == null)
            {
                throw new ParameterRequiredException("Request.Data");
            }
            if (request.Data.UnitTestId == null)
            {
                throw new ParameterRequiredException("Request.Data.UnitTestId");
            }

            AuthRequest(request);

            var unitTestId = request.Data.UnitTestId.Value;

            var storage = GetStorage();
            var service = new UnitTestService(storage, _timeService);
            service.Delete(unitTestId);

            return new DeleteUnitTestResponse()
            {
                Code = ResponseCode.Success
            };
        }

        public SendUnitTestResultResponseDto SendUnitTestResult(SendUnitTestResultRequestDto request)
        {
            CheckRequest(request);
            if (request.Data == null)
            {
                throw new ParameterRequiredException("Request.Data");
            }
            if (request.Data.UnitTestId == null)
            {
                throw new ParameterRequiredException("Request.Data.UnitTestId");
            }

            AuthRequest(request);

            var storage = GetStorage();
            var service = new UnitTestService(storage, _timeService);
            service.SendUnitTestResult(request.Data);

            return new SendUnitTestResultResponseDto()
            {
                Code = ResponseCode.Success
            };
        }

        public SendUnitTestResultsResponseDto SendUnitTestResults(SendUnitTestResultsRequestDto request)
        {
            CheckRequest(request);
            if (request.Data == null)
            {
                throw new ParameterRequiredException("Request.Data");
            }

            AuthRequest(request);

            var storage = GetStorage();
            var service = new UnitTestService(storage, _timeService);
            service.SendUnitTestResults(request.Data);

            return new SendUnitTestResultsResponseDto()
            {
                Code = ResponseCode.Success
            };
        }

        public GetUnitTestStateResponseDto GetUnitTestState(GetUnitTestStateRequestDto request)
        {
            CheckRequest(request);
            if (request.Data == null)
            {
                throw new ParameterRequiredException("Request.Data");
            }
            if (request.Data.UnitTestId == null)
            {
                throw new ParameterRequiredException("Request.Data.UnitTestId");
            }

            AuthRequest(request);

            var storage = GetStorage();
            var service = new UnitTestService(storage, _timeService);
            var result = service.GetUnitTestResult(request.Data.UnitTestId.Value);
            var resultDto = ApiConverter.GetStateDataInfo(result);

            return new GetUnitTestStateResponseDto()
            {
                Code = ResponseCode.Success,
                Data = resultDto
            };
        }

        public AddPingUnitTestResponse AddPingUnitTest(AddPingUnitTestRequest request)
        {
            CheckRequest(request);
            if (request.Data == null)
            {
                throw new ParameterRequiredException("Request.Data");
            }
            if (string.IsNullOrEmpty(request.Data.SystemName))
            {
                throw new ParameterRequiredException("Request.Data.SystemName");
            }
            if (request.Data.ComponentId == null)
            {
                throw new ParameterRequiredException("Request.Data.ComponentId");
            }

            AuthRequest(request);

            var storage = GetStorage();
            var service = new UnitTestService(storage, _timeService);
            var result = service.AddPingUnitTest(request.Data);
            var unittest = storage.UnitTests.GetOneById(result);
            var rule = storage.UnitTestPingRules.GetOneByUnitTestId(result);

            return new AddPingUnitTestResponse()
            {
                Code = ResponseCode.Success,
                Data = ApiConverter.AddPingUnitTestResponseData(unittest, rule)
            };
        }

        public AddHttpUnitTestResponse AddHttpUnitTest(AddHttpUnitTestRequest request)
        {
            CheckRequest(request);
            if (request.Data == null)
            {
                throw new ParameterRequiredException("Request.Data");
            }
            if (string.IsNullOrEmpty(request.Data.SystemName))
            {
                throw new ParameterRequiredException("Request.Data.SystemName");
            }
            if (request.Data.ComponentId == null)
            {
                throw new ParameterRequiredException("Request.Data.ComponentId");
            }

            AuthRequest(request);

            var storage = GetStorage();
            var service = new UnitTestService(storage, _timeService);
            var id = service.AddHttpUnitTest(request.Data);
            var unittest = storage.UnitTests.GetOneById(id);
            var rules = storage.HttpRequestUnitTestRules.GetByUnitTestId(id);

            return new AddHttpUnitTestResponse()
            {
                Code = ResponseCode.Success,
                Data = ApiConverter.AddHttpUnitTestResponseData(unittest, rules)
            };
        }

        public RecalcUnitTestsResultsResponse RecalcUnitTestsResults(RecalcUnitTestsResultsRequest request)
        {
            CheckRequest(request);

            AuthRequest(request);

            var storage = GetStorage();
            var service = new UnitTestService(storage, _timeService);
            var updated = service.RecalcUnitTestsResults(request.Data.MaxCount);

            return new RecalcUnitTestsResultsResponse()
            {
                Code = ResponseCode.Success,
                Data = new RecalcUnitTestsResultsResponseData()
                {
                    UpdateCount = updated
                }
            };
        }

        public SetUnitTestEnableResponseDto SetUnitTestEnable(SetUnitTestEnableRequestDto request)
        {
            CheckRequest(request);
            if (request.Data.UnitTestId == null)
            {
                throw new ParameterRequiredException("Request.Data.UnitTestId");
            }

            AuthRequest(request);

            var unitTestId = request.Data.UnitTestId;

            var storage = GetStorage();
            var service = new UnitTestService(storage, _timeService);
            service.Enable(unitTestId.Value);

            return new SetUnitTestEnableResponseDto()
            {
                Code = ResponseCode.Success
            };
        }

        public SetUnitTestDisableResponseDto SetUnitTestDisable(SetUnitTestDisableRequestDto request)
        {
            CheckRequest(request);
            if (request.Data.UnitTestId == null)
            {
                throw new ParameterRequiredException("Request.Data.UnitTestId");
            }

            AuthRequest(request);

            var storage = GetStorage();
            var service = new UnitTestService(storage, _timeService);
            service.Disable(request.Data);

            return new SetUnitTestDisableResponseDto()
            {
                Code = ResponseCode.Success
            };
        }

        #endregion

        #region Подписки

        public CreateSubscriptionResponse CreateSubscription(CreateSubscriptionRequest request)
        {
            CheckRequest(request);

            AuthRequest(request);

            var storage = GetStorage();
            var service = new SubscriptionService(storage, _timeService);
            var subscription = service.CreateSubscription(request.Data);
            var subscriptionDto = ApiConverter.GetSubscriptionInfo(subscription);

            return new CreateSubscriptionResponse()
            {
                Code = ResponseCode.Success,
                Data = subscriptionDto
            };
        }

        public CreateUserDefaultSubscriptionResponse CreateUserDefaultSubscription(CreateUserDefaultSubscriptionRequest request)
        {
            CheckRequest(request);

            AuthRequest(request);

            var storage = GetStorage();
            var service = new SubscriptionService(storage, _timeService);
            var subscription = service.CreateDefaultForUser(request.Data.UserId);
            var subscriptionDto = ApiConverter.GetSubscriptionInfo(subscription);

            return new CreateUserDefaultSubscriptionResponse()
            {
                Code = ResponseCode.Success,
                Data = subscriptionDto
            };
        }

        public UpdateSubscriptionResponse UpdateSubscription(UpdateSubscriptionRequest request)
        {
            CheckRequest(request);

            AuthRequest(request);

            var storage = GetStorage();
            var service = new SubscriptionService(storage, _timeService);
            service.UpdateSubscription(request.Data);
            var subscription = storage.Subscriptions.GetOneById(request.Data.Id);
            var subscriptionDto = ApiConverter.GetSubscriptionInfo(subscription);

            return new UpdateSubscriptionResponse()
            {
                Code = ResponseCode.Success,
                Data = subscriptionDto
            };
        }

        public SetSubscriptionDisableResponse SetSubscriptionDisable(SetSubscriptionDisableRequest request)
        {
            CheckRequest(request);

            AuthRequest(request);

            var storage = GetStorage();
            var service = new SubscriptionService(storage, _timeService);
            service.SetSubscriptionDisable(request.Data);

            return new SetSubscriptionDisableResponse()
            {
                Code = ResponseCode.Success
            };
        }

        public SetSubscriptionEnableResponse SetSubscriptionEnable(SetSubscriptionEnableRequest request)
        {
            CheckRequest(request);

            AuthRequest(request);

            var storage = GetStorage();
            var service = new SubscriptionService(storage, _timeService);
            service.SetSubscriptionEnable(request.Data);

            return new SetSubscriptionEnableResponse()
            {
                Code = ResponseCode.Success
            };
        }

        public DeleteSubscriptionResponse DeleteSubscription(DeleteSubscriptionRequest request)
        {
            CheckRequest(request);

            AuthRequest(request);

            var storage = GetStorage();
            var service = new SubscriptionService(storage, _timeService);
            service.DeleteSubscription(request.Data);

            return new DeleteSubscriptionResponse()
            {
                Code = ResponseCode.Success
            };
        }

        public SendSmsResponse SendSms(SendSmsRequest request)
        {
            CheckRequest(request);
            AuthRequest(request);

            var storage = GetStorage();

            var entity = new SendSmsCommandForAdd()
            {
                Id = Ulid.NewUlid(),
                CreateDate = _timeService.Now(),
                Status = SmsStatus.InQueue,
                ReferenceId = request.Data.ReferenceId,
                Body = request.Data.Body,
                Phone = request.Data.Phone
            };
            storage.SendSmsCommands.Add(entity);

            var checker = AccountLimitsCheckerManager.GetChecker();
            checker.AddSmsPerDay(storage);

            return new SendSmsResponse()
            {
                Code = ResponseCode.Success
            };
        }

        #endregion

        #region Статус компонента

        public GetComponentTotalStateResponseDto GetComponentTotalState(GetComponentTotalStateRequestDto request)
        {
            CheckRequest(request);
            if (request.Data == null)
            {
                throw new ParameterRequiredException("Request.Data");
            }
            if (request.Data.ComponentId == null)
            {
                throw new ParameterRequiredException("Request.Data.ComponentId");
            }

            AuthRequest(request);

            var componentId = request.Data.ComponentId.Value;

            var storage = GetStorage();
            var recalc = request.Data.Recalc;
            var componentService = new ComponentService(storage, _timeService);
            var state = componentService.GetComponentExternalState(componentId, recalc);
            var stateDto = ApiConverter.GetStateDataInfo(state);

            var component = componentService.GetComponentById(componentId);
            stateDto.DisableComment = component.DisableComment;
            stateDto.DisableToDate = component.DisableToDate;

            return new GetComponentTotalStateResponseDto()
            {
                Code = ResponseCode.Success,
                Data = stateDto
            };
        }

        public GetComponentInternalStateResponseDto GetComponentInternalState(GetComponentInternalStateRequestDto request)
        {
            CheckRequest(request);
            if (request.Data == null)
            {
                throw new ParameterRequiredException("Request.Data");
            }
            if (request.Data.ComponentId == null)
            {
                throw new ParameterRequiredException("Request.Data.ComponentId");
            }

            AuthRequest(request);

            var componentId = request.Data.ComponentId.Value;

            var storage = GetStorage();
            var recalc = request.Data.Recalc;
            var service = new ComponentService(storage, _timeService);
            var state = service.GetComponentInternalState(componentId, recalc);
            var stateDto = ApiConverter.GetStateDataInfo(state);

            return new GetComponentInternalStateResponseDto()
            {
                Code = ResponseCode.Success,
                Data = stateDto
            };
        }

        public UpdateComponentStateResponse UpdateComponentState(UpdateComponentStateRequest request)
        {
            CheckRequest(request);
            if (request.Data == null)
            {
                throw new ParameterRequiredException("Request.Data");
            }
            if (request.Data.ComponentId == null)
            {
                throw new ParameterRequiredException("Request.Data.ComponentId");
            }

            AuthRequest(request);

            var componentId = request.Data.ComponentId.Value;

            var storage = GetStorage();
            var service = new ComponentService(storage, _timeService);
            var component = service.CalculateAllStatuses(componentId);

            return new UpdateComponentStateResponse()
            {
                Code = ResponseCode.Success
            };
        }

        #endregion

        #region Тарифы и лимиты

        public GetAccountLimitsResponse GetAccountLimits(GetAccountLimitsRequest request)
        {
            CheckRequest(request);

            if (request.Data == null)
            {
                throw new ParameterRequiredException("Request.Data");
            }

            AuthRequest(request);

            var storage = GetStorage();
            var limits = GetAccountLimitsResponseData(storage, request.Data.ArchiveDays);

            return new GetAccountLimitsResponse()
            {
                Code = ResponseCode.Success,
                Data = limits
            };
        }

        // TODO Move to service
        protected GetAccountLimitsResponseData GetAccountLimitsResponseData(IStorage storage, int archiveDays)
        {
            // Получим проверяльщик лимитов
            var checker = AccountLimitsCheckerManager.GetChecker();

            // Получим использованные лимиты
            var usedToday = checker.GetUsedTodayTariffLimit(storage);
            var usedOverall = checker.GetUsedOverallTariffLimit(storage, archiveDays);

            // Сформируем результат
            return new GetAccountLimitsResponseData()
            {
                UsedToday = usedToday,
                UsedOverall = usedOverall
            };
        }

        #endregion

    }
}
