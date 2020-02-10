using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using NLog;
using Zidium.Api;
using Zidium.Api.Dto;
using Zidium.Core.Api.Accounts.ChangeApiKey;
using Zidium.Core.Caching;
using Zidium.Core.Common.TaskQueue;
using Zidium.Core.Limits;

namespace Zidium.Core.Api
{
    /// <summary>
    /// Обертка над настоящим диспетчером, чтобы отлавливать исключения
    /// </summary>
    public class DispatcherWrapper : IDispatcherService
    {
        protected IDispatcherService InternalService { get; set; }

        public IComponentControl Control { get; protected set; }

        public static Stopwatch UptimeTimer { get; set; }

        public DeadLockHunter DeadLockHunter;

        static DispatcherWrapper()
        {
            UptimeTimer = new Stopwatch();
            UptimeTimer.Start();
        }

        public DispatcherWrapper(
            IDispatcherService service,
            IComponentControl componentControl)
        {
            if (service == null)
            {
                throw new ArgumentNullException("service");
            }
            if (componentControl == null)
            {
                throw new ArgumentNullException("componentControl");
            }
            InternalService = service;
            Control = componentControl;
            DeadLockHunter = new DeadLockHunter(componentControl);
        }

        protected TResponse Execute<TResponse>(Func<TResponse> action, [CallerMemberName]string method = "unknown")
            where TResponse : Response,  new()
        {
            var threadId = Guid.Empty;
            try
            {
                threadId = DeadLockHunter.Add(method);
                return action();
            }
            catch (ResponseCodeException exception)
            {
                return new TResponse()
                {
                    ErrorMessage = exception.Message,
                    Code = exception.Code
                };
            }
            catch (ResponseException exception)
            {
                return new TResponse()
                {
                    ErrorMessage = exception.Response.ErrorMessage,
                    Code = exception.Response.Code
                };
            }
            catch (OverLimitException exception)
            {
                return new TResponse()
                {
                    ErrorMessage = exception.Message,
                    Code = ResponseCode.OverLimit
                };
            }
            catch (UserFriendlyException exception)
            {
                return new TResponse()
                {
                    ErrorMessage = exception.Message,
                    Code = ResponseCode.CommonError
                };
            }
            catch (Exception exception)
            {
                LogManager.GetCurrentClassLogger().Error(exception);

                return new TResponse()
                {
                    ErrorMessage = exception.Message,
                    Code = ResponseCode.ServerError
                };
            }
            finally
            {
                DeadLockHunter.Remove(threadId);
            }
        }

        public GetComponentControlByIdResponse GetComponentControlById(GetComponentControlByIdRequest request)
        {
            return Execute(() => InternalService.GetComponentControlById(request));
        }

        public GetOrAddComponentResponse GetOrAddComponent(GetOrAddComponentRequest request)
        {
            return Execute(() => InternalService.GetOrAddComponent(request));
        }

        public GetRootComponentResponse GetRootComponent(GetRootComponentRequest request)
        {
            return Execute(() => InternalService.GetRootComponent(request));
        }

        public GetComponentByIdResponse GetComponentById(GetComponentByIdRequest request)
        {
            return Execute(() => InternalService.GetComponentById(request));
        }

        public GetComponentBySystemNameResponse GetComponentBySystemName(GetComponentBySystemNameRequest request)
        {
            return Execute(() => InternalService.GetComponentBySystemName(request));
        }

        public CreateComponentResponse CreateComponent(CreateComponentRequest request)
        {
            return Execute(() => InternalService.CreateComponent(request));
        }

        public GetOrCreateComponentResponse GetOrCreateComponent(GetOrCreateComponentRequest request)
        {
            return Execute(() => InternalService.GetOrCreateComponent(request));
        }

        public UpdateComponentResponse UpdateComponent(UpdateComponentRequest request)
        {
            return Execute(() => InternalService.UpdateComponent(request));
        }

        public GetChildComponentsResponse GetChildComponents(GetChildComponentsRequest request)
        {
            return Execute(() => InternalService.GetChildComponents(request));
        }

        public GetComponentAndChildIdsResponse GetComponentAndChildIds(GetComponentAndChildIdsRequest request)
        {
            return Execute(() => InternalService.GetComponentAndChildIds(request));
        }

        public GetComponentTypeResponse GetComponentType(GetComponentTypeRequest request)
        {
            return Execute(() => InternalService.GetComponentType(request));
        }

        public GetOrCreateComponentTypeResponse GetOrCreateComponentType(GetOrCreateComponentTypeRequest request)
        {
            return Execute(() => InternalService.GetOrCreateComponentType(request));
        }

        public UpdateComponentTypeResponse UpdateComponentType(UpdateComponentTypeRequest request)
        {
            return Execute(() => InternalService.UpdateComponentType(request));
        }

        public DeleteComponentTypeResponse DeleteComponentType(DeleteComponentTypeRequest request)
        {
            return Execute(() => InternalService.DeleteComponentType(request));
        }

        public SendEventResponse SendEvent(SendEventRequest request)
        {
            return Execute(() => InternalService.SendEvent(request));
        }

        public JoinEventsResponse JoinEvents(JoinEventsRequest request)
        {
            return Execute(() => InternalService.JoinEvents(request));
        }

        public UpdateEventTypeResponse UpdateEventType(UpdateEventTypeRequest request)
        {
            return Execute(() => InternalService.UpdateEventType(request));
        }

        public CreateMetricResponse CreateMetric(CreateMetricRequest request)
        {
            return Execute(() => InternalService.CreateMetric(request));
        }

        public GetEventByIdResponse GetEventById(GetEventByIdRequest request)
        {
            return Execute(() => InternalService.GetEventById(request));
        }

        public GetEventsResponse GetEvents(GetEventsRequest request)
        {
            return Execute(() => InternalService.GetEvents(request));
        }

        public SendMetricsResponse SendMetrics(SendMetricsRequest request)
        {
            return Execute(() => InternalService.SendMetrics(request));
        }

        public GetMetricsResponse GetMetrics(GetMetricsRequest request)
        {
            return Execute(() => InternalService.GetMetrics(request));
        }

        public GetMetricsHistoryResponse GetMetricsHistory(GetMetricsHistoryRequest request)
        {
            return Execute(() => InternalService.GetMetricsHistory(request));
        }

        public DeleteMetricResponse DeleteMetric(DeleteMetricRequest request)
        {
            return Execute(() => InternalService.DeleteMetric(request));
        }

        public DeleteMetricTypeResponse DeleteMetricType(DeleteMetricTypeRequest request)
        {
            return Execute(() => InternalService.DeleteMetricType(request));
        }

        public SetMetricDisableResponse SetMetricDisable(SetMetricDisableRequest request)
        {
            return Execute(() => InternalService.SetMetricDisable(request));
        }

        public UpdateMetricResponse UpdateMetric(UpdateMetricRequest request)
        {
            return Execute(() => InternalService.UpdateMetric(request));
        }

        public UpdateMetricTypeResponse UpdateMetricType(UpdateMetricTypeRequest request)
        {
            return Execute(() => InternalService.UpdateMetricType(request));
        }

        public CreateMetricTypeResponse CreateMetricType(CreateMetricTypeRequest request)
        {
            return Execute(() => InternalService.CreateMetricType(request));
        }

        public SendLogResponse SendLog(SendLogRequest request)
        {
            return Execute(() => InternalService.SendLog(request));
        }

        public GetLogsResponse GetLogs(GetLogsRequest request)
        {
            return Execute(() => InternalService.GetLogs(request));
        }

        public GetLogConfigResponse GetLogConfig(GetLogConfigRequest request)
        {
            return Execute(() => InternalService.GetLogConfig(request));
        }

        public GetChangedWebLogConfigsResponse GetChangedWebLogConfigs(GetChangedWebLogConfigsRequest request)
        {
            return Execute(() => InternalService.GetChangedWebLogConfigs(request));
        }

        public CreateSubscriptionResponse CreateSubscription(CreateSubscriptionRequest request)
        {
            return Execute(() => InternalService.CreateSubscription(request));
        }

        public CreateUserDefaultSubscriptionResponse CreateUserDefaultSubscription(CreateUserDefaultSubscriptionRequest request)
        {
            return Execute(() => InternalService.CreateUserDefaultSubscription(request));
        }

        public UpdateSubscriptionResponse UpdateSubscription(UpdateSubscriptionRequest request)
        {
            return Execute(() => InternalService.UpdateSubscription(request));
        }

        public SetSubscriptionDisableResponse SetSubscriptionDisable(SetSubscriptionDisableRequest request)
        {
            return Execute(() => InternalService.SetSubscriptionDisable(request));
        }

        public SetSubscriptionEnableResponse SetSubscriptionEnable(SetSubscriptionEnableRequest request)
        {
            return Execute(() => InternalService.SetSubscriptionEnable(request));
        }

        public DeleteSubscriptionResponse DeleteSubscription(DeleteSubscriptionRequest request)
        {
            return Execute(() => InternalService.DeleteSubscription(request));
        }

        public SendSmsResponse SendSms(SendSmsRequest request)
        {
            return Execute(() => InternalService.SendSms(request));
        }

        public AccountRegistrationResponse AccountRegistrationStep1(AccountRegistrationStep1Request request)
        {
            return Execute(() => InternalService.AccountRegistrationStep1(request));
        }

        public AccountRegistrationResponse AccountRegistrationStep2(AccountRegistrationStep2Request request)
        {
            return Execute(() => InternalService.AccountRegistrationStep2(request));
        }

        public AccountRegistrationResponse AccountRegistrationStep3(AccountRegistrationStep3Request request)
        {
            return Execute(() => InternalService.AccountRegistrationStep3(request));
        }

        public EndAccountRegistrationResponse EndAccountRegistration(EndAccountRegistrationRequest request)
        {
            return Execute(() => InternalService.EndAccountRegistration(request));
        }

        public SetUnitTestNextTimeResponse SetUnitTestNextTime(SetUnitTestNextTimeRequest request)
        {
            return Execute(() => InternalService.SetUnitTestNextTime(request));
        }

        public SendUnitTestResultResponse SendUnitTestResult(SendUnitTestResultRequest request)
        {
            return Execute(() => InternalService.SendUnitTestResult(request));
        }

        public SendUnitTestResultsResponse SendUnitTestResults(SendUnitTestResultsRequest request)
        {
            return Execute(() => InternalService.SendUnitTestResults(request));
        }

        public GetUnitTestStateResponse GetUnitTestState(GetUnitTestStateRequest request)
        {
            return Execute(() => InternalService.GetUnitTestState(request));
        }

        public SendHttpUnitTestBannerResponse SendHttpUnitTestBanner(SendHttpUnitTestBannerRequest request)
        {
            return Execute(() => InternalService.SendHttpUnitTestBanner(request));
        }

        public GetComponentTotalStateResponse GetComponentTotalState(GetComponentTotalStateRequest request)
        {
            return Execute(() => InternalService.GetComponentTotalState(request));
        }

        public GetComponentInternalStateResponse GetComponentInternalState(GetComponentInternalStateRequest request)
        {
            return Execute(() => InternalService.GetComponentInternalState(request));
        }

        public GetServerTimeResponse GetServerTime()
        {
            return Execute(() => InternalService.GetServerTime());
        }

        public SaveAllCachesResponse SaveAllCaches(SaveAllCachesRequest request)
        {
            return Execute(() => InternalService.SaveAllCaches(request));
        }

        public GetAccountLimitsResponse GetAccountLimits(GetAccountLimitsRequest request)
        {
            return Execute(() => InternalService.GetAccountLimits(request));
        }

        public GetAllAccountsLimitsResponse GetAllAccountsLimits(GetAllAccountsLimitsRequest request)
        {
            return Execute(() => InternalService.GetAllAccountsLimits(request));
        }

        public MakeAccountFreeResponse MakeAccountFree(MakeAccountFreeRequest request)
        {
            return Execute(() => InternalService.MakeAccountFree(request));
        }

        public MakeAccountPaidResponse MakeAccountPaidAndSetLimits(MakeAccountPaidRequest request)
        {
            return Execute(() => InternalService.MakeAccountPaidAndSetLimits(request));
        }

        public SetAccountLimitsResponse SetAccountLimits(SetAccountLimitsRequest request)
        {
            return Execute(() => InternalService.SetAccountLimits(request));
        }

        public GetDatabaseByIdResponse GetDatabaseById(GetDatabaseByIdRequest request)
        {
            return Execute(() => InternalService.GetDatabaseById(request));
        }

        public GetDatabasesResponse GetDatabases(GetDatabasesRequest request)
        {
            return Execute(() => InternalService.GetDatabases(request));
        }

        public SetDatabaseIsBrokenResponse SetDatabaseIsBroken(SetDatabaseIsBrokenRequest request)
        {
            return Execute(() => InternalService.SetDatabaseIsBroken(request));
        }

        public GetAccountByIdResponse GetAccountById(GetAccountByIdRequest request)
        {
            return Execute(() => InternalService.GetAccountById(request));
        }

        public GetAccountsResponse GetAccounts(GetAccountsRequest request)
        {
            return Execute(() => InternalService.GetAccounts(request));
        }

        public UpdateAccountResponse UpdateAccount(UpdateAccountRequest request)
        {
            return Execute(() => InternalService.UpdateAccount(request));
        }

        public SendLogsResponse SendLogs(SendLogsRequest request)
        {
            return Execute(() => InternalService.SendLogs(request));
        }

        public GetRootControlDataResponse GetRootControlData(GetRootControlDataRequest request)
        {
            return Execute(() => InternalService.GetRootControlData(request));
        }

        public GetOrCreateUnitTestTypeResponse GetOrCreateUnitTestType(GetOrCreateUnitTestTypeRequest request)
        {
            return Execute(() => InternalService.GetOrCreateUnitTestType(request));
        }

        public GetUnitTestTypeByIdResponse GetUnitTestTypeById(GetUnitTestTypeByIdRequest request)
        {
            return Execute(() => InternalService.GetUnitTestTypeById(request));
        }

        public UpdateUnitTestTypeResponse UpdateUnitTestType(UpdateUnitTestTypeRequest request)
        {
            return Execute(() => InternalService.UpdateUnitTestType(request));
        }

        public DeleteUnitTestTypeResponse DeleteUnitTestType(DeleteUnitTestTypeRequest request)
        {
            return Execute(() => InternalService.DeleteUnitTestType(request));
        }

        public GetOrCreateUnitTestResponse GetOrCreateUnitTest(GetOrCreateUnitTestRequest request)
        {
            return Execute(() => InternalService.GetOrCreateUnitTest(request));
        }

        public UpdateUnitTestResponse UpdateUnitTest(UpdateUnitTestRequest request)
        {
            return Execute(() => InternalService.UpdateUnitTest(request));
        }

        public DeleteUnitTestResponse DeleteUnitTest(DeleteUnitTestRequest request)
        {
            return Execute(() => InternalService.DeleteUnitTest(request));
        }

        public GetEchoResponse GetEcho(GetEchoRequest request)
        {
            return Execute(() => InternalService.GetEcho(request));
        }

        public UpdateComponentStateResponse UpdateComponentState(UpdateComponentStateRequest request)
        {
            return Execute(() => InternalService.UpdateComponentState(request));
        }

        public AddYandexKassaPaymentResponse AddYandexKassaPayment(AddYandexKassaPaymentRequest request)
        {
            return Execute(() => InternalService.AddYandexKassaPayment(request));
        }

        public SetComponentEnableResponse SetComponentEnable(SetComponentEnableRequest request)
        {
            return Execute(() => InternalService.SetComponentEnable(request));
        }

        public SetComponentDisableResponse SetComponentDisable(SetComponentDisableRequest request)
        {
            return Execute(() => InternalService.SetComponentDisable(request));
        }

        public DeleteComponentResponse DeleteComponent(DeleteComponentRequest request)
        {
            return Execute(() => InternalService.DeleteComponent(request));
        }

        public UpdateMetricsResponse UpdateMetrics(UpdateMetricsRequest request)
        {
            return Execute(() => InternalService.UpdateMetrics(request));
        }

        public SetMetricEnableResponse SetMetricEnable(SetMetricEnableRequest request)
        {
            return Execute(() => InternalService.SetMetricEnable(request));
        }

        public SendMetricResponse SendMetric(SendMetricRequest request)
        {
            return Execute(() => InternalService.SendMetric(request));
        }

        public GetMetricResponse GetMetric(GetMetricRequest request)
        {
            return Execute(() => InternalService.GetMetric(request));
        }

        public UpdateEventsStatusesResponse UpdateEventsStatuses(UpdateEventsStatusesRequest request)
        {
            return Execute(() => InternalService.UpdateEventsStatuses(request));
        }

        public SetUnitTestDisableResponse SetUnitTestDisable(SetUnitTestDisableRequest request)
        {
            return Execute(() => InternalService.SetUnitTestDisable(request));
        }

        public AddPingUnitTestResponse AddPingUnitTest(AddPingUnitTestRequest request)
        {
            return Execute(() => InternalService.AddPingUnitTest(request));
        }

        public AddHttpUnitTestResponse AddHttpUnitTest(AddHttpUnitTestRequest request)
        {
            return Execute(() => InternalService.AddHttpUnitTest(request));
        }

        public RecalcUnitTestsResultsResponse RecalcUnitTestsResults(RecalcUnitTestsResultsRequest request)
        {
            return Execute(() => InternalService.RecalcUnitTestsResults(request));
        }

        public SetUnitTestEnableResponse SetUnitTestEnable(SetUnitTestEnableRequest request)
        {
            return Execute(() => InternalService.SetUnitTestEnable(request));
        }

        public ProcessPartnerPaymentsResponse ProcessPartnerPayments(ProcessPartnerPaymentsRequest request)
        {
            return Execute(() => InternalService.ProcessPartnerPayments(request));
        }

        public void SaveCaches()
        {
            AllCaches.SaveChanges();
        }

        public ChangeApiKeyResponse ChangeApiKey(ChangeApiKeyRequest request)
        {
            return Execute(() => InternalService.ChangeApiKey(request));
        }

        public SetUnitTestNextStepProcessTimeResponse SetUnitTestNextStepProcessTime(SetUnitTestNextStepProcessTimeRequest request)
        {
            return Execute(() => InternalService.SetUnitTestNextStepProcessTime(request));
        }
    }
}
