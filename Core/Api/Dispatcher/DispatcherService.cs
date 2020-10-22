using System;
using System.Collections.Generic;
using System.Linq;
using ApiAdapter;
using Zidium.Core.AccountsDb;
using Zidium.Core.Api;
using Zidium.Core.Api.Accounts.ChangeApiKey;
using Zidium.Core.Caching;
using Zidium.Core.Common.Helpers;
using Zidium.Core.ConfigDb;
using Zidium.Core.Limits;
using Zidium.Storage;

namespace Zidium.Core
{
    /// <summary>
    /// Данный класс отвечает за перенаправление вызовов тем экземплярам служб, который работают с нужной БД аккаунта
    /// </summary>
    public class DispatcherService : IDispatcherService
    {
        #region Служебные

        public Zidium.Api.IComponentControl Control { get; private set; }

        private static DispatcherWrapper _wrapper;

        private DispatcherService(Zidium.Api.IComponentControl control)
        {
            _accountStorageFactory = DependencyInjection.GetServicePersistent<IAccountStorageFactory>();
            _configDbServicesFactory = DependencyInjection.GetServicePersistent<IConfigDbServicesFactory>();
            Control = control;
            StaticControl = control;
            //AllCaches.Init();
        }

        private readonly IAccountStorageFactory _accountStorageFactory;

        private readonly IConfigDbServicesFactory _configDbServicesFactory;

        public static Zidium.Api.IComponentControl StaticControl = new Zidium.Api.FakeComponentControl();

        private static readonly object _lockObject = new object();

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
                            var realService = new DispatcherService(realControl);
                            var wrapper = new DispatcherWrapper(realService, realControl);
                            AllCaches.SetControl(realControl);
                            _wrapper = wrapper;
                        }
                    }
                }
                return _wrapper;
            }
        }

        public static string Version { get; set; }

        private static Zidium.Api.IComponentControl GetRealControl()
        {
            try
            {
                // делаем цепочку вызовов:
                // контрол => адаптер => внутренний диспетчер => фейковый контрол
                var fakeControl = GetFakeControl();
                IDispatcherService internalDispatcher = new DispatcherService(fakeControl); // обслуживает только запросы самого диспетчера (через контрол АПИ)
                internalDispatcher = new DispatcherWrapper(internalDispatcher, fakeControl);
                var adapter = new ApiToDispatcherAdapter(internalDispatcher, NetworkHelper.GetLocalIp(), SystemAccountHelper.SystemAccountName);
                var token = SystemAccountHelper.GetSystemToken();
                var apiService = new Zidium.Api.ApiService(adapter, token);
                var client = SystemAccountHelper.GetApiClient(apiService);
                client.EventPreparer = new HttpServiceEventPreparer();

                // Создадим компонент
                // Если запускаемся в отладке, то компонент будет не в корне, а в папке DEBUG
                var debugConfiguration = DependencyInjection.GetServicePersistent<IDebugConfiguration>();
                var folder = !debugConfiguration.DebugMode ? client.GetRootComponentControl() : client.GetRootComponentControl().GetOrCreateChildFolderControl("DEBUG");
                var componentType = client.GetOrCreateComponentTypeControl(!debugConfiguration.DebugMode ? "Dispatcher" : DebugHelper.DebugComponentType);
                var version = Version ?? typeof(DispatcherService).Assembly.GetName().Version.ToString();
                var componentControl = folder.GetOrCreateChildComponentControl(componentType, "Dispatcher", version);

                // Присвоим Id компонента по умолчанию, чтобы адаптер NLog мог его использовать
                Zidium.Api.Client.Instance = client;
                Zidium.Api.Client.Instance.Config.DefaultComponent.Id = componentControl.Info?.Id;

                return componentControl;
            }
            catch
            {
                return GetFakeControl();
            }
        }

        private static Zidium.Api.IComponentControl GetFakeControl()
        {
            return new Zidium.Api.FakeComponentControl("FakeDispatcher");
        }

        protected delegate T ExecuteMethodDelegate<T>();

        protected void CheckRequest(Request request)
        {
            if (request == null)
            {
                throw new ParameterRequiredException("Request");
            }
            if (request.Token == null)
            {
                throw new ParameterRequiredException("Request.Token");
            }
            if (!request.Token.IsLocalRequest)
            {
                if (string.IsNullOrEmpty(request.Token.AccountName) && request.Token.AccountId == null)
                {
                    throw new ParameterRequiredException("Request.Token.AccountName");
                }
                if (request.Token.SecretKey == null)
                {
                    throw new ParameterRequiredException("Request.Token.SecretKey");
                }
            }

        }

        protected IStorage GetStorage(Guid accountId)
        {
            return _accountStorageFactory.GetStorageByAccountId(accountId);
        }

        protected void AuthRequest(Request request)
        {
            // Если запрос локальный и аккаунт не указан, то это системный аккаунт
            if (request.Token.IsLocalRequest && request.Token.AccountId == null)
            {
                request.Token.AccountId = SystemAccountHelper.GetSystemAccountId();

                // Здесь проверяем только секретный ключ
                if (request.Token.SecretKey != SystemAccountHelper.GetSystemToken().SecretKey)
                {
                    throw new Zidium.Api.Dto.ResponseCodeException(Zidium.Api.ResponseCode.InvalidSecretKey, "Неверный SecretKey");
                }
            }
            else
            {
                // Для всех остальных запросов проверим указанный аккаунт

                // Проверим, что указан аккаунт
                if (request.Token.AccountId == null && string.IsNullOrEmpty(request.Token.AccountName))
                {
                    throw new Zidium.Api.Dto.ResponseCodeException(Zidium.Api.ResponseCode.InvalidAccountName, "Не указан AccountName");
                }

                AccountInfo account;
                string searchKey;

                // Найдём аккаунт по Id или по названию
                var accountService = _configDbServicesFactory.GetAccountService();
                if (request.Token.AccountId.HasValue)
                {
                    searchKey = request.Token.AccountId.Value.ToString();
                    account = accountService.GetOneOrNullById(request.Token.AccountId.Value);
                }
                else
                {
                    searchKey = request.Token.AccountName;
                    account = accountService.GetOneOrNullBySystemName(request.Token.AccountName);
                }

                if (account == null)
                {
                    throw new Zidium.Api.Dto.ResponseCodeException(
                        Zidium.Api.ResponseCode.InvalidAccountName,
                        "Неизвестный Account: " + searchKey);
                }

                request.Token.AccountId = account.Id;

                // Проверим секретный ключ
                // Для локального запроса ключ должен быть от системного аккаунта
                if (request.Token.IsLocalRequest)
                {
                    if (request.Token.SecretKey != SystemAccountHelper.GetSystemToken().SecretKey)
                    {
                        throw new Zidium.Api.Dto.ResponseCodeException(Zidium.Api.ResponseCode.InvalidSecretKey, "Неверный SecretKey");
                    }
                }
                else
                {
                    // Для обычного запроса ключ должен совпадать с ключом аккаунта
                    if (request.Token.SecretKey != account.SecretKey)
                    {
                        throw new Zidium.Api.Dto.ResponseCodeException(Zidium.Api.ResponseCode.InvalidSecretKey, "Неверный SecretKey");
                    }

                    if (account.Status == AccountStatus.Blocked)
                    {
                        throw new Zidium.Api.Dto.ResponseCodeException(Zidium.Api.ResponseCode.AccountBlocked, "Аккаунт заблокирован");
                    }
                }
            }
        }

        protected void CheckForLocalRequest(Request request)
        {
            if (!request.Token.IsLocalRequest)
                throw new Zidium.Api.Dto.ResponseCodeException(Zidium.Api.ResponseCode.NonLocalRequest, "Данный вызов должен быть локальным");
        }

        #endregion

        #region Разное

        public GetEchoResponse GetEcho(GetEchoRequest request)
        {
            CheckRequest(request);
            if (request.Data == null)
            {
                throw new ParameterRequiredException("Request.Data");
            }

            AuthRequest(request); // чтобы убедиться, что AccountId и SecretKey верные

            return new GetEchoResponse()
            {
                Code = Zidium.Api.ResponseCode.Success,
                InternalData = request.Data.Message
            };
        }

        public GetServerTimeResponse GetServerTime()
        {
            return new GetServerTimeResponse()
            {
                Code = Zidium.Api.ResponseCode.Success,
                InternalData = new GetServerTimeResponseData()
                {
                    Date = DateTime.Now
                }
            };
        }

        public SaveAllCachesResponse SaveAllCaches(SaveAllCachesRequest request)
        {
            CheckForLocalRequest(request);
            CheckRequest(request);
            AllCaches.SaveChanges();
            return new SaveAllCachesResponse()
            {
                Code = Zidium.Api.ResponseCode.Success
            };
        }

        #endregion

        #region Компоненты и папки

        public GetRootControlDataResponse GetRootControlData(GetRootControlDataRequest request)
        {
            CheckRequest(request);

            AuthRequest(request);

            var accountId = request.Token.AccountId.Value;
            var storage = GetStorage(accountId);

            var root = storage.Components.GetRoot();
            var componentType = storage.ComponentTypes.GetOneById(root.ComponentTypeId);
            var properties = storage.ComponentProperties.GetByComponentId(root.Id);

            var logService = new LogService(storage);
            var logConfig = logService.GetLogConfig(root.Id);

            return new GetRootControlDataResponse()
            {
                Code = Zidium.Api.ResponseCode.Success,
                InternalData = new ComponentControlData()
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

            var accountId = request.Token.AccountId.Value;

            // создаём компонент
            var storage = GetStorage(accountId);
            var componentService = new ComponentService(storage);
            var component = componentService.CreateComponent(accountId, request.Data);
            var componentType = storage.ComponentTypes.GetOneById(component.ComponentTypeId);
            var properties = storage.ComponentProperties.GetByComponentId(component.Id);
            var componentInfo = ApiConverter.GetComponentInfo(component, properties, componentType);

            // получаем конфиг веб-лога
            var logService = new LogService(storage);
            var logConfig = logService.GetLogConfig(component.Id);
            var logConfigDto = ApiConverter.GetLogConfig(logConfig);

            return new CreateComponentResponse()
            {
                Code = Zidium.Api.ResponseCode.Success,
                InternalData = new ComponentControlData()
                {
                    Component = componentInfo,
                    WebLogConfig = logConfigDto
                }
            };

        }

        public GetOrCreateComponentResponse GetOrCreateComponent(GetOrCreateComponentRequest request)
        {
            CheckRequest(request);
            if (request.Data == null)
            {
                throw new ParameterRequiredException("Request.Data");
            }

            AuthRequest(request);

            var accountId = request.Token.AccountId.Value;

            // получаем или создаём компонент
            var storage = GetStorage(accountId);
            var componentService = new ComponentService(storage);
            var component = componentService.GetOrCreateComponent(accountId, request.Data);
            var componentType = storage.ComponentTypes.GetOneById(component.ComponentTypeId);
            var properties = storage.ComponentProperties.GetByComponentId(component.Id);
            var componentInfo = ApiConverter.GetComponentInfo(component, properties, componentType);

            // получаем конфиг веб-лога
            var logService = new LogService(storage);
            var logConfig = logService.GetLogConfig(component.Id);
            var logConfigDto = ApiConverter.GetLogConfig(logConfig);

            return new GetOrCreateComponentResponse()
            {
                Code = Zidium.Api.ResponseCode.Success,
                InternalData = new ComponentControlData()
                {
                    Component = componentInfo,
                    WebLogConfig = logConfigDto
                }
            };
        }

        public GetComponentControlByIdResponse GetComponentControlById(GetComponentControlByIdRequest request)
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

            var accountId = request.Token.AccountId.Value;
            var componentId = request.Data.ComponentId.Value;

            // получаем или создаём компонент
            var storage = GetStorage(accountId);
            var componentService = new ComponentService(storage);

            var component = componentService.GetComponentByIdNoCache(accountId, componentId);
            if (component.IsDeleted)
            {
                throw new UnknownComponentIdException(componentId, accountId);
            }

            var componentType = storage.ComponentTypes.GetOneById(component.ComponentTypeId);
            var properties = storage.ComponentProperties.GetByComponentId(component.Id);
            var componentInfo = ApiConverter.GetComponentInfo(component, properties, componentType);

            // получаем конфиг веб-лога
            var logService = new LogService(storage);
            var logConfig = logService.GetLogConfig(component.Id);
            var logConfigDto = ApiConverter.GetLogConfig(logConfig);

            return new GetComponentControlByIdResponse()
            {
                Code = Zidium.Api.ResponseCode.Success,
                InternalData = new ComponentControlData()
                {
                    Component = componentInfo,
                    WebLogConfig = logConfigDto
                }
            };
        }

        public GetOrAddComponentResponse GetOrAddComponent(GetOrAddComponentRequest request)
        {
            CheckRequest(request);
            if (request.Data == null)
            {
                throw new ParameterRequiredException("Request.Data");
            }

            AuthRequest(request);

            var accountId = request.Token.AccountId.Value;

            // получаем или создаём компонент
            var storage = GetStorage(accountId);
            var componentService = new ComponentService(storage);
            var component = componentService.GetOrCreateComponent(accountId, request.Data);
            var componentType = storage.ComponentTypes.GetOneById(component.ComponentTypeId);
            var properties = storage.ComponentProperties.GetByComponentId(component.Id);
            var componentInfo = ApiConverter.GetComponentInfo(component, properties, componentType);

            return new GetOrAddComponentResponse()
            {
                Code = Zidium.Api.ResponseCode.Success,
                InternalData = componentInfo
            };

        }

        public GetRootComponentResponse GetRootComponent(GetRootComponentRequest request)
        {
            CheckRequest(request);

            AuthRequest(request);

            var accountId = request.Token.AccountId.Value;

            var storage = GetStorage(accountId);
            var root = storage.Components.GetRoot();
            var componentType = storage.ComponentTypes.GetOneById(root.ComponentTypeId);
            var properties = storage.ComponentProperties.GetByComponentId(root.Id);
            var componentInfo = ApiConverter.GetComponentInfo(root, properties, componentType);

            return new GetRootComponentResponse()
            {
                Code = Zidium.Api.ResponseCode.Success,
                InternalData = componentInfo
            };

        }

        public GetComponentByIdResponse GetComponentById(GetComponentByIdRequest request)
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

            var accountId = request.Token.AccountId.Value;
            var componentId = request.Data.ComponentId.Value;

            var storage = GetStorage(accountId);
            var componentService = new ComponentService(storage);
            var component = componentService.GetComponentByIdNoCache(accountId, componentId);
            var componentType = storage.ComponentTypes.GetOneById(component.ComponentTypeId);
            var properties = storage.ComponentProperties.GetByComponentId(component.Id);
            var componentInfo = ApiConverter.GetComponentInfo(component, properties, componentType);

            return new GetComponentByIdResponse()
            {
                Code = Zidium.Api.ResponseCode.Success,
                InternalData = componentInfo
            };

        }

        public GetComponentBySystemNameResponse GetComponentBySystemName(GetComponentBySystemNameRequest request)
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

            var accountId = request.Token.AccountId.Value;

            var storage = GetStorage(accountId);
            var componentService = new ComponentService(storage);
            var component = componentService.GetComponentBySystemName(accountId, request.Data.ParentId.Value, request.Data.SystemName);
            var componentType = storage.ComponentTypes.GetOneById(component.ComponentTypeId);
            var properties = storage.ComponentProperties.GetByComponentId(component.Id);
            var componentInfo = ApiConverter.GetComponentInfo(component, properties, componentType);

            return new GetComponentBySystemNameResponse()
            {
                Code = Zidium.Api.ResponseCode.Success,
                InternalData = componentInfo
            };
        }

        public UpdateComponentResponse UpdateComponent(UpdateComponentRequest request)
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

            var accountId = request.Token.AccountId.Value;

            var storage = GetStorage(accountId);
            var componentService = new ComponentService(storage);
            componentService.UpdateComponent(accountId, request.Data);
            var component = storage.Components.GetOneById(request.Data.Id.Value);
            var componentType = storage.ComponentTypes.GetOneById(component.ComponentTypeId);
            var properties = storage.ComponentProperties.GetByComponentId(component.Id);
            var componentInfo = ApiConverter.GetComponentInfo(component, properties, componentType);

            return new UpdateComponentResponse()
            {
                Code = Zidium.Api.ResponseCode.Success,
                InternalData = componentInfo
            };
        }

        public GetChildComponentsResponse GetChildComponents(GetChildComponentsRequest request)
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

            var accountId = request.Token.AccountId.Value;
            var componentId = request.Data.ComponentId.Value;

            var storage = GetStorage(accountId);
            var componentService = new ComponentService(storage);
            var components = componentService.GetChildComponents(componentId);
            var componentInfos = components.Select(component =>
            {
                var componentType = storage.ComponentTypes.GetOneById(component.ComponentTypeId);
                var properties = storage.ComponentProperties.GetByComponentId(component.Id);
                return ApiConverter.GetComponentInfo(component, properties, componentType);
            }).ToArray();

            return new GetChildComponentsResponse()
            {
                Code = Zidium.Api.ResponseCode.Success,
                InternalData = componentInfos
            };
        }

        public SetComponentEnableResponse SetComponentEnable(SetComponentEnableRequest request)
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

            var accountId = request.Token.AccountId.Value;

            var storage = GetStorage(accountId);
            var componentService = new ComponentService(storage);
            componentService.EnableComponent(accountId, request.Data.ComponentId.Value);

            return new SetComponentEnableResponse()
            {
                Code = Zidium.Api.ResponseCode.Success
            };
        }

        public SetComponentDisableResponse SetComponentDisable(SetComponentDisableRequest request)
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

            var accountId = request.Token.AccountId.Value;

            var storage = GetStorage(accountId);
            var componentService = new ComponentService(storage);
            componentService.DisableComponent(
                    accountId,
                    request.Data.ComponentId.Value,
                    request.Data.ToDate,
                    request.Data.Comment);

            return new SetComponentDisableResponse()
            {
                Code = Zidium.Api.ResponseCode.Success
            };
        }

        public DeleteComponentResponse DeleteComponent(DeleteComponentRequest request)
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

            var accountId = request.Token.AccountId.Value;
            var componentId = request.Data.ComponentId.Value;

            var storage = GetStorage(accountId);
            var componentService = new ComponentService(storage);
            componentService.DeleteComponent(accountId, componentId);

            return new DeleteComponentResponse()
            {
                Code = Zidium.Api.ResponseCode.Success
            };
        }

        public GetComponentAndChildIdsResponse GetComponentAndChildIds(GetComponentAndChildIdsRequest request)
        {
            CheckForLocalRequest(request);
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

            var accountId = request.Token.AccountId.Value;
            var componentId = request.Data.ComponentId.Value;

            var storage = GetStorage(accountId);
            var componentService = new ComponentService(storage);
            var result = componentService.GetComponentAndChildIds(accountId, componentId);

            return new GetComponentAndChildIdsResponse()
            {
                Code = Zidium.Api.ResponseCode.Success,
                InternalData = result
            };
        }

        public UpdateEventsStatusesResponse UpdateEventsStatuses(UpdateEventsStatusesRequest request)
        {
            CheckRequest(request);

            AuthRequest(request);

            var accountId = request.Token.AccountId.Value;

            var storage = GetStorage(accountId);
            var componentService = new ComponentService(storage);
            var updated = componentService.UpdateEventsStatuses(accountId, request.Data.MaxCount);

            return new UpdateEventsStatusesResponse()
            {
                Code = Zidium.Api.ResponseCode.Success,
                InternalData = new UpdateEventsStatusesResponseData()
                {
                    UpdateCount = updated
                }
            };
        }

        #endregion

        #region События

        public SendEventResponse SendEvent(SendEventRequest request)
        {
            CheckRequest(request);
            if (request.Data == null)
            {
                throw new ParameterRequiredException("Request.Data");
            }

            AuthRequest(request);

            try
            {
                var accountId = request.Token.AccountId.Value;
                var componentId = request.Data.ComponentId.Value;

                var storage = GetStorage(accountId);
                var service = new EventService(storage);

                var eventData = service.SendEvent(
                    accountId,
                    componentId,
                    request.Data);

                return new SendEventResponse()
                {
                    Code = Zidium.Api.ResponseCode.Success,
                    InternalData = new SendEventResponseData()
                    {
                        EventId = eventData.Id,
                        EventTypeId = eventData.EventTypeId
                    }
                };
            }
            catch (Zidium.Api.Dto.ResponseCodeException responseException)
            {
                return new SendEventResponse()
                {
                    Code = responseException.Code,
                    ErrorMessage = responseException.Message
                };
            }
        }

        public JoinEventsResponse JoinEvents(JoinEventsRequest request)
        {
            CheckRequest(request);
            if (request.Data == null)
            {
                throw new ParameterRequiredException("Request.Data");
            }

            AuthRequest(request);

            try
            {
                var accountId = request.Token.AccountId.Value;

                var storage = GetStorage(accountId);
                var service = new EventService(storage);

                foreach (var joinData in request.Data)
                {
                    service.JoinEvent(accountId, joinData.ComponentId.Value, joinData);
                }
            }
            catch (Exception)
            {

            }
            return new JoinEventsResponse()
            {
                Code = Zidium.Api.ResponseCode.Success
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

            var accountId = request.Token.AccountId.Value;

            var storage = GetStorage(accountId);
            var service = new EventTypeService(storage);
            service.Update(accountId, request.Data);

            return new UpdateEventTypeResponse()
            {
                Code = Zidium.Api.ResponseCode.Success,
            };
        }

        public GetEventByIdResponse GetEventById(GetEventByIdRequest request)
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

            var accountId = request.Token.AccountId.Value;

            var storage = GetStorage(accountId);
            var eventData = storage.Events.GetOneById(request.Data.EventId.Value);
            var type = storage.EventTypes.GetOneById(eventData.EventTypeId);
            var properties = storage.EventProperties.GetByEventId(eventData.Id);
            var eventInfo = ApiConverter.GetEventInfo(eventData, type, properties);

            return new GetEventByIdResponse()
            {
                Code = Zidium.Api.ResponseCode.Success,
                InternalData = eventInfo
            };
        }

        public GetEventsResponse GetEvents(GetEventsRequest request)
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

            var accountId = request.Token.AccountId.Value;

            var storage = GetStorage(accountId);
            var service = new EventService(storage);
            var events = service.GetEvents(accountId, request.Data);

            var eventInfos = events.Select(eventObj =>
            {
                var eventType = storage.EventTypes.GetOneById(eventObj.EventTypeId);
                var properties = storage.EventProperties.GetByEventId(eventObj.Id);
                var eventInfo = ApiConverter.GetEventInfo(eventObj, eventType, properties);
                return eventInfo;
            }).ToArray();

            return new GetEventsResponse()
            {
                Code = Zidium.Api.ResponseCode.Success,
                InternalData = eventInfos
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

            var accountId = request.Token.AccountId.Value;

            var storage = GetStorage(accountId);
            var service = new MetricService(storage);
            var metric = service.CreateMetric(accountId, request.Data);
            var responseData = new CreateMetricResponseData()
            {
                MetricId = metric.Id,
                MetricTypeId = metric.MetricTypeId
            };

            return new CreateMetricResponse()
            {
                Code = Zidium.Api.ResponseCode.Success,
                InternalData = responseData
            };
        }

        public GetMetricsResponse GetMetrics(GetMetricsRequest request)
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

            var accountId = request.Token.AccountId.Value;
            var componentId = request.Data.ComponentId.Value;

            var storage = GetStorage(accountId);
            var service = new MetricService(storage);
            var metrics = service.GetMetrics(accountId, componentId);
            var metricInfos = new List<MetricInfo>(metrics.Count);
            foreach (var metric in metrics)
            {
                var metricType = AllCaches.MetricTypes.Find(new AccountCacheRequest()
                {
                    AccountId = accountId,
                    ObjectId = metric.MetricTypeId
                });
                if (metricType == null)
                {
                    throw new Exception("metricType == null");
                }
                var statusData = AllCaches.StatusDatas.Find(new AccountCacheRequest()
                {
                    AccountId = accountId,
                    ObjectId = metric.StatusDataId
                });
                if (statusData == null)
                {
                    throw new Exception("statusData == null");
                }
                var metricInfo = ApiConverter.GetMetricInfo(metric, metricType, statusData);
                metricInfos.Add(metricInfo);
            }

            var response = new GetMetricsResponse
            {
                Code = Zidium.Api.ResponseCode.Success,
                InternalData = metricInfos.ToArray()
            };
            return response;
        }

        public GetMetricsHistoryResponse GetMetricsHistory(GetMetricsHistoryRequest request)
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

            var accountId = request.Token.AccountId.Value;

            var storage = GetStorage(accountId);
            var service = new MetricService(storage);
            var rows = service.GetMetricsHistory(accountId, request.Data);

            // конвертируем историю в MetricInfo
            var metricInfos = rows.Select(row =>
            {
                var type = storage.MetricTypes.GetOneById(row.MetricTypeId);
                var info = new MetricInfo()
                {
                    ActualDate = row.ActualDate,
                    BeginDate = row.BeginDate,
                    ComponentId = row.ComponentId,
                    SystemName = type.SystemName,
                    Status = MonitoringStatusHelper.Get(row.Color),
                    Value = row.Value
                };
                return info;
            }).ToArray();

            return new GetMetricsHistoryResponse()
            {
                Code = Zidium.Api.ResponseCode.Success,
                InternalData = metricInfos
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

            var accountId = typeRequest.Token.AccountId.Value;

            var storage = GetStorage(accountId);
            var service = new MetricService(storage);
            service.DeleteMetricType(accountId, typeRequest.Data.MetricTypeId);

            return new DeleteMetricTypeResponse()
            {
                Code = Zidium.Api.ResponseCode.Success
            };
        }

        public SendMetricsResponse SendMetrics(SendMetricsRequest request)
        {
            CheckRequest(request);
            if (request.Data == null)
            {
                throw new ParameterRequiredException("Request.Data");
            }

            AuthRequest(request);

            var accountId = request.Token.AccountId.Value;

            var storage = GetStorage(accountId);
            var service = new MetricService(storage);
            service.SaveMetrics(accountId, request.Data);
            return new SendMetricsResponse()
            {
                Code = Zidium.Api.ResponseCode.Success
            };
        }

        public UpdateMetricsResponse UpdateMetrics(UpdateMetricsRequest request)
        {
            CheckRequest(request);

            AuthRequest(request);

            var accountId = request.Token.AccountId.Value;

            var storage = GetStorage(accountId);
            var service = new MetricService(storage);
            var updated = service.UpdateMetrics(accountId, request.Data.MaxCount);

            return new UpdateMetricsResponse()
            {
                Code = Zidium.Api.ResponseCode.Success,
                InternalData = new UpdateMetricsResponseData()
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

            var accountId = request.Token.AccountId.Value;

            var storage = GetStorage(accountId);
            var service = new MetricService(storage);
            service.EnableMetric(accountId, request.Data.MetricId);

            return new SetMetricEnableResponse()
            {
                Code = Zidium.Api.ResponseCode.Success
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

            var accountId = request.Token.AccountId.Value;

            var storage = GetStorage(accountId);
            var service = new MetricService(storage);
            service.DisableMetric(accountId, request.Data);

            return new SetMetricDisableResponse()
            {
                Code = Zidium.Api.ResponseCode.Success
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

            var accountId = request.Token.AccountId.Value;

            var storage = GetStorage(accountId);
            var service = new MetricService(storage);
            service.UpdateMetric(accountId, request.Data);

            return new UpdateMetricResponse()
            {
                Code = Zidium.Api.ResponseCode.Success
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

            var accountId = request.Token.AccountId.Value;

            var storage = GetStorage(accountId);
            var service = new MetricService(storage);
            service.UpdateMetricType(accountId, request.Data);

            return new UpdateMetricTypeResponse()
            {
                Code = Zidium.Api.ResponseCode.Success
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

            var accountId = request.Token.AccountId.Value;

            var storage = GetStorage(accountId);
            var service = new MetricService(storage);
            var metricType = service.CreateMetricType(accountId, request.Data);
            var responseData = new CreateMetricTypeResponseData()
            {
                MetricTypeId = metricType.Id
            };

            return new CreateMetricTypeResponse()
            {
                Code = Zidium.Api.ResponseCode.Success,
                InternalData = responseData
            };
        }

        public SendMetricResponse SendMetric(SendMetricRequest request)
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

            var accountId = request.Token.AccountId.Value;

            var storage = GetStorage(accountId);
            var service = new MetricService(storage);
            var metric = service.SaveMetric(accountId, request.Data);

            var metricType = AllCaches.MetricTypes.Find(
                new AccountCacheRequest()
                {
                    AccountId = metric.AccountId,
                    ObjectId = metric.MetricTypeId
                });

            var status = AllCaches.StatusDatas.Find(
                new AccountCacheRequest()
                {
                    AccountId = accountId,
                    ObjectId = metric.StatusDataId
                });

            return new SendMetricResponse()
            {
                Code = Zidium.Api.ResponseCode.Success,
                InternalData = ApiConverter.GetMetricInfo(metric, metricType, status)
            };
        }

        public GetMetricResponse GetMetric(GetMetricRequest request)
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

            var accountId = request.Token.AccountId.Value;

            var storage = GetStorage(accountId);
            var service = new MetricService(storage);
            var metric = service.GetActualMetric(accountId, request.Data.ComponentId.Value, request.Data.Name);
            var statusData = AllCaches.StatusDatas.Find(new AccountCacheRequest()
            {
                AccountId = accountId,
                ObjectId = metric.StatusDataId
            });
            var metricType = AllCaches.MetricTypes.Find(new AccountCacheRequest()
            {
                AccountId = accountId,
                ObjectId = metric.MetricTypeId
            });

            return new GetMetricResponse()
            {
                Code = Zidium.Api.ResponseCode.Success,
                InternalData = ApiConverter.GetMetricInfo(metric, metricType, statusData)
            };
        }

        public DeleteMetricResponse DeleteMetric(DeleteMetricRequest request)
        {
            CheckRequest(request);

            AuthRequest(request);

            var accountId = request.Token.AccountId.Value;

            var storage = GetStorage(accountId);
            var service = new MetricService(storage);
            service.DeleteMetric(accountId, request.Data);

            var checker = AccountLimitsCheckerManager.GetCheckerForAccount(accountId);
            checker.RefreshMetricsCount();

            return new DeleteMetricResponse()
            {
                Code = Zidium.Api.ResponseCode.Success
            };
        }

        #endregion

        #region Лог

        /// <summary>
        /// Запись лога для указанного компонента 
        /// </summary>
        /// <returns></returns>
        public SendLogResponse SendLog(SendLogRequest request)
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

            var accountId = request.Token.AccountId.Value;
            var componentId = request.Data.ComponentId.Value;

            var storage = GetStorage(accountId);
            var service = new LogService(storage);
            service.SaveLogMessage(accountId, componentId, request.Data);

            return new SendLogResponse()
            {
                Code = Zidium.Api.ResponseCode.Success
            };
        }

        public GetLogsResponse GetLogs(GetLogsRequest request)
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

            var accountId = request.Token.AccountId.Value;
            var componentId = request.Data.ComponentId.Value;

            var storage = GetStorage(accountId);
            var service = new LogService(storage);
            var rows = service.GetLogs(accountId, componentId, request.Data);
            var rowsDto = rows.Select(log =>
            {
                var properties = storage.LogProperties.GetByLogId(log.Id);
                return ApiConverter.GetLogRow(log, properties);
            }).ToArray();

            return new GetLogsResponse()
            {
                Code = Zidium.Api.ResponseCode.Success,
                InternalData = rowsDto
            };
        }

        public GetLogConfigResponse GetLogConfig(GetLogConfigRequest request)
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

            var accountId = request.Token.AccountId.Value;
            var componentId = request.Data.ComponentId.Value;

            var storage = GetStorage(accountId);
            var service = new LogService(storage);
            var logConfig = service.GetLogConfig(componentId);
            var logConfigDto = ApiConverter.GetLogConfig(logConfig);

            return new GetLogConfigResponse()
            {
                Code = Zidium.Api.ResponseCode.Success,
                InternalData = logConfigDto
            };
        }

        public SendLogsResponse SendLogs(SendLogsRequest request)
        {
            CheckRequest(request);
            if (request.Data == null)
            {
                throw new ParameterRequiredException("Request.Data");
            }

            AuthRequest(request);

            var accountId = request.Token.AccountId.Value;

            var storage = GetStorage(accountId);
            var service = new LogService(storage);

            service.SaveLogMessages(accountId, request.Data);

            return new SendLogsResponse()
            {
                Code = Zidium.Api.ResponseCode.Success
            };
        }

        public GetChangedWebLogConfigsResponse GetChangedWebLogConfigs(GetChangedWebLogConfigsRequest request)
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

            var accountId = request.Token.AccountId.Value;

            var storage = GetStorage(accountId);
            var service = new LogService(storage);

            var configs = service.GetChangedConfigs(
                accountId,
                request.Data.LastUpdateDate.Value,
                request.Data.ComponentIds);

            var response = new GetChangedWebLogConfigsResponse()
            {
                Code = Zidium.Api.ResponseCode.Success,
                InternalData = configs.Select(ApiConverter.GetLogConfig).ToArray()
            };

            return response;
        }

        #endregion

        #region Регистрация аккаунта

        public AccountRegistrationResponse AccountRegistrationStep1(AccountRegistrationStep1Request request)
        {
            CheckForLocalRequest(request);
            CheckRequest(request);

            if (request.Data == null)
            {
                throw new ParameterRequiredException("Request.Data");
            }

            AuthRequest(request);

            var data = request.Data;
            var service = _configDbServicesFactory.GetAccountManagementService();

            var reginfoId = service.RegistrationStep1(
                data.AccountName,
                data.EMail,
                data.UserAgentTag);

            return new AccountRegistrationResponse()
            {
                Code = Zidium.Api.ResponseCode.Success,
                InternalData = reginfoId
            };
        }

        public AccountRegistrationResponse AccountRegistrationStep2(AccountRegistrationStep2Request request)
        {
            CheckForLocalRequest(request);
            CheckRequest(request);

            if (request.Data == null)
            {
                throw new ParameterRequiredException("Request.Data");
            }

            AuthRequest(request);

            var data = request.Data;
            var service = _configDbServicesFactory.GetAccountManagementService();

            var reginfoId = service.RegistrationStep2(
                data.RegId,
                data.CompanyName,
                data.Site,
                data.CompanyPost);

            return new AccountRegistrationResponse()
            {
                Code = Zidium.Api.ResponseCode.Success,
                InternalData = reginfoId
            };
        }

        public AccountRegistrationResponse AccountRegistrationStep3(AccountRegistrationStep3Request request)
        {
            CheckForLocalRequest(request);
            CheckRequest(request);

            if (request.Data == null)
            {
                throw new ParameterRequiredException("Request.Data");
            }

            AuthRequest(request);

            var data = request.Data;
            var service = _configDbServicesFactory.GetAccountManagementService();

            var reginfoId = service.RegistrationStep3(
                data.RegId,
                data.FirstName,
                data.LastName,
                data.FatherName,
                data.Phone);

            return new AccountRegistrationResponse()
            {
                Code = Zidium.Api.ResponseCode.Success,
                InternalData = reginfoId
            };
        }

        public EndAccountRegistrationResponse EndAccountRegistration(EndAccountRegistrationRequest request)
        {
            CheckForLocalRequest(request);
            CheckRequest(request);

            if (request.Data == null)
            {
                throw new ParameterRequiredException("Request.SecretKey");
            }

            AuthRequest(request);

            var service = _configDbServicesFactory.GetAccountManagementService();
            var accountId = service.EndRegistration(request.Data.SecretKey);

            return new EndAccountRegistrationResponse()
            {
                Code = Zidium.Api.ResponseCode.Success,
                InternalData = accountId
            };
        }

        #endregion

        #region Типы компонентов

        public GetComponentTypeResponse GetComponentType(GetComponentTypeRequest request)
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

            var accountId = request.Token.AccountId.Value;

            var storage = GetStorage(accountId);

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

            return new GetComponentTypeResponse()
            {
                Code = Zidium.Api.ResponseCode.Success,
                InternalData = typeDto
            };
        }

        public GetOrCreateComponentTypeResponse GetOrCreateComponentType(GetOrCreateComponentTypeRequest request)
        {
            CheckRequest(request);
            if (request.Data == null)
            {
                throw new ParameterRequiredException("Request.Data");
            }

            AuthRequest(request);

            var accountId = request.Token.AccountId.Value;

            var storage = GetStorage(accountId);
            var service = new ComponentTypeService(storage);

            var type = service.GetOrCreateComponentType(accountId, request.Data);
            var typeDto = ApiConverter.GetComponentTypeInfo(type);

            return new GetOrCreateComponentTypeResponse()
            {
                Code = Zidium.Api.ResponseCode.Success,
                InternalData = typeDto
            };
        }

        public UpdateComponentTypeResponse UpdateComponentType(UpdateComponentTypeRequest request)
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

            var accountId = request.Token.AccountId.Value;

            var storage = GetStorage(accountId);
            var service = new ComponentTypeService(storage);

            service.UpdateComponentType(accountId, request.Data);
            var type = storage.ComponentTypes.GetOneById(request.Data.Id.Value);

            var typeDto = ApiConverter.GetComponentTypeInfo(type);
            return new UpdateComponentTypeResponse()
            {
                Code = Zidium.Api.ResponseCode.Success,
                InternalData = typeDto
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

            var accountId = request.Token.AccountId.Value;

            var storage = GetStorage(accountId);
            var service = new ComponentTypeService(storage);
            service.Delete(accountId, request.Data.ComponentTypeId.Value);

            return new DeleteComponentTypeResponse()
            {
                Code = Zidium.Api.ResponseCode.Success
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

            var accountId = request.Token.AccountId.Value;

            var storage = GetStorage(accountId);
            var service = new UnitTestService(storage);
            service.SetUnitTestNextTime(accountId, request.Data);

            return new SetUnitTestNextTimeResponse()
            {
                Code = Zidium.Api.ResponseCode.Success
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

            var accountId = request.Token.AccountId.Value;

            var storage = GetStorage(accountId);
            var service = new UnitTestService(storage);
            service.SetUnitTestNextStepProcessTime(accountId, request.Data);

            return new SetUnitTestNextStepProcessTimeResponse()
            {
                Code = Zidium.Api.ResponseCode.Success
            };
        }

        public GetOrCreateUnitTestTypeResponse GetOrCreateUnitTestType(GetOrCreateUnitTestTypeRequest request)
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

            var accountId = request.Token.AccountId.Value;

            var storage = GetStorage(accountId);
            var service = new UnitTestTypeService(storage);

            var unitTestType = service.GetOrCreateUnitTestType(accountId, request.Data);
            var typeDto = ApiConverter.GetUnitTestTypeInfo(unitTestType);

            return new GetOrCreateUnitTestTypeResponse()
            {
                Code = Zidium.Api.ResponseCode.Success,
                InternalData = typeDto
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

            var accountId = request.Token.AccountId.Value;

            var storage = GetStorage(accountId);
            var service = new UnitTestTypeService(storage);
            var unitTestType = service.GetUnitTestTypeById(accountId, request.Data.Id.Value);
            var typeDto = ApiConverter.GetUnitTestTypeInfo(unitTestType);

            return new GetUnitTestTypeByIdResponse()
            {
                Code = Zidium.Api.ResponseCode.Success,
                InternalData = typeDto
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

            var accountId = request.Token.AccountId.Value;

            var storage = GetStorage(accountId);
            var service = new UnitTestTypeService(storage);
            service.UpdateUnitTestType(accountId, request.Data);
            var unitTestType = service.GetUnitTestTypeById(accountId, request.Data.UnitTestTypeId.Value);
            var typeDto = ApiConverter.GetUnitTestTypeInfo(unitTestType);

            return new UpdateUnitTestTypeResponse()
            {
                Code = Zidium.Api.ResponseCode.Success,
                InternalData = typeDto
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

            var accountId = request.Token.AccountId.Value;

            var storage = GetStorage(accountId);
            var service = new UnitTestTypeService(storage);
            service.DeleteUnitTestType(accountId, request.Data.UnitTestTypeId.Value);

            return new DeleteUnitTestTypeResponse()
            {
                Code = Zidium.Api.ResponseCode.Success
            };
        }

        public GetOrCreateUnitTestResponse GetOrCreateUnitTest(GetOrCreateUnitTestRequest request)
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

            var accountId = request.Token.AccountId.Value;

            var storage = GetStorage(accountId);
            var service = new UnitTestService(storage);
            var result = service.GetOrCreateUnitTest(accountId, request.Data);

            return new GetOrCreateUnitTestResponse()
            {
                Code = Zidium.Api.ResponseCode.Success,
                InternalData = ApiConverter.GetOrCreateUnitTestResponseData(result)
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

            var accountId = request.Token.AccountId.Value;

            var storage = GetStorage(accountId);
            var service = new UnitTestService(storage);
            service.UpdateUnitTest(accountId, request.Data);

            return new UpdateUnitTestResponse()
            {
                Code = Zidium.Api.ResponseCode.Success
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

            var accountId = request.Token.AccountId.Value;
            var unitTestId = request.Data.UnitTestId.Value;

            var storage = GetStorage(accountId);
            var service = new UnitTestService(storage);
            service.Delete(accountId, unitTestId);

            return new DeleteUnitTestResponse()
            {
                Code = Zidium.Api.ResponseCode.Success
            };
        }

        public SendUnitTestResultResponse SendUnitTestResult(SendUnitTestResultRequest request)
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

            var accountId = request.Token.AccountId.Value;

            var storage = GetStorage(accountId);
            var service = new UnitTestService(storage);
            service.SendUnitTestResult(accountId, request.Data);

            return new SendUnitTestResultResponse()
            {
                Code = Zidium.Api.ResponseCode.Success,
                InternalData = null
            };
        }

        public SendUnitTestResultsResponse SendUnitTestResults(SendUnitTestResultsRequest request)
        {
            CheckRequest(request);
            if (request.Data == null)
            {
                throw new ParameterRequiredException("Request.Data");
            }

            AuthRequest(request);

            var accountId = request.Token.AccountId.Value;

            var storage = GetStorage(accountId);
            var service = new UnitTestService(storage);
            service.SendUnitTestResults(accountId, request.Data);

            return new SendUnitTestResultsResponse()
            {
                Code = Zidium.Api.ResponseCode.Success,
                InternalData = null
            };
        }

        public GetUnitTestStateResponse GetUnitTestState(GetUnitTestStateRequest request)
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

            var accountId = request.Token.AccountId.Value;

            var storage = GetStorage(accountId);
            var service = new UnitTestService(storage);
            var result = service.GetUnitTestResult(accountId, request.Data.UnitTestId.Value);
            var resultDto = ApiConverter.GetStatusDataInfo(result);

            return new GetUnitTestStateResponse()
            {
                Code = Zidium.Api.ResponseCode.Success,
                InternalData = resultDto
            };
        }

        public SendHttpUnitTestBannerResponse SendHttpUnitTestBanner(SendHttpUnitTestBannerRequest request)
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
            if (request.Data.HasBanner == null)
            {
                throw new ParameterRequiredException("Request.Data.HasBanner");
            }

            AuthRequest(request);

            var accountId = request.Token.AccountId.Value;

            var storage = GetStorage(accountId);
            var service = new UnitTestService(storage);
            var result = service.SendHttpUnitTestBanner(accountId, request.Data.UnitTestId.Value, request.Data.HasBanner.Value);

            return new SendHttpUnitTestBannerResponse()
            {
                Code = Zidium.Api.ResponseCode.Success,
                InternalData = new SendHttpUnitTestBannerResponseData()
                {
                    CanProcessUnitTest = result
                }
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

            var accountId = request.Token.AccountId.Value;
            var storage = GetStorage(accountId);
            var service = new UnitTestService(storage);
            var result = service.AddPingUnitTest(accountId, request.Data);
            var unittest = storage.UnitTests.GetOneById(result);
            var rule = storage.UnitTestPingRules.GetOneByUnitTestId(result);

            return new AddPingUnitTestResponse()
            {
                Code = Zidium.Api.ResponseCode.Success,
                InternalData = ApiConverter.AddPingUnitTestResponseData(unittest, rule)
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

            var accountId = request.Token.AccountId.Value;

            var storage = GetStorage(accountId);
            var service = new UnitTestService(storage);
            var id = service.AddHttpUnitTest(accountId, request.Data);
            var unittest = storage.UnitTests.GetOneById(id);
            var rules = storage.HttpRequestUnitTestRules.GetByUnitTestId(id);

            return new AddHttpUnitTestResponse()
            {
                Code = Zidium.Api.ResponseCode.Success,
                InternalData = ApiConverter.AddHttpUnitTestResponseData(unittest, rules)
            };
        }

        public RecalcUnitTestsResultsResponse RecalcUnitTestsResults(RecalcUnitTestsResultsRequest request)
        {
            CheckRequest(request);

            AuthRequest(request);

            var accountId = request.Token.AccountId.Value;

            var storage = GetStorage(accountId);
            var service = new UnitTestService(storage);
            var updated = service.RecalcUnitTestsResults(accountId, request.Data.MaxCount);

            return new RecalcUnitTestsResultsResponse()
            {
                Code = Zidium.Api.ResponseCode.Success,
                InternalData = new RecalcUnitTestsResultsResponseData()
                {
                    UpdateCount = updated
                }
            };
        }

        public SetUnitTestEnableResponse SetUnitTestEnable(SetUnitTestEnableRequest request)
        {
            CheckRequest(request);
            if (request.Data.UnitTestId == null)
            {
                throw new ParameterRequiredException("Request.Data.UnitTestId");
            }

            AuthRequest(request);

            var accountId = request.Token.AccountId.Value;
            var unitTestId = request.Data.UnitTestId;

            var storage = GetStorage(accountId);
            var service = new UnitTestService(storage);
            service.Enable(accountId, unitTestId.Value);

            return new SetUnitTestEnableResponse()
            {
                Code = Zidium.Api.ResponseCode.Success
            };
        }

        public SetUnitTestDisableResponse SetUnitTestDisable(SetUnitTestDisableRequest request)
        {
            CheckRequest(request);
            if (request.Data.UnitTestId == null)
            {
                throw new ParameterRequiredException("Request.Data.UnitTestId");
            }

            AuthRequest(request);

            var accountId = request.Token.AccountId.Value;

            var storage = GetStorage(accountId);
            var service = new UnitTestService(storage);
            service.Disable(accountId, request.Data);

            return new SetUnitTestDisableResponse()
            {
                Code = Zidium.Api.ResponseCode.Success
            };
        }

        #endregion

        #region Подписки

        public CreateSubscriptionResponse CreateSubscription(CreateSubscriptionRequest request)
        {
            CheckRequest(request);

            AuthRequest(request);

            var accountId = request.Token.AccountId.Value;

            var storage = GetStorage(accountId);
            var service = new SubscriptionService(storage);
            var subscription = service.CreateSubscription(accountId, request.Data);
            var subscriptionDto = ApiConverter.GetSubscriptionInfo(subscription);

            return new CreateSubscriptionResponse()
            {
                Code = Zidium.Api.ResponseCode.Success,
                InternalData = subscriptionDto
            };
        }

        public CreateUserDefaultSubscriptionResponse CreateUserDefaultSubscription(CreateUserDefaultSubscriptionRequest request)
        {
            CheckRequest(request);

            AuthRequest(request);

            var accountId = request.Token.AccountId.Value;

            var storage = GetStorage(accountId);
            var service = new SubscriptionService(storage);
            var subscription = service.CreateDefaultForUser(accountId, request.Data.UserId);
            var subscriptionDto = ApiConverter.GetSubscriptionInfo(subscription);

            return new CreateUserDefaultSubscriptionResponse()
            {
                Code = Zidium.Api.ResponseCode.Success,
                InternalData = subscriptionDto
            };
        }

        public UpdateSubscriptionResponse UpdateSubscription(UpdateSubscriptionRequest request)
        {
            CheckRequest(request);

            AuthRequest(request);

            var accountId = request.Token.AccountId.Value;

            var storage = GetStorage(accountId);
            var service = new SubscriptionService(storage);
            service.UpdateSubscription(accountId, request.Data);
            var subscription = storage.Subscriptions.GetOneById(request.Data.Id);
            var subscriptionDto = ApiConverter.GetSubscriptionInfo(subscription);

            return new UpdateSubscriptionResponse()
            {
                Code = Zidium.Api.ResponseCode.Success,
                InternalData = subscriptionDto
            };
        }

        public SetSubscriptionDisableResponse SetSubscriptionDisable(SetSubscriptionDisableRequest request)
        {
            CheckRequest(request);

            AuthRequest(request);

            var accountId = request.Token.AccountId.Value;

            var storage = GetStorage(accountId);
            var service = new SubscriptionService(storage);
            service.SetSubscriptionDisable(accountId, request.Data);

            return new SetSubscriptionDisableResponse()
            {
                Code = Zidium.Api.ResponseCode.Success
            };
        }

        public SetSubscriptionEnableResponse SetSubscriptionEnable(SetSubscriptionEnableRequest request)
        {
            CheckRequest(request);

            AuthRequest(request);

            var accountId = request.Token.AccountId.Value;

            var storage = GetStorage(accountId);
            var service = new SubscriptionService(storage);
            service.SetSubscriptionEnable(accountId, request.Data);

            return new SetSubscriptionEnableResponse()
            {
                Code = Zidium.Api.ResponseCode.Success
            };
        }

        public DeleteSubscriptionResponse DeleteSubscription(DeleteSubscriptionRequest request)
        {
            CheckRequest(request);

            AuthRequest(request);

            var accountId = request.Token.AccountId.Value;

            var storage = GetStorage(accountId);
            var service = new SubscriptionService(storage);
            service.DeleteSubscription(accountId, request.Data);

            return new DeleteSubscriptionResponse()
            {
                Code = Zidium.Api.ResponseCode.Success
            };
        }

        public SendSmsResponse SendSms(SendSmsRequest request)
        {
            CheckRequest(request);
            CheckForLocalRequest(request);

            AuthRequest(request);

            var accountId = request.Token.AccountId.Value;

            // Проверим лимиты
            var storage = GetStorage(accountId);
            var checker = AccountLimitsCheckerManager.GetCheckerForAccount(accountId);
            checker.CheckSmsPerDay(storage);

            var entity = new SendSmsCommandForAdd()
            {
                Id = Guid.NewGuid(),
                CreateDate = DateTime.Now,
                Status = SmsStatus.InQueue,
                ReferenceId = request.Data.ReferenceId,
                Body = request.Data.Body,
                Phone = request.Data.Phone
            };
            storage.SendSmsCommands.Add(entity);

            checker.AddSmsPerDay(storage);

            return new SendSmsResponse()
            {
                Code = Zidium.Api.ResponseCode.Success
            };
        }

        #endregion

        #region Статус компонента

        public GetComponentTotalStateResponse GetComponentTotalState(GetComponentTotalStateRequest request)
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

            var accountId = request.Token.AccountId.Value;
            var componentId = request.Data.ComponentId.Value;

            var storage = GetStorage(accountId);
            var recalc = request.Data.Recalc;
            var componentService = new ComponentService(storage);
            var state = componentService.GetComponentExternalState(accountId, componentId, recalc);
            var stateDto = ApiConverter.GetStatusDataInfo(state);

            var component = componentService.GetComponentById(accountId, componentId);
            stateDto.DisableComment = component.DisableComment;
            stateDto.DisableToDate = component.DisableToDate;

            return new GetComponentTotalStateResponse()
            {
                Code = Zidium.Api.ResponseCode.Success,
                InternalData = stateDto
            };
        }

        public GetComponentInternalStateResponse GetComponentInternalState(GetComponentInternalStateRequest request)
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

            var accountId = request.Token.AccountId.Value;
            var componentId = request.Data.ComponentId.Value;

            var storage = GetStorage(accountId);
            var recalc = request.Data.Recalc;
            var service = new ComponentService(storage);
            var state = service.GetComponentInternalState(accountId, componentId, recalc);
            var stateDto = ApiConverter.GetStatusDataInfo(state);

            return new GetComponentInternalStateResponse()
            {
                Code = Zidium.Api.ResponseCode.Success,
                InternalData = stateDto
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

            var accountId = request.Token.AccountId.Value;
            var componentId = request.Data.ComponentId.Value;

            var storage = GetStorage(accountId);
            var service = new ComponentService(storage);
            var component = service.CalculateAllStatuses(accountId, componentId);

            return new UpdateComponentStateResponse()
            {
                Code = Zidium.Api.ResponseCode.Success
            };
        }

        #endregion

        #region Платежи и баланс

        public AddYandexKassaPaymentResponse AddYandexKassaPayment(AddYandexKassaPaymentRequest request)
        {
            CheckForLocalRequest(request);
            CheckRequest(request);

            if (request.Data == null)
            {
                throw new ParameterRequiredException("Request.Data");
            }

            AuthRequest(request);

            var accountId = request.Token.AccountId.Value;

            var service = _configDbServicesFactory.GetPaymentService();
            service.AddYandexKassaRefillPayment(accountId, request.Data);

            return new AddYandexKassaPaymentResponse()
            {
                Code = Zidium.Api.ResponseCode.Success,
                InternalData = true
            };
        }

        public ProcessPartnerPaymentsResponse ProcessPartnerPayments(ProcessPartnerPaymentsRequest request)
        {
            CheckForLocalRequest(request);
            CheckRequest(request);

            if (request.Data == null)
            {
                throw new ParameterRequiredException("Request.Data");
            }

            AuthRequest(request);

            var service = _configDbServicesFactory.GetPaymentService();
            service.ProcessPartnerPayments(request.Data.FromDate);

            return new ProcessPartnerPaymentsResponse()
            {
                Code = Zidium.Api.ResponseCode.Success
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

            var accountId = request.Token.AccountId.Value;
            var storage = GetStorage(accountId);
            var limits = GetAccountLimitsResponseData(accountId, storage, request.Data.ArchiveDays);

            return new GetAccountLimitsResponse()
            {
                Code = Zidium.Api.ResponseCode.Success,
                InternalData = limits
            };
        }

        public GetAllAccountsLimitsResponse GetAllAccountsLimits(GetAllAccountsLimitsRequest request)
        {
            CheckForLocalRequest(request);
            CheckRequest(request);

            if (request.Data == null)
            {
                throw new ParameterRequiredException("Request.Data");
            }

            var limitsList = new List<GetAllAccountsLimitsResponseData>();

            AuthRequest(request);

            var service = _configDbServicesFactory.GetAccountService();
            var accountsIds = service.GetAccounts(new GetAccountsRequestData()).Select(t => t.Id);
            foreach (var accountId in accountsIds)
            {
                try
                {
                    var storage = GetStorage(accountId);
                    var limits = GetAccountLimitsResponseData(accountId, storage, request.Data.ArchiveDays);
                    limitsList.Add(new GetAllAccountsLimitsResponseData()
                    {
                        AccountId = accountId,
                        Limits = limits
                    });
                }
                catch { }
            }

            return new GetAllAccountsLimitsResponse()
            {
                Code = Zidium.Api.ResponseCode.Success,
                InternalData = limitsList.ToArray()
            };
        }

        // TODO Move to service
        protected GetAccountLimitsResponseData GetAccountLimitsResponseData(Guid accountId, IStorage storage, int archiveDays)
        {
            // Получим проверяльщик лимитов
            var checker = AccountLimitsCheckerManager.GetCheckerForAccount(accountId);

            // Получим максимальные лимиты
            var hardLimit = checker.GetHardTariffLimit(storage);
            var hardLimitInfo = GetTariffLimitInfo(hardLimit);
            var softLimit = checker.GetSoftTariffLimit(storage);
            var softLimitInfo = GetTariffLimitInfo(softLimit);

            // Получим использованные лимиты
            var usedToday = checker.GetUsedTodayTariffLimit(storage);
            var usedInstant = checker.GetUsedInstantTariffLimit(storage);
            var usedOverall = checker.GetUsedOverallTariffLimit(storage, archiveDays);

            // Сформируем результат
            return new GetAccountLimitsResponseData()
            {
                Hard = hardLimitInfo,
                Soft = softLimitInfo,
                UsedToday = usedToday,
                UsedInstant = usedInstant,
                UsedOverall = usedOverall
            };
        }

        public MakeAccountFreeResponse MakeAccountFree(MakeAccountFreeRequest request)
        {
            CheckForLocalRequest(request);
            CheckRequest(request);

            AuthRequest(request);

            var accountId = request.Token.AccountId.Value;

            var service = _configDbServicesFactory.GetAccountManagementService();
            service.MakeAccountFree(accountId);

            var checker = AccountLimitsCheckerManager.GetCheckerForAccount(accountId);
            checker.RefreshTariffLimit();

            return new MakeAccountFreeResponse()
            {
                Code = Zidium.Api.ResponseCode.Success
            };
        }

        public MakeAccountPaidResponse MakeAccountPaidAndSetLimits(MakeAccountPaidRequest request)
        {
            CheckForLocalRequest(request);
            CheckRequest(request);

            if (request.Data == null)
            {
                throw new ParameterRequiredException("Request.Data");
            }

            AuthRequest(request);

            var accountId = request.Token.AccountId.Value;
            var service = _configDbServicesFactory.GetAccountManagementService();
            service.MakeAccountPaidAndSetLimits(accountId, request.Data);

            var checker = AccountLimitsCheckerManager.GetCheckerForAccount(accountId);
            checker.RefreshTariffLimit();

            return new MakeAccountPaidResponse()
            {
                Code = Zidium.Api.ResponseCode.Success
            };
        }

        public SetAccountLimitsResponse SetAccountLimits(SetAccountLimitsRequest request)
        {
            CheckForLocalRequest(request);
            CheckRequest(request);

            if (request.Data == null)
            {
                throw new ParameterRequiredException("Request.Data");
            }

            AuthRequest(request);

            var accountId = request.Token.AccountId.Value;
            var service = _configDbServicesFactory.GetAccountManagementService();
            service.SetAccountLimits(accountId, request.Data.Limits, request.Data.Type);

            var checker = AccountLimitsCheckerManager.GetCheckerForAccount(accountId);
            checker.RefreshTariffLimit();

            return new SetAccountLimitsResponse()
            {
                Code = Zidium.Api.ResponseCode.Success
            };
        }

        // TODO Move to service
        protected AccountTotalLimitsDataInfo GetTariffLimitInfo(TariffLimitForRead tariffLimit)
        {
            var result = new AccountTotalLimitsDataInfo()
            {
                EventsMaxDays = tariffLimit.EventsMaxDays,
                LogMaxDays = tariffLimit.LogMaxDays,
                MetricsMaxDays = tariffLimit.MetricsMaxDays,
                EventRequestsPerDay = tariffLimit.EventsRequestsPerDay,
                LogSizePerDay = tariffLimit.LogSizePerDay,
                ComponentsMax = tariffLimit.ComponentsMax,
                ComponentTypesMax = tariffLimit.ComponentTypesMax,
                UnitTestTypesMax = tariffLimit.UnitTestTypesMax,
                HttpChecksMaxNoBanner = tariffLimit.HttpUnitTestsMaxNoBanner,
                UnitTestsMax = tariffLimit.UnitTestsMax,
                UnitTestsRequestsPerDay = tariffLimit.UnitTestsRequestsPerDay,
                MetricsMax = tariffLimit.MetricsMax,
                MetricRequestsPerDay = tariffLimit.MetricsRequestsPerDay,
                UnitTestsMaxDays = tariffLimit.UnitTestsMaxDays,
                StorageSizeMax = tariffLimit.StorageSizeMax,
                SmsPerDay = tariffLimit.SmsPerDay
            };
            return result;
        }

        #endregion

        #region Аккаунты

        public GetAccountByIdResponse GetAccountById(GetAccountByIdRequest request)
        {
            CheckForLocalRequest(request);
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

            var accountInfo = _configDbServicesFactory.GetAccountService().GetOneById(request.Data.Id.Value);

            return new GetAccountByIdResponse()
            {
                Code = Zidium.Api.ResponseCode.Success,
                InternalData = accountInfo
            };
        }

        public GetAccountsResponse GetAccounts(GetAccountsRequest request)
        {
            CheckForLocalRequest(request);
            CheckRequest(request);

            AuthRequest(request);

            var accountInfos = _configDbServicesFactory.GetAccountService().GetAccounts(request.Data);

            return new GetAccountsResponse()
            {
                Code = Zidium.Api.ResponseCode.Success,
                InternalData = accountInfos
            };
        }

        public UpdateAccountResponse UpdateAccount(UpdateAccountRequest request)
        {
            CheckForLocalRequest(request);
            CheckRequest(request);

            AuthRequest(request);

            var service = _configDbServicesFactory.GetAccountService();
            service.Update(request.Data);
            var accountInfo = service.GetOneById(request.Data.Id);

            return new UpdateAccountResponse()
            {
                Code = Zidium.Api.ResponseCode.Success,
                InternalData = accountInfo
            };
        }

        public ChangeApiKeyResponse ChangeApiKey(ChangeApiKeyRequest request)
        {
            CheckForLocalRequest(request);
            CheckRequest(request);

            AuthRequest(request);

            var newKey = _configDbServicesFactory.GetAccountService().ChangeApiKey(request.Data);

            return new ChangeApiKeyResponse()
            {
                Code = Zidium.Api.ResponseCode.Success,
                InternalData = newKey
            };
        }

        #endregion

        #region Базы данных

        public GetDatabaseByIdResponse GetDatabaseById(GetDatabaseByIdRequest request)
        {
            CheckForLocalRequest(request);
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

            var databaseInfo = _configDbServicesFactory.GetDatabaseService().GetOneById(request.Data.Id.Value);

            return new GetDatabaseByIdResponse()
            {
                Code = Zidium.Api.ResponseCode.Success,
                InternalData = databaseInfo
            };
        }

        public GetDatabasesResponse GetDatabases(GetDatabasesRequest request)
        {
            CheckForLocalRequest(request);
            CheckRequest(request);

            AuthRequest(request);

            var databaseInfos = _configDbServicesFactory.GetDatabaseService().GetDatabases();

            return new GetDatabasesResponse()
            {
                Code = Zidium.Api.ResponseCode.Success,
                InternalData = databaseInfos
            };
        }

        public SetDatabaseIsBrokenResponse SetDatabaseIsBroken(SetDatabaseIsBrokenRequest request)
        {
            CheckForLocalRequest(request);
            CheckRequest(request);

            if (request.Data == null)
            {
                throw new ParameterRequiredException("Request.Data");
            }

            if (request.Data.Id == null)
            {
                throw new ParameterRequiredException("Request.Data.Id");
            }

            if (request.Data.IsBroken == null)
            {
                throw new ParameterRequiredException("Request.Data.IsBroken");
            }

            AuthRequest(request);

            var service = _configDbServicesFactory.GetDatabaseService();
            service.SetIsBroken(request.Data.Id.Value, request.Data.IsBroken.Value);

            return new SetDatabaseIsBrokenResponse()
            {
                Code = Zidium.Api.ResponseCode.Success
            };
        }

        #endregion
    }
}
