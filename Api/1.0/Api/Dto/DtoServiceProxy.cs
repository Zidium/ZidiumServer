using System;

namespace Zidium.Api.Dto
{
    public class DtoServiceProxy : IDtoService
    {
        public Uri Uri { get; protected set; }

        private ISerializer _serializer = null;

        public ISerializer Serializer
        {
            get { return _serializer; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }
                _serializer = value;
            }
        }

        public DtoServiceProxy(Uri uri)
        {
            if (uri == null)
            {
                throw new ArgumentNullException("uri");
            }
            Uri = uri;

#if !PocketPC
            _serializer = new JsonSerializer();
#else
            _serializer = new XmlSerializer();
#endif
        }

        // метод - обертка всех вызовов веб-сервиса
        protected TResponse ExecuteAction<TResponse>(string action, Request requestObj)
            where TResponse : Response, new()
        {
            if (action == null)
            {
                throw new ArgumentNullException("action");
            }
            try
            {
                if (requestObj != null)
                {
                    if (requestObj.Token == null)
                    {
                        throw new ArgumentException("AccessToken is NULL");
                    }
                    if (string.IsNullOrEmpty(requestObj.Token.SecretKey))
                    {
                        return new TResponse()
                        {
                            Code = ResponseCode.EmptySecretKey,
                            ErrorMessage = "Укажите SecretKey"
                        };
                    }
                }
                var proxy = new WebServiceProxyHelper();
                var handlerUrl = Uri.AbsoluteUri;
                return proxy.ExecuteAction<TResponse>(handlerUrl, action, Serializer, requestObj);
            }
            catch (Exception exception)
            {
                var response = new TResponse
                {
                    Code = ResponseCode.ClientError,
                    ErrorMessage = exception.Message
                };
                return response;
            }
        }

        // метод - обертка всех вызовов веб-сервиса
        protected TResponse ExecuteAction<TResponse>(string action)
            where TResponse : Response, new()
        {
            try
            {
                if (action == null)
                {
                    throw new ArgumentNullException("action");
                }
                return ExecuteAction<TResponse>(action, null);
            }
            catch (Exception exception)
            {
                var response = new TResponse
                {
                    Code = ResponseCode.ClientError,
                    ErrorMessage = exception.Message
                };
                return response;
            }
        }


        #region Разное

        public GetEchoResponseDto GetEcho(GetEchoRequestDto requestDto)
        {
            return ExecuteAction<GetEchoResponseDto>("GetEcho", requestDto);
        }

        public GetServerTimeResponseDto GetServerTime()
        {
            return ExecuteAction<GetServerTimeResponseDto>("GetServerTime");
        }

        #endregion

        #region контролы

        public GetRootControlDataResponseDto GetRootControlData(GetRootControlDataRequestDto request)
        {
            return ExecuteAction<GetRootControlDataResponseDto>("GetRootControlData", request);
        }

        #endregion

        #region Компоненты и папки

        public GetRootComponentResponseDto GetRootComponent(GetRootComponentRequestDto request)
        {
            return ExecuteAction<GetRootComponentResponseDto>("GetRootComponent", request);
        }

        public GetOrAddComponentResponseDto GetOrAddComponent(GetOrAddComponentRequestDto request)
        {
            return ExecuteAction<GetOrAddComponentResponseDto>("GetOrAddComponent", request);
        }

        public GetComponentByIdResponseDto GetComponentById(GetComponentByIdRequestDto request)
        {
            return ExecuteAction<GetComponentByIdResponseDto>("GetComponentById", request);
        }

        public GetComponentBySystemNameResponseDto GetComponentBySystemName(GetComponentBySystemNameRequestDto request)
        {
            return ExecuteAction<GetComponentBySystemNameResponseDto>("GetComponentBySystemName", request);
        }

        public GetChildComponentsResponseDto GetChildComponents(GetChildComponentsRequestDto request)
        {
            return ExecuteAction<GetChildComponentsResponseDto>("GetChildComponents", request);
        }

        public GetOrCreateComponentResponseDto GetOrCreateComponent(GetOrCreateComponentRequestDto request)
        {
            return ExecuteAction<GetOrCreateComponentResponseDto>("GetOrCreateComponent", request);
        }

        public GetComponentControlByIdResponseDto GetComponentControlById(GetComponentControlByIdRequestDto request)
        {
            return ExecuteAction<GetComponentControlByIdResponseDto>("GetComponentControlById", request);
        }

        public UpdateComponentResponseDto UpdateComponent(UpdateComponentRequestDto request)
        {
            return ExecuteAction<UpdateComponentResponseDto>("UpdateComponent", request);
        }

        public SetComponentEnableResponseDto SetComponentEnable(SetComponentEnableRequestDto request)
        {
            return ExecuteAction<SetComponentEnableResponseDto>("SetComponentEnable", request);
        }

        public SetComponentDisableResponseDto SetComponentDisable(SetComponentDisableRequestDto request)
        {
            return ExecuteAction<SetComponentDisableResponseDto>("SetComponentDisable", request);
        }

        public DeleteComponentResponseDto DeleteComponent(DeleteComponentRequestDto request)
        {
            return ExecuteAction<DeleteComponentResponseDto>("DeleteComponent", request);
        }

        #endregion

        #region События

        public SendEventResponseDto SendEvent(SendEventRequestDto request)
        {
            return ExecuteAction<SendEventResponseDto>("SendEvent", request);
        }

        public JoinEventsResponseDto JoinEvents(JoinEventsRequestDto request)
        {
            return ExecuteAction<JoinEventsResponseDto>("JoinEvents", request);
        }

        public GetEventByIdResponseDto GetEventById(GetEventByIdRequestDto request)
        {
            return ExecuteAction<GetEventByIdResponseDto>("GetEventById", request);
        }

        public GetEventsResponseDto GetEvents(GetEventsRequestDto request)
        {
            return ExecuteAction<GetEventsResponseDto>("GetEvents", request);
        }

        #endregion

        #region Метрики

        public SendMetricsResponseDto SendMetrics(SendMetricsRequestDto request)
        {
            return ExecuteAction<SendMetricsResponseDto>("SendMetrics", request);
        }

        public GetMetricsHistoryResponseDto GetMetricsHistory(GetMetricsHistoryRequestDto request)
        {
            return ExecuteAction<GetMetricsHistoryResponseDto>("GetMetricsHistory", request);
        }

        public GetMetricsResponseDto GetMetrics(GetMetricsRequestDto request)
        {
            return ExecuteAction<GetMetricsResponseDto>("GetMetrics", request);
        }

        public SetUnitTestDisableResponseDto SetUnitTestDisable(SetUnitTestDisableRequestDto request)
        {
            return ExecuteAction<SetUnitTestDisableResponseDto>("SetUnitTestDisable", request);
        }

        public SendMetricResponseDto SendMetric(SendMetricRequestDto request)
        {
            return ExecuteAction<SendMetricResponseDto>("SendMetric", request);
        }

        public GetMetricResponseDto GetMetric(GetMetricRequestDto request)
        {
            return ExecuteAction<GetMetricResponseDto>("GetMetric", request);
        }

        #endregion

        #region Лог

        public SendLogResponseDto SendLog(SendLogRequestDto request)
        {
            return ExecuteAction<SendLogResponseDto>("SendLog", request);
        }

        public GetLogsResponseDto GetLogs(GetLogsRequestDto request)
        {
            return ExecuteAction<GetLogsResponseDto>("GetLogs", request);
        }

        public GetLogConfigResponseDto GetLogConfig(GetLogConfigRequestDto request)
        {
            return ExecuteAction<GetLogConfigResponseDto>("GetLogConfig", request);
        }

        public GetChangedWebLogConfigsResponseDto GetChangedWebLogConfigs(GetChangedWebLogConfigsRequestDto request)
        {
            return ExecuteAction<GetChangedWebLogConfigsResponseDto>("GetChangedWebLogConfigs", request);
        }

        public SendLogsResponseDto SendLogs(SendLogsRequestDto request)
        {
            return ExecuteAction<SendLogsResponseDto>("SendLogs", request);
        }

        #endregion

        #region Типы компонентов

        public GetOrCreateComponentTypeResponseDto GetOrCreateComponentType(GetOrCreateComponentTypeRequestDto request)
        {
            return ExecuteAction<GetOrCreateComponentTypeResponseDto>("GetOrCreateComponentType", request);
        }

        public GetComponentTypeResponseDto GetComponentType(GetComponentTypeRequestDto request)
        {
            return ExecuteAction<GetComponentTypeResponseDto>("GetComponentTypeBySystemName", request);
        }

        public UpdateComponentTypeResponseDto UpdateComponentType(UpdateComponentTypeRequestDto request)
        {
            return ExecuteAction<UpdateComponentTypeResponseDto>("UpdateComponentType", request);
        }

        #endregion

        #region Юнит-тесты

        public SetUnitTestEnableResponseDto SetUnitTestEnable(SetUnitTestEnableRequestDto request)
        {
            return ExecuteAction<SetUnitTestEnableResponseDto>("SetUnitTestEnable", request);
        }

        public GetOrCreateUnitTestTypeResponseDto GetOrCreateUnitTestType(GetOrCreateUnitTestTypeRequestDto request)
        {
            return ExecuteAction<GetOrCreateUnitTestTypeResponseDto>("GetOrCreateUnitTestType", request);
        }

        public GetOrCreateUnitTestResponseDto GetOrCreateUnitTest(GetOrCreateUnitTestRequestDto request)
        {
            return ExecuteAction<GetOrCreateUnitTestResponseDto>("GetOrCreateUnitTest", request);
        }

        public SendUnitTestResultResponseDto SendUnitTestResult(SendUnitTestResultRequestDto request)
        {
            return ExecuteAction<SendUnitTestResultResponseDto>("SendUnitTestResult", request);
        }

        public GetUnitTestStateResponseDto GetUnitTestState(GetUnitTestStateRequestDto request)
        {
            return ExecuteAction<GetUnitTestStateResponseDto>("GetUnitTestState", request);
        }

        #endregion

        #region Статус компонента

        public GetComponentTotalStateResponseDto GetComponentTotalState(GetComponentTotalStateRequestDto request)
        {
            return ExecuteAction<GetComponentTotalStateResponseDto>("GetComponentTotalState", request);
        }

        public GetComponentInternalStateResponseDto GetComponentInternalState(GetComponentInternalStateRequestDto request)
        {
            return ExecuteAction<GetComponentInternalStateResponseDto>("GetComponentInternalState", request);
        }

        #endregion
    }
}
