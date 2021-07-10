using System;
using System.Collections.Generic;
using Zidium.Api.Dto;

namespace Zidium.Api
{
    public class FakeApiService : IApiService
    {
        public int ResponseCode { get; protected set; }

        public string ErrorMessage { get; protected set; }

        public AccessTokenDto AccessToken { get; protected set; }

        public FakeApiService()
        {
            ResponseCode = Dto.ResponseCode.Offine;
            ErrorMessage = "Заглушка";
            AccessToken = new AccessTokenDto();
        }

        public FakeApiService(int responseCode, string errorMessage, AccessTokenDto accessToken)
        {
            ResponseCode = responseCode;
            ErrorMessage = errorMessage;
            AccessToken = accessToken;
        }

        protected TResponse GetResponse<TResponse>()
           where TResponse : ResponseDto, new()
        {
            return new TResponse()
            {
                Code = ResponseCode,
                ErrorMessage = ErrorMessage
            };
        }

        public void SetAccessToken(AccessTokenDto accessToken)
        {
            if (accessToken == null)
            {
                throw new ArgumentNullException("accessToken");
            }
            AccessToken = accessToken;
        }

        public GetEchoResponseDto GetEcho(string message)
        {
            return GetResponse<GetEchoResponseDto>();
        }

        public GetServerTimeResponseDto GetServerTime()
        {
            return GetResponse<GetServerTimeResponseDto>();
        }

        public GetRootControlDataResponseDto GetRootControlData()
        {
            return GetResponse<GetRootControlDataResponseDto>();
        }

        public GetComponentControlByIdResponseDto GetComponentControlById(Guid componentId)
        {
            return GetResponse<GetComponentControlByIdResponseDto>();
        }

        public GetOrCreateComponentResponseDto GetOrCreateComponent(GetOrCreateComponentRequestDataDto data)
        {
            return GetResponse<GetOrCreateComponentResponseDto>();
        }

        public GetRootComponentResponseDto GetRootComponent()
        {
            return GetResponse<GetRootComponentResponseDto>();
        }

        public GetComponentByIdResponseDto GetComponentById(Guid componentId)
        {
            return GetResponse<GetComponentByIdResponseDto>();
        }

        public GetComponentBySystemNameResponseDto GetComponentBySystemName(Guid? parentId, string systemName)
        {
            return GetResponse<GetComponentBySystemNameResponseDto>();
        }

        public GetComponentBySystemNameResponseDto GetComponentBySystemName(GetComponentBySystemNameRequestDataDto data)
        {
            return GetResponse<GetComponentBySystemNameResponseDto>();
        }

        public GetChildComponentsResponseDto GetChildComponents(Guid componentId)
        {
            return GetResponse<GetChildComponentsResponseDto>();
        }

        public UpdateComponentResponseDto UpdateComponent(UpdateComponentRequestDataDto data)
        {
            return GetResponse<UpdateComponentResponseDto>();
        }

        public GetComponentTotalStateResponseDto GetComponentTotalState(Guid componentId, bool recalc)
        {
            return GetResponse<GetComponentTotalStateResponseDto>();
        }

        public GetComponentInternalStateResponseDto GetComponentInternalState(Guid componentId, bool recalc)
        {
            return GetResponse<GetComponentInternalStateResponseDto>();
        }

        public SetComponentEnableResponseDto SetComponentEnable(Guid componentId)
        {
            return GetResponse<SetComponentEnableResponseDto>();
        }

        public SetComponentDisableResponseDto SetComponentDisable(Guid componentId, DateTime? toDate, string comment)
        {
            return GetResponse<SetComponentDisableResponseDto>();
        }

        public DeleteComponentResponseDto DeleteComponent(Guid componentId)
        {
            return GetResponse<DeleteComponentResponseDto>();
        }

        public GetComponentTypeResponseDto GetComponentType(GetComponentTypeRequestDataDto data)
        {
            return GetResponse<GetComponentTypeResponseDto>();
        }

        public GetOrCreateComponentTypeResponseDto GetOrCreateComponentType(GetOrCreateComponentTypeRequestDataDto data)
        {
            return GetResponse<GetOrCreateComponentTypeResponseDto>();
        }

        public UpdateComponentTypeResponseDto UpdateComponentType(UpdateComponentTypeRequestDataDto data)
        {
            return GetResponse<UpdateComponentTypeResponseDto>();
        }

        public SendEventResponseDto SendEvent(SendEventRequestDataDto data)
        {
            return GetResponse<SendEventResponseDto>();
        }

        public GetEventByIdResponseDto GetEventById(Guid eventId)
        {
            return GetResponse<GetEventByIdResponseDto>();
        }

        public GetEventsResponseDto GetEvents(GetEventsRequestDataDto data)
        {
            return GetResponse<GetEventsResponseDto>();
        }

        public JoinEventsResponseDto JoinEvents(List<JoinEventRequestDataDto> data)
        {
            return GetResponse<JoinEventsResponseDto>();
        }

        public GetOrCreateUnitTestResponseDto GetOrCreateUnitTest(GetOrCreateUnitTestRequestDataDto data)
        {
            return GetResponse<GetOrCreateUnitTestResponseDto>();
        }

        public GetOrCreateUnitTestTypeResponseDto GetOrCreateUnitTestType(GetOrCreateUnitTestTypeRequestDataDto data)
        {
            return GetResponse<GetOrCreateUnitTestTypeResponseDto>();
        }

        public SendUnitTestResultResponseDto SendUnitTestResult(SendUnitTestResultRequestDataDto data)
        {
            return GetResponse<SendUnitTestResultResponseDto>();
        }

        public SendUnitTestResultsResponseDto SendUnitTestResults(SendUnitTestResultRequestDataDto[] data)
        {
            return GetResponse<SendUnitTestResultsResponseDto>();
        }

        public GetUnitTestStateResponseDto GetUnitTestState(Guid unitTestId)
        {
            return GetResponse<GetUnitTestStateResponseDto>();
        }

        public SetUnitTestEnableResponseDto SetUnitTestEnable(Guid unitTestId)
        {
            return GetResponse<SetUnitTestEnableResponseDto>();
        }

        public SetUnitTestDisableResponseDto SetUnitTestDisable(SetUnitTestDisableRequestDataDto data)
        {
            return GetResponse<SetUnitTestDisableResponseDto>();
        }

        public SendMetricResponseDto SendMetric(SendMetricRequestDataDto data)
        {
            return GetResponse<SendMetricResponseDto>();
        }

        public SendMetricsResponseDto SendMetrics(List<SendMetricRequestDataDto> data)
        {
            return GetResponse<SendMetricsResponseDto>();
        }

        public GetMetricsHistoryResponseDto GetMetricsHistory(GetMetricsHistoryRequestDataDto filter)
        {
            return GetResponse<GetMetricsHistoryResponseDto>();
        }

        public GetMetricsResponseDto GetMetrics(Guid componentId)
        {
            return GetResponse<GetMetricsResponseDto>();
        }

        public GetMetricResponseDto GetMetric(Guid componentId, string metricName)
        {
            return GetResponse<GetMetricResponseDto>();
        }

        public SendLogResponseDto SendLog(SendLogRequestDataDto data)
        {
            return GetResponse<SendLogResponseDto>();
        }

        public SendLogsResponseDto SendLogs(SendLogRequestDataDto[] data)
        {
            return GetResponse<SendLogsResponseDto>();
        }

        public GetLogsResponseDto GetLogs(GetLogsRequestDataDto filter)
        {
            return GetResponse<GetLogsResponseDto>();
        }

        public GetLogConfigResponseDto GetLogConfig(Guid componentId)
        {
            return GetResponse<GetLogConfigResponseDto>();
        }

        public GetChangedWebLogConfigsResponseDto GetChangedWebLogConfigs(DateTime lastUpdateDate, Guid[] componentIds)
        {
            return GetResponse<GetChangedWebLogConfigsResponseDto>();
        }

        public bool IsFake
        {
            get { return true; }
        }

    }
}
