using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;
using Zidium.Api;
using Zidium.Api.Dto;
using Zidium.Common;
using Zidium.Core.Caching;
using Zidium.Core.Common.TaskQueue;

namespace Zidium.Core.Api
{
    /// <summary>
    /// Обертка над настоящим диспетчером, чтобы отлавливать исключения
    /// </summary>
    public class DispatcherWrapper : IDispatcherService
    {
        protected IDispatcherService InternalService { get; set; }

        public IComponentControl Control { get; protected set; }

        public ILogger Logger { get; protected set; }

        public static Stopwatch UptimeTimer { get; set; }

        public DeadLockHunter DeadLockHunter;

        static DispatcherWrapper()
        {
            UptimeTimer = new Stopwatch();
            UptimeTimer.Start();
        }

        public DispatcherWrapper(
            IDispatcherService service,
            IComponentControl componentControl,
            ILogger logger)
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
            Logger = logger;
            DeadLockHunter = new DeadLockHunter(componentControl, Logger);
        }

        protected TResponse Execute<TResponse>(Func<TResponse> action, [CallerMemberName] string method = "unknown")
            where TResponse : ResponseDto, new()
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
                Logger.LogError(exception, exception.Message);

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

        public GetComponentControlByIdResponseDto GetComponentControlById(GetComponentControlByIdRequestDto request)
        {
            return Execute(() => InternalService.GetComponentControlById(request));
        }

        public GetOrAddComponentResponseDto GetOrAddComponent(GetOrAddComponentRequestDto request)
        {
            return Execute(() => InternalService.GetOrAddComponent(request));
        }

        public GetRootComponentResponseDto GetRootComponent(GetRootComponentRequestDto request)
        {
            return Execute(() => InternalService.GetRootComponent(request));
        }

        public GetComponentByIdResponseDto GetComponentById(GetComponentByIdRequestDto request)
        {
            return Execute(() => InternalService.GetComponentById(request));
        }

        public GetComponentBySystemNameResponseDto GetComponentBySystemName(GetComponentBySystemNameRequestDto request)
        {
            return Execute(() => InternalService.GetComponentBySystemName(request));
        }

        public CreateComponentResponse CreateComponent(CreateComponentRequest request)
        {
            return Execute(() => InternalService.CreateComponent(request));
        }

        public GetOrCreateComponentResponseDto GetOrCreateComponent(GetOrCreateComponentRequestDto request)
        {
            return Execute(() => InternalService.GetOrCreateComponent(request));
        }

        public UpdateComponentResponseDto UpdateComponent(UpdateComponentRequestDto request)
        {
            return Execute(() => InternalService.UpdateComponent(request));
        }

        public GetChildComponentsResponseDto GetChildComponents(GetChildComponentsRequestDto request)
        {
            return Execute(() => InternalService.GetChildComponents(request));
        }

        public GetComponentAndChildIdsResponse GetComponentAndChildIds(GetComponentAndChildIdsRequest request)
        {
            return Execute(() => InternalService.GetComponentAndChildIds(request));
        }

        public GetComponentTypeResponseDto GetComponentType(GetComponentTypeRequestDto request)
        {
            return Execute(() => InternalService.GetComponentType(request));
        }

        public GetOrCreateComponentTypeResponseDto GetOrCreateComponentType(GetOrCreateComponentTypeRequestDto request)
        {
            return Execute(() => InternalService.GetOrCreateComponentType(request));
        }

        public UpdateComponentTypeResponseDto UpdateComponentType(UpdateComponentTypeRequestDto request)
        {
            return Execute(() => InternalService.UpdateComponentType(request));
        }

        public DeleteComponentTypeResponse DeleteComponentType(DeleteComponentTypeRequest request)
        {
            return Execute(() => InternalService.DeleteComponentType(request));
        }

        public SendEventResponseDto SendEvent(SendEventRequestDto request)
        {
            return Execute(() => InternalService.SendEvent(request));
        }

        public JoinEventsResponseDto JoinEvents(JoinEventsRequestDto request)
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

        public GetEventByIdResponseDto GetEventById(GetEventByIdRequestDto request)
        {
            return Execute(() => InternalService.GetEventById(request));
        }

        public GetEventsResponseDto GetEvents(GetEventsRequestDto request)
        {
            return Execute(() => InternalService.GetEvents(request));
        }

        public SendMetricsResponseDto SendMetrics(SendMetricsRequestDto request)
        {
            return Execute(() => InternalService.SendMetrics(request));
        }

        public GetMetricsResponseDto GetMetrics(GetMetricsRequestDto request)
        {
            return Execute(() => InternalService.GetMetrics(request));
        }

        public GetMetricsHistoryResponseDto GetMetricsHistory(GetMetricsHistoryRequestDto request)
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

        public SendLogResponseDto SendLog(SendLogRequestDto request)
        {
            return Execute(() => InternalService.SendLog(request));
        }

        public GetLogsResponseDto GetLogs(GetLogsRequestDto request)
        {
            return Execute(() => InternalService.GetLogs(request));
        }

        public GetLogConfigResponseDto GetLogConfig(GetLogConfigRequestDto request)
        {
            return Execute(() => InternalService.GetLogConfig(request));
        }

        public GetChangedWebLogConfigsResponseDto GetChangedWebLogConfigs(GetChangedWebLogConfigsRequestDto request)
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

        public SetUnitTestNextTimeResponse SetUnitTestNextTime(SetUnitTestNextTimeRequest request)
        {
            return Execute(() => InternalService.SetUnitTestNextTime(request));
        }

        public SendUnitTestResultResponseDto SendUnitTestResult(SendUnitTestResultRequestDto request)
        {
            return Execute(() => InternalService.SendUnitTestResult(request));
        }

        public SendUnitTestResultsResponseDto SendUnitTestResults(SendUnitTestResultsRequestDto request)
        {
            return Execute(() => InternalService.SendUnitTestResults(request));
        }

        public GetUnitTestStateResponseDto GetUnitTestState(GetUnitTestStateRequestDto request)
        {
            return Execute(() => InternalService.GetUnitTestState(request));
        }

        public GetComponentTotalStateResponseDto GetComponentTotalState(GetComponentTotalStateRequestDto request)
        {
            return Execute(() => InternalService.GetComponentTotalState(request));
        }

        public GetComponentInternalStateResponseDto GetComponentInternalState(GetComponentInternalStateRequestDto request)
        {
            return Execute(() => InternalService.GetComponentInternalState(request));
        }

        public GetServerTimeResponseDto GetServerTime()
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

        public SendLogsResponseDto SendLogs(SendLogsRequestDto request)
        {
            return Execute(() => InternalService.SendLogs(request));
        }

        public GetRootControlDataResponseDto GetRootControlData(GetRootControlDataRequestDto request)
        {
            return Execute(() => InternalService.GetRootControlData(request));
        }

        public GetOrCreateUnitTestTypeResponseDto GetOrCreateUnitTestType(GetOrCreateUnitTestTypeRequestDto request)
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

        public GetOrCreateUnitTestResponseDto GetOrCreateUnitTest(GetOrCreateUnitTestRequestDto request)
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

        public GetEchoResponseDto GetEcho(GetEchoRequestDto request)
        {
            return Execute(() => InternalService.GetEcho(request));
        }

        public UpdateComponentStateResponse UpdateComponentState(UpdateComponentStateRequest request)
        {
            return Execute(() => InternalService.UpdateComponentState(request));
        }

        public SetComponentEnableResponseDto SetComponentEnable(SetComponentEnableRequestDto request)
        {
            return Execute(() => InternalService.SetComponentEnable(request));
        }

        public SetComponentDisableResponseDto SetComponentDisable(SetComponentDisableRequestDto request)
        {
            return Execute(() => InternalService.SetComponentDisable(request));
        }

        public DeleteComponentResponseDto DeleteComponent(DeleteComponentRequestDto request)
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

        public SendMetricResponseDto SendMetric(SendMetricRequestDto request)
        {
            return Execute(() => InternalService.SendMetric(request));
        }

        public GetMetricResponseDto GetMetric(GetMetricRequestDto request)
        {
            return Execute(() => InternalService.GetMetric(request));
        }

        public UpdateEventsStatusesResponse UpdateEventsStatuses(UpdateEventsStatusesRequest request)
        {
            return Execute(() => InternalService.UpdateEventsStatuses(request));
        }

        public SetUnitTestDisableResponseDto SetUnitTestDisable(SetUnitTestDisableRequestDto request)
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

        public SetUnitTestEnableResponseDto SetUnitTestEnable(SetUnitTestEnableRequestDto request)
        {
            return Execute(() => InternalService.SetUnitTestEnable(request));
        }

        public void SaveCaches()
        {
            AllCaches.SaveChanges();
        }

        public SetUnitTestNextStepProcessTimeResponse SetUnitTestNextStepProcessTime(SetUnitTestNextStepProcessTimeRequest request)
        {
            return Execute(() => InternalService.SetUnitTestNextStepProcessTime(request));
        }

        public GetLogicSettingsResponse GetLogicSettings(GetLogicSettingsRequest request)
        {
            return Execute(() => InternalService.GetLogicSettings(request));
        }

        public void Stop()
        {
            AllCaches.Stop();
        }
    }
}
