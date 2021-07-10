using System;
using System.Collections.Generic;
using Zidium.Api.Dto;

namespace Zidium.Api
{
    public class ApiService : IApiService
    {
        public ApiService(IDtoService dtoService)
        {
            if (dtoService == null)
            {
                throw new ArgumentNullException(nameof(dtoService));
            }
            DtoService = dtoService;
        }

        public ApiService(IDtoService dtoService, AccessTokenDto accessToken)
        {
            if (dtoService == null)
            {
                throw new ArgumentNullException(nameof(dtoService));
            }
            DtoService = dtoService;
            AccessToken = accessToken;
        }

        public AccessTokenDto AccessToken { get; protected set; }

        public IDtoService DtoService { get; set; }

        public void SetAccessToken(AccessTokenDto accessToken)
        {
            if (accessToken == null)
            {
                throw new ArgumentNullException(nameof(accessToken));
            }
            AccessToken = accessToken;
        }

        public bool IsFake
        {
            get { return false; }
        }

        protected AccessTokenDto GetAccessTokenDto()
        {
            // TODO cashe in variable
            return new AccessTokenDto()
            {
                SecretKey = AccessToken.SecretKey,
                Program = AccessToken.Program
            };
        }

        protected TRequest GetRequest<TRequest>() where TRequest : RequestDto, new()
        {
            return new TRequest()
            {
                Token = GetAccessTokenDto()
            };
        }

        protected TRequest GetRequest<TRequest, TRequestData>(TRequestData data) where TRequest : RequestDtoT<TRequestData>, new()
        {
            return new TRequest()
            {
                Token = GetAccessTokenDto(),
                Data = data
            };
        }

        public GetEchoResponseDto GetEcho(string message)
        {
            var request = GetRequest<GetEchoRequestDto, GetEchoRequestDataDto>(
                new GetEchoRequestDataDto()
                {
                    Message = message
                });
            return DtoService.GetEcho(request);
        }

        public GetServerTimeResponseDto GetServerTime()
        {
            return DtoService.GetServerTime();
        }

        public GetRootControlDataResponseDto GetRootControlData()
        {
            var request = GetRequest<GetRootControlDataRequestDto>();
            return DtoService.GetRootControlData(request);
        }

        public GetComponentControlByIdResponseDto GetComponentControlById(Guid componentId)
        {
            var request = GetRequest<GetComponentControlByIdRequestDto, GetComponentControlByIdRequestDataDto>(
                new GetComponentControlByIdRequestDataDto()
                {
                    ComponentId = componentId
                });
            return DtoService.GetComponentControlById(request);
        }

        public GetOrCreateComponentResponseDto GetOrCreateComponent(GetOrCreateComponentRequestDataDto data)
        {
            var request = GetRequest<GetOrCreateComponentRequestDto, GetOrCreateComponentRequestDataDto>(data);
            return DtoService.GetOrCreateComponent(request);
        }

        public GetRootComponentResponseDto GetRootComponent()
        {
            var request = GetRequest<GetRootComponentRequestDto>();
            return DtoService.GetRootComponent(request);
        }

        public GetComponentByIdResponseDto GetComponentById(Guid componentId)
        {
            var request = GetRequest<GetComponentByIdRequestDto, GetComponentByIdRequestDataDto>(
                new GetComponentByIdRequestDataDto()
                {
                    ComponentId = componentId
                });
            return DtoService.GetComponentById(request);
        }

        public GetComponentBySystemNameResponseDto GetComponentBySystemName(Guid? parentId, string systemName)
        {
            var request = GetRequest<GetComponentBySystemNameRequestDto, GetComponentBySystemNameRequestDataDto>(
                new GetComponentBySystemNameRequestDataDto()
                {
                    ParentId = parentId,
                    SystemName = systemName
                });
            return DtoService.GetComponentBySystemName(request);
        }

        public GetComponentBySystemNameResponseDto GetComponentBySystemName(GetComponentBySystemNameRequestDataDto data)
        {
            var request = GetRequest<GetComponentBySystemNameRequestDto, GetComponentBySystemNameRequestDataDto>(data);
            return DtoService.GetComponentBySystemName(request);
        }

        public GetChildComponentsResponseDto GetChildComponents(Guid componentId)
        {
            var request = GetRequest<GetChildComponentsRequestDto, GetChildComponentsRequestDataDto>(
                new GetChildComponentsRequestDataDto()
                {
                    ComponentId = componentId
                });
            return DtoService.GetChildComponents(request);
        }

        public UpdateComponentResponseDto UpdateComponent(UpdateComponentRequestDataDto data)
        {
            var request = GetRequest<UpdateComponentRequestDto, UpdateComponentRequestDataDto>(data);
            return DtoService.UpdateComponent(request);
        }

        public GetComponentTotalStateResponseDto GetComponentTotalState(Guid componentId, bool recalc)
        {
            var request = GetRequest<GetComponentTotalStateRequestDto, GetComponentTotalStateRequestDataDto>(
                new GetComponentTotalStateRequestDataDto()
                {
                    ComponentId = componentId,
                    Recalc = recalc
                });
            return DtoService.GetComponentTotalState(request);
        }

        public GetComponentInternalStateResponseDto GetComponentInternalState(Guid componentId, bool recalc)
        {
            var request = GetRequest<GetComponentInternalStateRequestDto, GetComponentInternalStateRequestDataDto>(
                new GetComponentInternalStateRequestDataDto()
                {
                    ComponentId = componentId,
                    Recalc = recalc
                });
            return DtoService.GetComponentInternalState(request);
        }

        public SetComponentEnableResponseDto SetComponentEnable(Guid componentId)
        {
            var request = GetRequest<SetComponentEnableRequestDto, SetComponentEnableRequestDataDto>(
                new SetComponentEnableRequestDataDto()
                {
                    ComponentId = componentId
                });
            return DtoService.SetComponentEnable(request);
        }

        public SetComponentDisableResponseDto SetComponentDisable(Guid componentId, DateTime? toDate, string comment)
        {
            var request = GetRequest<SetComponentDisableRequestDto, SetComponentDisableRequestDataDto>(
                new SetComponentDisableRequestDataDto()
                {
                    ComponentId = componentId,
                    ToDate = toDate,
                    Comment = comment
                });
            return DtoService.SetComponentDisable(request);
        }

        public DeleteComponentResponseDto DeleteComponent(Guid componentId)
        {
            var request = GetRequest<DeleteComponentRequestDto, DeleteComponentRequestDataDto>(
                new DeleteComponentRequestDataDto()
                {
                    ComponentId = componentId
                });
            return DtoService.DeleteComponent(request);
        }

        public GetComponentTypeResponseDto GetComponentType(GetComponentTypeRequestDataDto data)
        {
            var request = GetRequest<GetComponentTypeRequestDto, GetComponentTypeRequestDataDto>(data);
            return DtoService.GetComponentType(request);
        }

        public GetOrCreateComponentTypeResponseDto GetOrCreateComponentType(GetOrCreateComponentTypeRequestDataDto data)
        {
            var request = GetRequest<GetOrCreateComponentTypeRequestDto, GetOrCreateComponentTypeRequestDataDto>(data);
            return DtoService.GetOrCreateComponentType(request);
        }

        public UpdateComponentTypeResponseDto UpdateComponentType(UpdateComponentTypeRequestDataDto data)
        {
            var request = GetRequest<UpdateComponentTypeRequestDto, UpdateComponentTypeRequestDataDto>(data);
            return DtoService.UpdateComponentType(request);
        }

        public SendEventResponseDto SendEvent(SendEventRequestDataDto data)
        {
            var request = GetRequest<SendEventRequestDto, SendEventRequestDataDto>(data);
            return DtoService.SendEvent(request);
        }

        public GetEventByIdResponseDto GetEventById(Guid eventId)
        {
            var request = GetRequest<GetEventByIdRequestDto, GetEventByIdRequestDataDto>(
                new GetEventByIdRequestDataDto()
                {
                    EventId = eventId
                });
            return DtoService.GetEventById(request);
        }

        public GetEventsResponseDto GetEvents(GetEventsRequestDataDto data)
        {
            var request = GetRequest<GetEventsRequestDto, GetEventsRequestDataDto>(data);
            return DtoService.GetEvents(request);
        }

        public JoinEventsResponseDto JoinEvents(List<JoinEventRequestDataDto> data)
        {
            var request = GetRequest<JoinEventsRequestDto, List<JoinEventRequestDataDto>>(data);
            return DtoService.JoinEvents(request);
        }

        public GetOrCreateUnitTestResponseDto GetOrCreateUnitTest(GetOrCreateUnitTestRequestDataDto data)
        {
            var request = GetRequest<GetOrCreateUnitTestRequestDto, GetOrCreateUnitTestRequestDataDto>(data);
            return DtoService.GetOrCreateUnitTest(request);
        }

        public GetOrCreateUnitTestTypeResponseDto GetOrCreateUnitTestType(GetOrCreateUnitTestTypeRequestDataDto data)
        {
            var request = GetRequest<GetOrCreateUnitTestTypeRequestDto, GetOrCreateUnitTestTypeRequestDataDto>(data);
            return DtoService.GetOrCreateUnitTestType(request);
        }

        public SendUnitTestResultResponseDto SendUnitTestResult(SendUnitTestResultRequestDataDto data)
        {
            var request = GetRequest<SendUnitTestResultRequestDto, SendUnitTestResultRequestDataDto>(data);
            return DtoService.SendUnitTestResult(request);
        }

        public SendUnitTestResultsResponseDto SendUnitTestResults(SendUnitTestResultRequestDataDto[] data)
        {
            var request = GetRequest<SendUnitTestResultsRequestDto, SendUnitTestResultRequestDataDto[]>(data);
            return DtoService.SendUnitTestResults(request);
        }

        public GetUnitTestStateResponseDto GetUnitTestState(Guid unitTestId)
        {
            var request = GetRequest<GetUnitTestStateRequestDto, GetUnitTestStateRequestDataDto>(
                new GetUnitTestStateRequestDataDto()
                {
                    UnitTestId = unitTestId
                });
            return DtoService.GetUnitTestState(request);
        }

        public SetUnitTestEnableResponseDto SetUnitTestEnable(Guid unitTestId)
        {
            var request = GetRequest<SetUnitTestEnableRequestDto, SetUnitTestEnableRequestDataDto>(
                new SetUnitTestEnableRequestDataDto()
                {
                    UnitTestId = unitTestId
                });
            return DtoService.SetUnitTestEnable(request);
        }

        public SetUnitTestDisableResponseDto SetUnitTestDisable(SetUnitTestDisableRequestDataDto data)
        {
            var request = GetRequest<SetUnitTestDisableRequestDto, SetUnitTestDisableRequestDataDto>(data);
            return DtoService.SetUnitTestDisable(request);
        }

        public SendMetricResponseDto SendMetric(SendMetricRequestDataDto data)
        {
            var request = GetRequest<SendMetricRequestDto, SendMetricRequestDataDto>(data);
            return DtoService.SendMetric(request);
        }

        public SendMetricsResponseDto SendMetrics(List<SendMetricRequestDataDto> data)
        {
            var request = GetRequest<SendMetricsRequestDto, List<SendMetricRequestDataDto>>(data);
            return DtoService.SendMetrics(request);
        }

        public GetMetricsHistoryResponseDto GetMetricsHistory(GetMetricsHistoryRequestDataDto data)
        {
            var request = GetRequest<GetMetricsHistoryRequestDto, GetMetricsHistoryRequestDataDto>(data);
            return DtoService.GetMetricsHistory(request);
        }

        public GetMetricsResponseDto GetMetrics(Guid componentId)
        {
            var request = GetRequest<GetMetricsRequestDto, GetMetricsRequestDataDto>(
                new GetMetricsRequestDataDto()
                {
                    ComponentId = componentId
                });
            return DtoService.GetMetrics(request);
        }

        public GetMetricResponseDto GetMetric(Guid componentId, string metricName)
        {
            var request = GetRequest<GetMetricRequestDto, GetMetricRequestDataDto>(
                new GetMetricRequestDataDto()
                {
                    ComponentId = componentId,
                    Name = metricName
                });
            return DtoService.GetMetric(request);
        }

        public SendLogResponseDto SendLog(SendLogRequestDataDto data)
        {
            var request = GetRequest<SendLogRequestDto, SendLogRequestDataDto>(data);
            return DtoService.SendLog(request);
        }

        public SendLogsResponseDto SendLogs(SendLogRequestDataDto[] data)
        {
            var request = GetRequest<SendLogsRequestDto, SendLogRequestDataDto[]>(data);
            return DtoService.SendLogs(request);
        }

        public GetLogsResponseDto GetLogs(GetLogsRequestDataDto data)
        {
            var request = GetRequest<GetLogsRequestDto, GetLogsRequestDataDto>(data);
            return DtoService.GetLogs(request);
        }

        public GetLogConfigResponseDto GetLogConfig(Guid componentId)
        {
            var request = GetRequest<GetLogConfigRequestDto, GetLogConfigRequestDataDto>(
                new GetLogConfigRequestDataDto()
                {
                    ComponentId = componentId
                });
            return DtoService.GetLogConfig(request);
        }

        public GetChangedWebLogConfigsResponseDto GetChangedWebLogConfigs(DateTime lastUpdateDate, Guid[] componentIds)
        {
            var request = GetRequest<GetChangedWebLogConfigsRequestDto, GetChangedWebLogConfigsRequestDataDto>(
                new GetChangedWebLogConfigsRequestDataDto()
                {
                    LastUpdateDate = lastUpdateDate,
                    ComponentIds = componentIds
                });
            return DtoService.GetChangedWebLogConfigs(request);
        }
    }
}
