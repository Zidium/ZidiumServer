using System;
using System.Runtime.CompilerServices;
using Zidium.Api.Dto;

namespace Zidium.Core.Api
{
    /// <summary>
    /// Это класс-прокси для вызова методов веб-сервиса Диспетчера.
    /// </summary>
    public class DispatcherServiceProxy : IDispatcherService
    {
        public static string LastInvalidResponse { get; set; }

        public DispatcherServiceProxy(Uri uri)
        {
            if (uri == null)
            {
                throw new ArgumentNullException("uri");
            }
            Uri = uri;
        }

        private Uri _uri;

        public Uri Uri
        {
            get { return _uri; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }
                _uri = new Uri(value.AbsoluteUri.TrimEnd('/'));
            }
        }

        // TODO Can be static?
        private ISerializer _serializer = new JsonSerializer();

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

        protected TResponse ExecuteHttp<TResponse>(RequestDto requestObj, string action)
             where TResponse : ResponseDto, new()
        {
            if (action == null)
            {
                throw new ArgumentNullException("action");
            }
            try
            {
                if (requestObj != null && requestObj.Token == null)
                {
                    throw new ArgumentException("Token is NULL");
                }
                var handlerUrl = Uri.AbsoluteUri;
                return WebServiceProxyHelper.ExecuteAction<TResponse>(handlerUrl, action, Serializer, requestObj);
            }
            catch (Exception exception)
            {
                var response = new TResponse
                {
                    Code = ResponseCode.ServerOffline,
                    ErrorMessage = exception.Message
                };
                return response;
            }
        }

        // метод - обертка всех вызовов веб-сервиса
        protected TResponse ExecuteAction<TResponse>(RequestDto requestObj, [CallerMemberName] string action = null)
            where TResponse : ResponseDto, new()
        {
            try
            {
                if (action == null)
                {
                    throw new ArgumentNullException("action");
                }
                if (requestObj != null && requestObj.Token == null)
                {
                    throw new ArgumentException("requestObj.Token is NULL");
                }
                return ExecuteHttp<TResponse>(requestObj, action);
            }
            catch (Exception exception)
            {
                var response = new TResponse
                {
                    Code = ResponseCode.ServerOffline,
                    ErrorMessage = exception.Message
                };
                return response;
            }
        }

        public GetEchoResponseDto GetEcho(GetEchoRequestDto request)
        {
            return ExecuteAction<GetEchoResponseDto>(request);
        }

        public GetComponentControlByIdResponseDto GetComponentControlById(GetComponentControlByIdRequestDto request)
        {
            return ExecuteAction<GetComponentControlByIdResponseDto>(request);
        }

        public GetOrAddComponentResponseDto GetOrAddComponent(GetOrAddComponentRequestDto request)
        {
            return ExecuteAction<GetOrAddComponentResponseDto>(request);
        }

        public GetRootComponentResponseDto GetRootComponent(GetRootComponentRequestDto request)
        {
            return ExecuteAction<GetRootComponentResponseDto>(request);
        }

        public GetComponentByIdResponseDto GetComponentById(GetComponentByIdRequestDto request)
        {
            return ExecuteAction<GetComponentByIdResponseDto>(request);
        }

        public GetComponentBySystemNameResponseDto GetComponentBySystemName(GetComponentBySystemNameRequestDto request)
        {
            return ExecuteAction<GetComponentBySystemNameResponseDto>(request);
        }

        public CreateComponentResponse CreateComponent(CreateComponentRequest request)
        {
            return ExecuteAction<CreateComponentResponse>(request);
        }

        public GetOrCreateComponentResponseDto GetOrCreateComponent(GetOrCreateComponentRequestDto request)
        {
            return ExecuteAction<GetOrCreateComponentResponseDto>(request);
        }

        public UpdateComponentResponseDto UpdateComponent(UpdateComponentRequestDto request)
        {
            return ExecuteAction<UpdateComponentResponseDto>(request);
        }

        public GetChildComponentsResponseDto GetChildComponents(GetChildComponentsRequestDto request)
        {
            return ExecuteAction<GetChildComponentsResponseDto>(request);
        }

        public GetComponentAndChildIdsResponse GetComponentAndChildIds(GetComponentAndChildIdsRequest request)
        {
            return ExecuteAction<GetComponentAndChildIdsResponse>(request);
        }

        public GetComponentTypeResponseDto GetComponentType(GetComponentTypeRequestDto request)
        {
            return ExecuteAction<GetComponentTypeResponseDto>(request);
        }

        public GetOrCreateComponentTypeResponseDto GetOrCreateComponentType(GetOrCreateComponentTypeRequestDto request)
        {
            return ExecuteAction<GetOrCreateComponentTypeResponseDto>(request);
        }

        public UpdateComponentTypeResponseDto UpdateComponentType(UpdateComponentTypeRequestDto request)
        {
            return ExecuteAction<UpdateComponentTypeResponseDto>(request);
        }

        public DeleteComponentTypeResponse DeleteComponentType(DeleteComponentTypeRequest request)
        {
            return ExecuteAction<DeleteComponentTypeResponse>(request);
        }

        public SendEventResponseDto SendEvent(SendEventRequestDto request)
        {
            return ExecuteAction<SendEventResponseDto>(request);
        }

        public JoinEventsResponseDto JoinEvents(JoinEventsRequestDto request)
        {
            return ExecuteAction<JoinEventsResponseDto>(request);
        }

        public UpdateEventTypeResponse UpdateEventType(UpdateEventTypeRequest request)
        {
            return ExecuteAction<UpdateEventTypeResponse>(request);
        }

        public CreateMetricResponse CreateMetric(CreateMetricRequest request)
        {
            return ExecuteAction<CreateMetricResponse>(request);
        }

        public GetEventByIdResponseDto GetEventById(GetEventByIdRequestDto request)
        {
            return ExecuteAction<GetEventByIdResponseDto>(request);
        }

        public GetEventsResponseDto GetEvents(GetEventsRequestDto request)
        {
            return ExecuteAction<GetEventsResponseDto>(request);
        }

        public SendMetricsResponseDto SendMetrics(SendMetricsRequestDto request)
        {
            return ExecuteAction<SendMetricsResponseDto>(request);
        }

        public GetMetricsResponseDto GetMetrics(GetMetricsRequestDto request)
        {
            return ExecuteAction<GetMetricsResponseDto>(request);
        }

        public GetMetricsHistoryResponseDto GetMetricsHistory(GetMetricsHistoryRequestDto request)
        {
            return ExecuteAction<GetMetricsHistoryResponseDto>(request);
        }

        public DeleteMetricResponse DeleteMetric(DeleteMetricRequest request)
        {
            return ExecuteAction<DeleteMetricResponse>(request);
        }

        public DeleteMetricTypeResponse DeleteMetricType(DeleteMetricTypeRequest typeRequest)
        {
            return ExecuteAction<DeleteMetricTypeResponse>(typeRequest);
        }

        public SetMetricDisableResponse SetMetricDisable(SetMetricDisableRequest request)
        {
            return ExecuteAction<SetMetricDisableResponse>(request);
        }

        public UpdateMetricResponse UpdateMetric(UpdateMetricRequest request)
        {
            return ExecuteAction<UpdateMetricResponse>(request);
        }

        public UpdateMetricTypeResponse UpdateMetricType(UpdateMetricTypeRequest request)
        {
            return ExecuteAction<UpdateMetricTypeResponse>(request);
        }

        public CreateMetricTypeResponse CreateMetricType(CreateMetricTypeRequest request)
        {
            return ExecuteAction<CreateMetricTypeResponse>(request);
        }

        public SendLogResponseDto SendLog(SendLogRequestDto request)
        {
            return ExecuteAction<SendLogResponseDto>(request);
        }

        public GetLogsResponseDto GetLogs(GetLogsRequestDto request)
        {
            return ExecuteAction<GetLogsResponseDto>(request);
        }

        public GetLogConfigResponseDto GetLogConfig(GetLogConfigRequestDto request)
        {
            return ExecuteAction<GetLogConfigResponseDto>(request);
        }

        public GetChangedWebLogConfigsResponseDto GetChangedWebLogConfigs(GetChangedWebLogConfigsRequestDto request)
        {
            return ExecuteAction<GetChangedWebLogConfigsResponseDto>(request);
        }

        public CreateSubscriptionResponse CreateSubscription(CreateSubscriptionRequest request)
        {
            return ExecuteAction<CreateSubscriptionResponse>(request);
        }

        public CreateUserDefaultSubscriptionResponse CreateUserDefaultSubscription(CreateUserDefaultSubscriptionRequest request)
        {
            return ExecuteAction<CreateUserDefaultSubscriptionResponse>(request);
        }

        public UpdateSubscriptionResponse UpdateSubscription(UpdateSubscriptionRequest request)
        {
            return ExecuteAction<UpdateSubscriptionResponse>(request);
        }

        public SetSubscriptionDisableResponse SetSubscriptionDisable(SetSubscriptionDisableRequest request)
        {
            return ExecuteAction<SetSubscriptionDisableResponse>(request);
        }

        public SetSubscriptionEnableResponse SetSubscriptionEnable(SetSubscriptionEnableRequest request)
        {
            return ExecuteAction<SetSubscriptionEnableResponse>(request);
        }

        public DeleteSubscriptionResponse DeleteSubscription(DeleteSubscriptionRequest request)
        {
            return ExecuteAction<DeleteSubscriptionResponse>(request);
        }

        public SendSmsResponse SendSms(SendSmsRequest request)
        {
            return ExecuteAction<SendSmsResponse>(request);
        }

        public SetUnitTestNextTimeResponse SetUnitTestNextTime(SetUnitTestNextTimeRequest request)
        {
            return ExecuteAction<SetUnitTestNextTimeResponse>(request);
        }

        public UpdateUnitTestResponse UpdateUnitTest(UpdateUnitTestRequest request)
        {
            return ExecuteAction<UpdateUnitTestResponse>(request);
        }

        public DeleteUnitTestResponse DeleteUnitTest(DeleteUnitTestRequest request)
        {
            return ExecuteAction<DeleteUnitTestResponse>(request);
        }

        public SendUnitTestResultResponseDto SendUnitTestResult(SendUnitTestResultRequestDto request)
        {
            return ExecuteAction<SendUnitTestResultResponseDto>(request);
        }

        public SendUnitTestResultsResponseDto SendUnitTestResults(SendUnitTestResultsRequestDto request)
        {
            return ExecuteAction<SendUnitTestResultsResponseDto>(request);
        }

        public GetUnitTestStateResponseDto GetUnitTestState(GetUnitTestStateRequestDto request)
        {
            return ExecuteAction<GetUnitTestStateResponseDto>(request);
        }

        public GetComponentTotalStateResponseDto GetComponentTotalState(GetComponentTotalStateRequestDto request)
        {
            return ExecuteAction<GetComponentTotalStateResponseDto>(request);
        }

        public GetComponentInternalStateResponseDto GetComponentInternalState(GetComponentInternalStateRequestDto request)
        {
            return ExecuteAction<GetComponentInternalStateResponseDto>(request);
        }

        public GetServerTimeResponseDto GetServerTime()
        {
            return ExecuteAction<GetServerTimeResponseDto>(null);
        }

        public SaveAllCachesResponse SaveAllCaches(SaveAllCachesRequest request)
        {
            return ExecuteAction<SaveAllCachesResponse>(request);
        }

        public GetAccountLimitsResponse GetAccountLimits(GetAccountLimitsRequest request)
        {
            return ExecuteAction<GetAccountLimitsResponse>(request);
        }
        public SendLogsResponseDto SendLogs(SendLogsRequestDto request)
        {
            return ExecuteAction<SendLogsResponseDto>(request);
        }

        public GetRootControlDataResponseDto GetRootControlData(GetRootControlDataRequestDto request)
        {
            return ExecuteAction<GetRootControlDataResponseDto>(request);
        }

        public GetOrCreateUnitTestTypeResponseDto GetOrCreateUnitTestType(GetOrCreateUnitTestTypeRequestDto request)
        {
            return ExecuteAction<GetOrCreateUnitTestTypeResponseDto>(request);
        }

        public GetUnitTestTypeByIdResponse GetUnitTestTypeById(GetUnitTestTypeByIdRequest request)
        {
            return ExecuteAction<GetUnitTestTypeByIdResponse>(request);
        }

        public UpdateUnitTestTypeResponse UpdateUnitTestType(UpdateUnitTestTypeRequest request)
        {
            return ExecuteAction<UpdateUnitTestTypeResponse>(request);
        }

        public DeleteUnitTestTypeResponse DeleteUnitTestType(DeleteUnitTestTypeRequest request)
        {
            return ExecuteAction<DeleteUnitTestTypeResponse>(request);
        }

        public GetOrCreateUnitTestResponseDto GetOrCreateUnitTest(GetOrCreateUnitTestRequestDto request)
        {
            return ExecuteAction<GetOrCreateUnitTestResponseDto>(request);
        }

        public UpdateComponentStateResponse UpdateComponentState(UpdateComponentStateRequest request)
        {
            return ExecuteAction<UpdateComponentStateResponse>(request);
        }

        public SetComponentDisableResponseDto SetComponentDisable(SetComponentDisableRequestDto request)
        {
            return ExecuteAction<SetComponentDisableResponseDto>(request);
        }

        public DeleteComponentResponseDto DeleteComponent(DeleteComponentRequestDto request)
        {
            return ExecuteAction<DeleteComponentResponseDto>(request);
        }

        public UpdateMetricsResponse UpdateMetrics(UpdateMetricsRequest request)
        {
            return ExecuteAction<UpdateMetricsResponse>(request);
        }

        public SetMetricEnableResponse SetMetricEnable(SetMetricEnableRequest request)
        {
            return ExecuteAction<SetMetricEnableResponse>(request);
        }

        public SendMetricResponseDto SendMetric(SendMetricRequestDto request)
        {
            return ExecuteAction<SendMetricResponseDto>(request);
        }

        public GetMetricResponseDto GetMetric(GetMetricRequestDto request)
        {
            return ExecuteAction<GetMetricResponseDto>(request);
        }

        public SetUnitTestDisableResponseDto SetUnitTestDisable(SetUnitTestDisableRequestDto request)
        {
            return ExecuteAction<SetUnitTestDisableResponseDto>(request);
        }

        public AddPingUnitTestResponse AddPingUnitTest(AddPingUnitTestRequest request)
        {
            return ExecuteAction<AddPingUnitTestResponse>(request);
        }

        public AddHttpUnitTestResponse AddHttpUnitTest(AddHttpUnitTestRequest request)
        {
            return ExecuteAction<AddHttpUnitTestResponse>(request);
        }

        public RecalcUnitTestsResultsResponse RecalcUnitTestsResults(RecalcUnitTestsResultsRequest request)
        {
            return ExecuteAction<RecalcUnitTestsResultsResponse>(request);
        }

        public UpdateEventsStatusesResponse UpdateEventsStatuses(UpdateEventsStatusesRequest request)
        {
            return ExecuteAction<UpdateEventsStatusesResponse>(request);
        }

        public SetComponentEnableResponseDto SetComponentEnable(SetComponentEnableRequestDto request)
        {
            return ExecuteAction<SetComponentEnableResponseDto>(request);
        }

        public SetUnitTestEnableResponseDto SetUnitTestEnable(SetUnitTestEnableRequestDto request)
        {
            return ExecuteAction<SetUnitTestEnableResponseDto>(request);
        }

        public SetUnitTestNextStepProcessTimeResponse SetUnitTestNextStepProcessTime(SetUnitTestNextStepProcessTimeRequest request)
        {
            return ExecuteAction<SetUnitTestNextStepProcessTimeResponse>(request);
        }

        public GetLogicSettingsResponse GetLogicSettings(GetLogicSettingsRequest request)
        {
            return ExecuteAction<GetLogicSettingsResponse>(request);
        }
    }
}
