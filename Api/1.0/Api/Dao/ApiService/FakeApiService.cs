using System;
using System.Collections.Generic;

namespace Zidium.Api
{
    public class FakeApiService : IApiService
    {
        public int ResponseCode { get; protected set; }

        public string ErrorMessage { get; protected set; }

        public AccessToken AccessToken { get; protected set; }

        public FakeApiService()
        {
            ResponseCode = Api.ResponseCode.Offine;
            ErrorMessage = "Заглушка";
            AccessToken = new AccessToken();
        }

        public FakeApiService(int responseCode, string errorMessage, AccessToken accessToken)
        {
            ResponseCode = responseCode;
            ErrorMessage = errorMessage;
            AccessToken = accessToken;
        }

        protected TResponse GetResponse<TResponse>()
           where TResponse : Response, new()
        {
            return new TResponse()
            {
                Code = ResponseCode,
                ErrorMessage = ErrorMessage
            };
        }

        public void SetAccessToken(AccessToken accessToken)
        {
            if (accessToken == null)
            {
                throw new ArgumentNullException("accessToken");
            }
            AccessToken = accessToken;
        }

        public bool IsFake
        {
            get { return true; }
        }

        public GetEchoResponse GetEcho(string message)
        {
            return GetResponse<GetEchoResponse>();
        }

        public GetServerTimeResponse GetServerTime()
        {
            return GetResponse<GetServerTimeResponse>();
        }

        public GetRootControlDataResponse GetRootControlData()
        {
            return GetResponse<GetRootControlDataResponse>();
        }

        public GetComponentControlByIdResponse GetComponentControlById(Guid componentId)
        {
            return GetResponse<GetComponentControlByIdResponse>();
        }

        public GetOrCreateComponentResponse GetOrCreateComponent(Guid parentId, GetOrCreateComponentData data)
        {
            return GetResponse<GetOrCreateComponentResponse>();
        }

        public GetRootComponentResponse GetRootComponent()
        {
            return GetResponse<GetRootComponentResponse>();
        }

        public GetComponentByIdResponse GetComponentById(Guid componentId)
        {
            return GetResponse<GetComponentByIdResponse>();
        }

        public GetComponentBySystemNameResponse GetComponentBySystemName(string systemName)
        {
            return GetResponse<GetComponentBySystemNameResponse>();
        }

        public GetComponentBySystemNameResponse GetComponentBySystemName(GetComponentBySystemNameData data)
        {
            return GetResponse<GetComponentBySystemNameResponse>();
        }

        public GetChildComponentsResponse GetChildComponents(Guid componentId)
        {
            return GetResponse<GetChildComponentsResponse>();
        }

        public UpdateComponentResponse UpdateComponent(Guid componentId, UpdateComponentData request)
        {
            return GetResponse<UpdateComponentResponse>();
        }

        public GetComponentTotalStateResponse GetComponentTotalState(Guid componentId, bool recalc)
        {
            return GetResponse<GetComponentTotalStateResponse>();
        }

        public GetComponentInternalStateResponse GetComponentInternalState(Guid componentId, bool recalc)
        {
            return GetResponse<GetComponentInternalStateResponse>();
        }

        public SetComponentEnableResponse SetComponentEnable(Guid componentId)
        {
            return GetResponse<SetComponentEnableResponse>();
        }

        public SetComponentDisableResponse SetComponentDisable(Guid componentId, DateTime? toDate, string comment)
        {
            return GetResponse<SetComponentDisableResponse>();
        }

        public DeleteComponentResponse DeleteComponent(Guid componentId)
        {
            return GetResponse<DeleteComponentResponse>();
        }

        public GetComponentTypeResponse GetComponentType(GetComponentTypeData data)
        {
            return GetResponse<GetComponentTypeResponse>();
        }

        public GetOrCreateComponentTypeResponse GetOrCreateComponentType(GetOrCreateComponentTypeData data)
        {
            return GetResponse<GetOrCreateComponentTypeResponse>();
        }

        public UpdateComponentTypeResponse UpdateComponentType(UpdateComponentTypeData data)
        {
            return GetResponse<UpdateComponentTypeResponse>();
        }

        public SendEventResponse SendEvent(SendEventData data)
        {
            return GetResponse<SendEventResponse>();
        }

        public GetEventByIdResponse GetEventById(Guid eventId)
        {
            return GetResponse<GetEventByIdResponse>();
        }

        public GetEventsResponse GetEvents(GetEventsData data)
        {
            return GetResponse<GetEventsResponse>();
        }

        public JoinEventsResponse JoinEvents(List<JoinEventData> data)
        {
            return GetResponse<JoinEventsResponse>();
        }

        public GetOrCreateUnitTestResponse GetOrCreateUnitTest(Guid componentId, GetOrCreateUnitTestData data)
        {
            return GetResponse<GetOrCreateUnitTestResponse>();
        }

        public GetOrCreateUnitTestTypeResponse GetOrCreateUnitTestType(GetOrCreateUnitTestTypeData data)
        {
            return GetResponse<GetOrCreateUnitTestTypeResponse>();
        }

        public SendUnitTestResultResponse SendUnitTestResult(Guid unitTestId, SendUnitTestResultData data)
        {
            return GetResponse<SendUnitTestResultResponse>();
        }

        public GetUnitTestStateResponse GetUnitTestState(Guid unitTestId)
        {
            return GetResponse<GetUnitTestStateResponse>();
        }

        public SetUnitTestEnableResponse SetUnitTestEnable(Guid unitTestId)
        {
            return GetResponse<SetUnitTestEnableResponse>();
        }

        public SetUnitTestDisableResponse SetUnitTestDisable(Guid unitTestId, SetUnitTestDisableRequestData data)
        {
            return GetResponse<SetUnitTestDisableResponse>();
        }

        public SendMetricResponse SendMetric(Guid componentId, SendMetricData data)
        {
            return GetResponse<SendMetricResponse>();
        }

        public SendMetricsResponse SendMetrics(Guid componentId, List<SendMetricData> data)
        {
            return GetResponse<SendMetricsResponse>();
        }

        public GetMetricsHistoryResponse GetMetricsHistory(Guid componentId, GetMetricsHistoryFilter filter)
        {
            return GetResponse<GetMetricsHistoryResponse>();
        }

        public GetMetricsResponse GetMetrics(Guid componentId)
        {
            return GetResponse<GetMetricsResponse>();
        }

        public GetMetricResponse GetMetric(Guid componentId, string metricName)
        {
            return GetResponse<GetMetricResponse>();
        }

        public SendLogResponse SendLog(SendLogData data)
        {
            return GetResponse<SendLogResponse>();
        }

        public SendLogsResponse SendLogs(SendLogData[] data)
        {
            return GetResponse<SendLogsResponse>();
        }

        public GetLogsResponse GetLogs(Guid componentId, GetLogsFilter filter)
        {
            return GetResponse<GetLogsResponse>();
        }

        public GetLogConfigResponse GetLogConfig(Guid componentId)
        {
            return GetResponse<GetLogConfigResponse>();
        }

        public GetChangedWebLogConfigsResponse GetChangedWebLogConfigs(DateTime lastUpdateDate, List<Guid> componentIds)
        {
            return GetResponse<GetChangedWebLogConfigsResponse>();
        }

        public GetOrCreateComponentResponse GetOrCreateComponent(GetOrCreateComponentData data)
        {
            return GetResponse<GetOrCreateComponentResponse>();
        }

        public GetEventByIdResponse GetEventById(Guid componentId, Guid eventId)
        {
            return GetResponse<GetEventByIdResponse>();
        }

        public SendUnitTestResultResponse SendUnitTestResult(SendUnitTestResultData data)
        {
            return GetResponse<SendUnitTestResultResponse>();
        }
    }
}
