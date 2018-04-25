using System;
using System.Runtime.CompilerServices;
using Zidium.Api;
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

        private Uri _uri = null; // new Uri("http://dispatcher.zidium.net");

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

        protected TResponse ExecuteHttp<TResponse>(Request requestObj, string action)
             where TResponse : Response, new()
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
                var proxy = new WebServiceProxyHelper();
                var handlerUrl = Uri.AbsoluteUri;
                return proxy.ExecuteAction<TResponse>(handlerUrl, action, Serializer, requestObj);
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
        protected TResponse ExecuteAction<TResponse>(Request requestObj, [CallerMemberName] string action = null)
            where TResponse : Response, new()
        {
            try
            {
                if (action == null)
                {
                    throw new ArgumentNullException("action");
                }
                if (requestObj!=null && requestObj.Token == null)
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

        public GetEchoResponse GetEcho(GetEchoRequest request)
        {
            return ExecuteAction<GetEchoResponse>(request);
        }

        public GetComponentControlByIdResponse GetComponentControlById(GetComponentControlByIdRequest request)
        {
            return ExecuteAction<GetComponentControlByIdResponse>(request);
        }

        public GetOrAddComponentResponse GetOrAddComponent(GetOrAddComponentRequest request)
        {
            return ExecuteAction<GetOrAddComponentResponse>(request);
        }

        public GetRootComponentResponse GetRootComponent(GetRootComponentRequest request)
        {
            return ExecuteAction<GetRootComponentResponse>(request);
        }

        public GetComponentByIdResponse GetComponentById(GetComponentByIdRequest request)
        {
            return ExecuteAction<GetComponentByIdResponse>(request);
        }

        public GetComponentBySystemNameResponse GetComponentBySystemName(GetComponentBySystemNameRequest request)
        {
            return ExecuteAction<GetComponentBySystemNameResponse>(request);
        }

        public CreateComponentResponse CreateComponent(CreateComponentRequest request)
        {
            return ExecuteAction<CreateComponentResponse>(request);
        }

        public GetOrCreateComponentResponse GetOrCreateComponent(GetOrCreateComponentRequest request)
        {
            return ExecuteAction<GetOrCreateComponentResponse>(request);
        }

        public UpdateComponentResponse UpdateComponent(UpdateComponentRequest request)
        {
            return ExecuteAction<UpdateComponentResponse>(request);
        }

        public GetChildComponentsResponse GetChildComponents(GetChildComponentsRequest request)
        {
            return ExecuteAction<GetChildComponentsResponse>(request);
        }

        public GetComponentAndChildIdsResponse GetComponentAndChildIds(GetComponentAndChildIdsRequest request)
        {
            return ExecuteAction<GetComponentAndChildIdsResponse>(request);
        }

        public GetComponentTypeResponse GetComponentType(GetComponentTypeRequest request)
        {
            return ExecuteAction<GetComponentTypeResponse>(request);
        }

        public GetOrCreateComponentTypeResponse GetOrCreateComponentType(GetOrCreateComponentTypeRequest request)
        {
            return ExecuteAction<GetOrCreateComponentTypeResponse>(request);
        }

        public UpdateComponentTypeResponse UpdateComponentType(UpdateComponentTypeRequest request)
        {
            return ExecuteAction<UpdateComponentTypeResponse>(request);
        }

        public DeleteComponentTypeResponse DeleteComponentType(DeleteComponentTypeRequest request)
        {
            return ExecuteAction<DeleteComponentTypeResponse>(request);
        }

        public SendEventResponse SendEvent(SendEventRequest request)
        {
            return ExecuteAction<SendEventResponse>(request);
        }

        public JoinEventsResponse JoinEvents(JoinEventsRequest request)
        {
            return ExecuteAction<JoinEventsResponse>(request);
        }

        public UpdateEventTypeResponse UpdateEventType(UpdateEventTypeRequest request)
        {
            return ExecuteAction<UpdateEventTypeResponse>(request);
        }

        public CreateMetricResponse CreateMetric(CreateMetricRequest request)
        {
            return ExecuteAction<CreateMetricResponse>(request);
        }

        public GetEventByIdResponse GetEventById(GetEventByIdRequest request)
        {
            return ExecuteAction<GetEventByIdResponse>(request);
        }

        public GetEventsResponse GetEvents(GetEventsRequest request)
        {
            return ExecuteAction<GetEventsResponse>(request);
        }

        public SendMetricsResponse SendMetrics(SendMetricsRequest request)
        {
            return ExecuteAction<SendMetricsResponse>(request);
        }

        public GetMetricsResponse GetMetrics(GetMetricsRequest request)
        {
            return ExecuteAction<GetMetricsResponse>(request);
        }

        public GetMetricsHistoryResponse GetMetricsHistory(GetMetricsHistoryRequest request)
        {
            return ExecuteAction<GetMetricsHistoryResponse>(request);
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

        public SendLogResponse SendLog(SendLogRequest request)
        {
            return ExecuteAction<SendLogResponse>(request);
        }

        public GetLogsResponse GetLogs(GetLogsRequest request)
        {
            return ExecuteAction<GetLogsResponse>(request);
        }

        public GetLogConfigResponse GetLogConfig(GetLogConfigRequest request)
        {
            return ExecuteAction<GetLogConfigResponse>(request);
        }

        public GetChangedWebLogConfigsResponse GetChangedWebLogConfigs(GetChangedWebLogConfigsRequest request)
        {
            return ExecuteAction<GetChangedWebLogConfigsResponse>(request);
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

        public SendSmsResponse SendSms(SendSmsRequest request)
        {
            return ExecuteAction<SendSmsResponse>(request);
        }

        public AccountRegistrationResponse AccountRegistrationStep1(AccountRegistrationStep1Request request)
        {
            return ExecuteAction<AccountRegistrationResponse>(request);
        }

        public AccountRegistrationResponse AccountRegistrationStep2(AccountRegistrationStep2Request request)
        {
            return ExecuteAction<AccountRegistrationResponse>(request);
        }

        public AccountRegistrationResponse AccountRegistrationStep3(AccountRegistrationStep3Request request)
        {
            return ExecuteAction<AccountRegistrationResponse>(request);
        }

        public EndAccountRegistrationResponse EndAccountRegistration(EndAccountRegistrationRequest request)
        {
            return ExecuteAction<EndAccountRegistrationResponse>(request);
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

        public SendUnitTestResultResponse SendUnitTestResult(SendUnitTestResultRequest request)
        {
            return ExecuteAction<SendUnitTestResultResponse>(request);
        }

        public GetUnitTestStateResponse GetUnitTestState(GetUnitTestStateRequest request)
        {
            return ExecuteAction<GetUnitTestStateResponse>(request);
        }

        public SendHttpUnitTestBannerResponse SendHttpUnitTestBanner(SendHttpUnitTestBannerRequest request)
        {
            return ExecuteAction<SendHttpUnitTestBannerResponse>(request);
        }

        public GetComponentTotalStateResponse GetComponentTotalState(GetComponentTotalStateRequest request)
        {
            return ExecuteAction<GetComponentTotalStateResponse>(request);
        }

        public GetComponentInternalStateResponse GetComponentInternalState(GetComponentInternalStateRequest request)
        {
            return ExecuteAction<GetComponentInternalStateResponse>(request);
        }

        public GetServerTimeResponse GetServerTime()
        {
            return ExecuteAction<GetServerTimeResponse>(null);
        }

        public SaveAllCachesResponse SaveAllCaches(SaveAllCachesRequest request)
        {
            return ExecuteAction<SaveAllCachesResponse>(request);
        }

        public GetAccountLimitsResponse GetAccountLimits(GetAccountLimitsRequest request)
        {
            return ExecuteAction<GetAccountLimitsResponse>(request);
        }

        public GetAllAccountsLimitsResponse GetAllAccountsLimits(GetAllAccountsLimitsRequest request)
        {
            return ExecuteAction<GetAllAccountsLimitsResponse>(request);
        }

        public MakeAccountFreeResponse MakeAccountFree(MakeAccountFreeRequest request)
        {
            return ExecuteAction<MakeAccountFreeResponse>(request);
        }

        public MakeAccountPaidResponse MakeAccountPaidAndSetLimits(MakeAccountPaidRequest request)
        {
            return ExecuteAction<MakeAccountPaidResponse>(request);
        }

        public SetAccountLimitsResponse SetAccountLimits(SetAccountLimitsRequest request)
        {
            return ExecuteAction<SetAccountLimitsResponse>(request);
        }

        public GetDatabaseByIdResponse GetDatabaseById(GetDatabaseByIdRequest request)
        {
            return ExecuteAction<GetDatabaseByIdResponse>(request);
        }

        public GetDatabasesResponse GetDatabases(GetDatabasesRequest request)
        {
            return ExecuteAction<GetDatabasesResponse>(request);
        }

        public SetDatabaseIsBrokenResponse SetDatabaseIsBroken(SetDatabaseIsBrokenRequest request)
        {
            return ExecuteAction<SetDatabaseIsBrokenResponse>(request);
        }

        public GetAccountByIdResponse GetAccountById(GetAccountByIdRequest request)
        {
            return ExecuteAction<GetAccountByIdResponse>(request);
        }

        public GetAccountsResponse GetAccounts(GetAccountsRequest request)
        {
            return ExecuteAction<GetAccountsResponse>(request);
        }

        public UpdateAccountResponse UpdateAccount(UpdateAccountRequest request)
        {
            return ExecuteAction<UpdateAccountResponse>(request);
        }

        public SendLogsResponse SendLogs(SendLogsRequest request)
        {
            return ExecuteAction<SendLogsResponse>(request);
        }

        public GetRootControlDataResponse GetRootControlData(GetRootControlDataRequest request)
        {
            return ExecuteAction<GetRootControlDataResponse>(request);
        }

        public GetOrCreateUnitTestTypeResponse GetOrCreateUnitTestType(GetOrCreateUnitTestTypeRequest request)
        {
            return ExecuteAction<GetOrCreateUnitTestTypeResponse>(request);
        }

        public UpdateUnitTestTypeResponse UpdateUnitTestType(UpdateUnitTestTypeRequest request)
        {
            return ExecuteAction<UpdateUnitTestTypeResponse>(request);
        }

        public DeleteUnitTestTypeResponse DeleteUnitTestType(DeleteUnitTestTypeRequest request)
        {
            return ExecuteAction<DeleteUnitTestTypeResponse>(request);
        }

        public GetOrCreateUnitTestResponse GetOrCreateUnitTest(GetOrCreateUnitTestRequest request)
        {
            return ExecuteAction<GetOrCreateUnitTestResponse>(request);
        }

        public UpdateComponentStateResponse UpdateComponentState(UpdateComponentStateRequest request)
        {
            return ExecuteAction<UpdateComponentStateResponse>(request);
        }

        public AddYandexKassaPaymentResponse AddYandexKassaPayment(AddYandexKassaPaymentRequest request)
        {
            return ExecuteAction<AddYandexKassaPaymentResponse>(request);
        }

        public SetComponentDisableResponse SetComponentDisable(SetComponentDisableRequest request)
        {
            return ExecuteAction<SetComponentDisableResponse>(request);
        }

        public DeleteComponentResponse DeleteComponent(DeleteComponentRequest request)
        {
            return ExecuteAction<DeleteComponentResponse>(request);
        }

        public UpdateMetricsResponse UpdateMetrics(UpdateMetricsRequest request)
        {
            return ExecuteAction<UpdateMetricsResponse>(request);
        }

        public SetMetricEnableResponse SetMetricEnable(SetMetricEnableRequest request)
        {
            return ExecuteAction<SetMetricEnableResponse>(request);
        }


        public SendMetricResponse SendMetric(SendMetricRequest request)
        {
            return ExecuteAction<SendMetricResponse>(request);
        }

        public GetMetricResponse GetMetric(GetMetricRequest request)
        {
            return ExecuteAction<GetMetricResponse>(request);
        }

        public SetUnitTestDisableResponse SetUnitTestDisable(SetUnitTestDisableRequest request)
        {
            return ExecuteAction<SetUnitTestDisableResponse>(request);
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


        public SetComponentEnableResponse SetComponentEnable(SetComponentEnableRequest request)
        {
            return ExecuteAction<SetComponentEnableResponse>(request);
        }


        public SetUnitTestEnableResponse SetUnitTestEnable(SetUnitTestEnableRequest request)
        {
            return ExecuteAction<SetUnitTestEnableResponse>(request);
        }

        public ProcessPartnerPaymentsResponse ProcessPartnerPayments(ProcessPartnerPaymentsRequest request)
        {
            return ExecuteAction<ProcessPartnerPaymentsResponse>(request);
        }
    }
}
