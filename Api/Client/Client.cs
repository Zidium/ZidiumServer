using System;
using System.Diagnostics;
using System.Threading;
using Zidium.Api.Dto;
using Zidium.Api.Logs;
using Zidium.Api.Others;
using Zidium.Api.XmlConfig;

namespace Zidium.Api
{
    public class Client : IClient
    {
        private static IClient _instance;
        private IComponentControl _rootComponentControl;
        private IComponentControl _defaultComponentControl;

        public static IClient Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (typeof(Client))
                    {
                        if (_instance == null)
                        {
                            _instance = new Client();
                        }
                    }
                }
                return _instance;
            }
            set { _instance = value; }
        }

        public bool Disable
        {
            get { return Config.Access.Disable; }
            set { Config.Access.Disable = value; }
        }

        private ResponseInfo _lastResponse;

        protected PrepareDataHelper PrepareDataHelper;

        private ApiServiceWrapper _apiService;

        public IApiService ApiService
        {
            get { return GetApiService(); }
        }

        protected IApiService GetApiService()
        {
            lock (this)
            {
                // проверим нужно ли создавать новый сервис
                var service = _apiService;

                if (service != null)
                {
                    return service;
                }

                // если не заполнен url, то вернет заглушку
                if (string.IsNullOrEmpty(Config.Access.Url))
                {
                    return new FakeApiService(ResponseCode.EmptyUrl, "Не указан Url", AccessToken);
                }

                // создаем новый сервис
                service = CreateApiService(AccessToken);
                _apiService = service;
                return service;
            }
        }

        public IApiService SetApiService(IApiService service)
        {
            if (service == null)
            {
                throw new ArgumentNullException("service");
            }
            service.SetAccessToken(AccessToken);
            var wrapper = new ApiServiceWrapper(this, service);
            _apiService = wrapper;
            return wrapper;
        }

        protected ApiServiceWrapper CreateApiService(AccessTokenDto accessToken)
        {
            try
            {
                var dtoService = CreateDtoService();
                var apiService = new ApiService(dtoService, accessToken);
                return new ApiServiceWrapper(this, apiService);
            }
            catch (Exception exception)
            {
                InternalLog.Error("Не удалось настроить веб-сервис", exception);

                // отдадим заглушку
                var apiService = new FakeApiService();
                apiService.SetAccessToken(accessToken);
                return new ApiServiceWrapper(this, apiService);
            }
        }

        protected DtoServiceProxy CreateDtoService()
        {
            Uri apiUrl;
            try
            {
                apiUrl = new Uri(Config.Access.Url);
            }
            catch (UriFormatException)
            {
                throw new ResponseCodeException(ResponseCode.ApiUrlFormatError, "Неверный формат адреса Api: " + Config.Access.Url);
            }

            InternalLog.Info("Url: " + apiUrl.AbsoluteUri);

            var dtoService = new DtoServiceProxy(apiUrl);
            return dtoService;
        }

        public AccessTokenDto AccessToken { get; protected set; }

        public IEventManager EventManager { get; set; }

        private Config _config = new Config();

        public Config Config
        {
            get { return _config; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }
                _config = value;
            }
        }

        internal IInternalLog InternalLog { get; private set; }

        internal void SetLastResponse(ResponseInfo response)
        {
            _lastResponse = response;
        }

        public ResponseInfo LastResponse
        {
            get { return _lastResponse; }
        }

        public bool CanSendData
        {
            get
            {
                if (Disable)
                {
                    return false;
                }
                var lastResponse = _lastResponse;
                if (lastResponse == null || lastResponse.Response.Success)
                {
                    return true;
                }
                var waitOnError = Config.Access.WaitOnError;
                if (waitOnError == TimeSpan.Zero)
                {
                    return true;
                }
                var nextTime = lastResponse.Date + waitOnError;
                return nextTime <= DateTime.Now;
            }
        }

        protected void InitInternal(Config config)
        {
            if (config == null)
            {
                throw new ArgumentNullException("config");
            }
            Config = config;

            var programName = Tools.GetProgramName();
            AccessToken = new AccessTokenDto()
            {
                SecretKey = config.Access.SecretKey,
                Program = programName
            };

            InternalLog = new InternalLog(this);
            PrepareDataHelper = new PrepareDataHelper(this);

            GetFolderComponentTypeControl(); // чтобы заполнить кэш

            WebLogManager = new WebLogManager(this);
            EventManager = new EventManager(this);

            Start();
        }

        protected virtual IDtoService CreateServiceProxy(Uri uri)
        {
            return new DtoServiceProxy(uri);
        }

        private IExceptionRender _exceptionRender = new ExceptionRender();

        public IExceptionRender ExceptionRender
        {
            get { return _exceptionRender; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }
                _exceptionRender = value;
            }
        }

        public IEventPreparer EventPreparer { get; set; }

        #region Конструкторы

        public Client()
        {
            var config = ConfigHelper.LoadFromXmlOrGetDefault();
            InitInternal(config);
        }

        public Client(string url, string secretKey)
        {
            if (url == null)
            {
                throw new ArgumentNullException("accountName");
            }
            if (secretKey == null)
            {
                throw new ArgumentNullException("secretKey");
            }
            var config = ConfigHelper.LoadFromXmlOrGetDefault();
            config.Access.Url = url;
            config.Access.SecretKey = secretKey;
            InitInternal(config);
        }

        public Client(Config config)
        {
            if (config == null)
            {
                throw new ArgumentNullException("config");
            }
            InitInternal(config);
        }

        public Client(IApiService service, Config config)
        {
            if (service == null)
            {
                throw new ArgumentNullException("service");
            }
            if (config == null)
            {
                throw new ArgumentNullException("config");
            }
            InitInternal(config);
            SetApiService(service);
        }

        #endregion

        #region Разное

        public bool CheckConnection()
        {
            var echo = Guid.NewGuid().ToString();
            var response = ApiService.GetEcho(echo);
            return response.Success && response.GetDataAndCheck() == echo;
        }

        public void WaitUntilAvailable(TimeSpan timeout)
        {
            var stopWatch = Stopwatch.StartNew();
            while (stopWatch.Elapsed < timeout)
            {
                if (CheckConnection())
                    return;
                Thread.Sleep(1000);
            }
            throw new TimeoutException($"Zidium API server is still unavailable after {timeout}");
        }

        public void Flush()
        {
            WebLogManager.Flush();
            EventManager.Flush();
        }

        protected void Start()
        {
            WebLogManager.Start();
            EventManager.Start();
        }

        protected void Stop()
        {
            WebLogManager.Stop();
            EventManager.Stop();
        }

        protected virtual void ThrowException(ResponseDto response)
        {
            if (response.Success == false)
            {
                throw new ResponseException(response);
            }
        }

        protected T GetParametrNullResponse<T>(string parameterName) where T : ResponseDto, new()
        {
            return new T()
            {
                Code = ResponseCode.ClientError,
                ErrorMessage = "Не задан параметр: " + parameterName
            };
        }

        protected T GetClientOfflineResponse<T>() where T : ResponseDto, new()
        {
            return new T()
            {
                Code = ResponseCode.ClientError,
                ErrorMessage = "Клиент отключен, установите свойство Enable = true"
            };
        }

        protected T GetClientErrorResponse<T>(Exception exception) where T : ResponseDto, new()
        {
            return new T()
            {
                Code = ResponseCode.ClientError,
                ErrorMessage = exception.Message
            };
        }

        #endregion

        #region Контролы компонентов

        public IComponentControl GetRootComponentControl()
        {
            if (_rootComponentControl == null)
            {
                lock (typeof(Client))
                {
                    if (_rootComponentControl == null)
                    {
                        _rootComponentControl = ComponentControlWrapper.CreateRoot(this);
                    }
                }
            }
            return _rootComponentControl;
        }

        public IComponentControl GetDefaultComponentControl()
        {
            if (_defaultComponentControl == null)
            {
                lock (typeof(Client))
                {
                    if (_defaultComponentControl == null)
                    {
                        _defaultComponentControl = ComponentControlWrapper.GetDefault(this);
                    }
                }
            }
            return _defaultComponentControl;
        }

        public IComponentControl GetComponentControl(Guid id)
        {
            var wrapper = ComponentControlWrapper.GetById(this, id);
            //todo не понятно как вести кэш по ИД
            //return ComponentCache.Add(wrapper);
            return wrapper;
        }

        #endregion

        #region События

        internal SendEventResponseDto SendEventWrapper(SendEventBase sendEventBase)
        {
            if (sendEventBase == null)
            {
                throw new ArgumentNullException("sendEventBase");
            }
            if (sendEventBase.Ignore)
            {
                return ResponseHelper.GetClientErrorResponse<SendEventResponseDto>("Событие проигнорировано");
            }

            PrepareDataHelper.PrepareEvent(sendEventBase);

            if (sendEventBase.IsServerTime == false)
            {
                SetServerTime(sendEventBase);
            }

            var data = new SendEventRequestDataDto()
            {
                Category = sendEventBase.EventCategory,
                ComponentId = sendEventBase.ComponentControl.Info.Id,
                Count = sendEventBase.Count,
                Importance = sendEventBase.Importance,
                JoinIntervalSeconds = sendEventBase.JoinInterval?.TotalSeconds,
                JoinKey = sendEventBase.JoinKey,
                Message = sendEventBase.Message,
                StartDate = sendEventBase.StartDate,
                TypeCode = sendEventBase.TypeCode,
                TypeDisplayName = sendEventBase.TypeDisplayName,
                TypeSystemName = sendEventBase.TypeSystemName,
                Version = sendEventBase.Version,
                Properties = sendEventBase.Properties?.ToDtoList()
            };

            return ApiService.SendEvent(data);
        }

        #endregion

        #region Лог

        public IWebLogManager WebLogManager { get; set; }

        #endregion

        #region Типы компонентов

        public IComponentTypeControl GetOrCreateComponentTypeControl(string systemName)
        {
            if (systemName == null)
            {
                throw new ArgumentNullException("systemName");
            }
            var data = new GetOrCreateComponentTypeRequestDataDto() { SystemName = systemName };
            return GetOrCreateComponentTypeControl(data);
        }

        public IComponentTypeControl GetOrCreateComponentTypeControl(GetOrCreateComponentTypeRequestDataDto data)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }
            return new ComponentTypeControlWrapper(this, data.SystemName, data);
        }

        public IComponentTypeControl GetFolderComponentTypeControl()
        {
            var typeDto = new ComponentTypeDto()
            {
                SystemName = SystemComponentType.Folder.SystemName,
                DisplayName = SystemComponentType.Folder.SystemName,
                Id = SystemComponentType.Folder.Id,
                IsSystem = true,
                IsFolder = true,
                IsRoot = false
            };

            var control = new ComponentTypeControlOnline(this, typeDto);

            return control;
        }

        #endregion

        #region Типы проверок

        public IUnitTestTypeControl GetOrCreateUnitTestTypeControl(GetOrCreateUnitTestTypeRequestDataDto data)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }
            return new UnitTestTypeControlWrapper(this, data.SystemName, data);
        }

        public IUnitTestTypeControl GetOrCreateUnitTestTypeControl(string systemName)
        {
            if (systemName == null)
            {
                throw new ArgumentNullException("systemName");
            }
            var data = new GetOrCreateUnitTestTypeRequestDataDto() { SystemName = systemName };
            return GetOrCreateUnitTestTypeControl(data);
        }

        public IUnitTestTypeControl GetOrCreateUnitTestTypeControl(string systemName, string displayName)
        {
            if (systemName == null)
            {
                throw new ArgumentNullException("systemName");
            }
            var data = new GetOrCreateUnitTestTypeRequestDataDto()
            {
                SystemName = systemName,
                DisplayName = displayName
            };
            return GetOrCreateUnitTestTypeControl(data);
        }

        #endregion

        #region Время

        public TimeSpan? TimeDifference { get; set; }

        protected void SetServerTime(SendEventBase eventData)
        {
            if (eventData.StartDate == null)
            {
                eventData.StartDate = DateTime.Now;
            }
            if (CanConvertToServerDate())
            {
                eventData.StartDate = ToServerTime(eventData.StartDate.Value);
            }
            eventData.IsServerTime = true;
        }

        public void CalculateServerTimeDifference()
        {
            var diff = GetServerTimeDifference();
            if (diff.HasValue)
            {
                TimeDifference = diff;
            }
        }

        protected TimeSpan? GetServerTimeDifference()
        {
            var response = ApiService.GetServerTime();
            if (response.Success)
            {
                return response.GetDataAndCheck().Date - DateTime.Now;
            }
            return null;
        }

        public bool CanConvertToServerDate()
        {
            if (TimeDifference.HasValue)
            {
                return true;
            }
            if (CanSendData)
            {
                CalculateServerTimeDifference();
            }
            return TimeDifference.HasValue;
        }

        public virtual DateTime ToServerTime(DateTime date)
        {
            if (CanConvertToServerDate())
            {
                return date + (TimeDifference ?? TimeSpan.Zero);
            }
            return date;
        }

        #endregion

    }
}
