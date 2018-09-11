using System;
using System.Collections.Generic;
using Zidium.Api.Common;
using Zidium.Api.Dto;

namespace Zidium.Api
{
    public class ApiService : IApiService
    {
        public ApiService(IDtoService dtoService)
        {
            if (dtoService == null)
            {
                throw new ArgumentNullException("dtoService");
            }
            DtoService = dtoService;
        }

        public ApiService(IDtoService dtoService, AccessToken accessToken)
        {
            if (dtoService == null)
            {
                throw new ArgumentNullException("dtoService");
            }
            DtoService = dtoService;
            AccessToken = accessToken;
        }

        public AccessToken AccessToken { get; protected set; }

        public IDtoService DtoService { get; set; }

        protected TResponse CreateResponse<TResponse>(Response response)
            where TResponse : Response, new()
        {
            return new TResponse()
            {
                Code = response.Code,
                ErrorMessage = response.ErrorMessage
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
            get { return false; }
        }

        protected AccessTokenDto GetAccessTokenDto()
        {
            return new AccessTokenDto()
            {
                SecretKey = AccessToken.SecretKey,
                Program = AccessToken.Program
            };
        }

        public GetEchoResponse GetEcho(string message)
        {
            var dataDto = DataConverter.GetEchoRequestDtoData(message);
            var request = new GetEchoRequestDto()
            {
                Data = dataDto,
                Token = GetAccessTokenDto()
            };
            var responseDto = DtoService.GetEcho(request);
            var response = CreateResponse<GetEchoResponse>(responseDto);
            response.InternalData = responseDto.Data;
            return response;
        }

        public GetServerTimeResponse GetServerTime()
        {
            var responseDto = DtoService.GetServerTime();
            var response = CreateResponse<GetServerTimeResponse>(responseDto);
            response.InternalData = DataConverter.GetServerTimeResponse(responseDto.Data);
            return response;
        }

        public GetRootControlDataResponse GetRootControlData()
        {
            var request = new GetRootControlDataRequestDto()
            {
                Data = null,
                Token = GetAccessTokenDto()
            };
            var responseDto = DtoService.GetRootControlData(request);
            var response = CreateResponse<GetRootControlDataResponse>(responseDto);
            response.InternalData = DataConverter.GetComponentControlData(responseDto.Data);
            return response;
        }

        public GetComponentControlByIdResponse GetComponentControlById(Guid componentId)
        {
            var request = new GetComponentControlByIdRequestDto()
            {
                Data = new GetComponentControlByIdRequestDtoData()
                {
                    ComponentId = componentId
                },
                Token = GetAccessTokenDto()
            };
            var responseDto = DtoService.GetComponentControlById(request);
            var response = CreateResponse<GetComponentControlByIdResponse>(responseDto);
            response.InternalData = DataConverter.GetComponentControlData(responseDto.Data);
            return response;
        }

        public GetOrCreateComponentResponse GetOrCreateComponent(Guid parentId, GetOrCreateComponentData data)
        {
            var dataDto = DataConverter.GetOrCreateComponentDataDto(parentId, data);
            var request = new GetOrCreateComponentRequestDto()
            {
                Data = dataDto,
                Token = GetAccessTokenDto()
            };
            var responseDto = DtoService.GetOrCreateComponent(request);
            var response = CreateResponse<GetOrCreateComponentResponse>(responseDto);
            response.InternalData = DataConverter.GetComponentControlData(responseDto.Data);
            return response;
        }

        public GetRootComponentResponse GetRootComponent()
        {
            var request = new GetRootComponentRequestDto()
            {
                Data = null,
                Token = GetAccessTokenDto()
            };
            var responseDto = DtoService.GetRootComponent(request);
            var response = CreateResponse<GetRootComponentResponse>(responseDto);
            response.InternalData = DataConverter.GetComponentInfo(responseDto.Data);
            return response;
        }

        public GetComponentByIdResponse GetComponentById(Guid componentId)
        {
            var dataDto = new GetComponentByIdRequestDtoData()
            {
                ComponentId = componentId
            };
            var request = new GetComponentByIdRequestDto()
            {
                Data = dataDto,
                Token = GetAccessTokenDto()
            };
            var responseDto = DtoService.GetComponentById(request);
            var response = CreateResponse<GetComponentByIdResponse>(responseDto);
            response.InternalData = DataConverter.GetComponentInfo(responseDto.Data);
            return response;
        }

        public GetComponentBySystemNameResponse GetComponentBySystemName(string systemName)
        {
            if (systemName == null)
            {
                throw new ArgumentNullException("systemName");
            }
            var data = new GetComponentBySystemNameData()
            {
                SystemName = systemName
            };
            return GetComponentBySystemName(data);
        }

        public GetComponentBySystemNameResponse GetComponentBySystemName(GetComponentBySystemNameData data)
        {
            var dataDto = DataConverter.GetComponentBySystemNameRequestDtoData(data);
            var request = new GetComponentBySystemNameRequestDto()
            {
                Data = dataDto,
                Token = GetAccessTokenDto()
            };
            var responseDto = DtoService.GetComponentBySystemName(request);
            var response = CreateResponse<GetComponentBySystemNameResponse>(responseDto);
            response.InternalData = DataConverter.GetComponentInfo(responseDto.Data);
            return response;
        }

        public GetChildComponentsResponse GetChildComponents(Guid componentId)
        {
            var dataDto = new GetChildComponentsRequestDtoData()
            {
                ComponentId = componentId
            };
            var request = new GetChildComponentsRequestDto()
            {
                Data = dataDto,
                Token = GetAccessTokenDto()
            };
            var responseDto = DtoService.GetChildComponents(request);
            var response = CreateResponse<GetChildComponentsResponse>(responseDto);
            response.InternalData = DataConverter.GetComponentInfoList(responseDto.Data);
            return response;
        }

        public UpdateComponentResponse UpdateComponent(Guid componentId, UpdateComponentData data)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }
            var dataDto = DataConverter.GetUpdateComponentRequestDtoData(componentId, data);
            var request = new UpdateComponentRequestDto()
            {
                Data = dataDto,
                Token = GetAccessTokenDto()
            };
            var responseDto = DtoService.UpdateComponent(request);
            var response = CreateResponse<UpdateComponentResponse>(responseDto);
            response.InternalData = DataConverter.GetComponentInfo(responseDto.Data);
            return response;
        }

        public GetComponentTotalStateResponse GetComponentTotalState(Guid componentId, bool recalc)
        {
            var dataDto = new GetComponentTotalStateRequestDtoData()
            {
                ComponentId = componentId,
                Recalc = recalc
            };
            var request = new GetComponentTotalStateRequestDto()
            {
                Data = dataDto,
                Token = GetAccessTokenDto()
            };
            var responseDto = DtoService.GetComponentTotalState(request);
            var response = CreateResponse<GetComponentTotalStateResponse>(responseDto);
            response.InternalData = DataConverter.GetStatusDataInfo(responseDto.Data);
            return response;
        }

        public GetComponentInternalStateResponse GetComponentInternalState(Guid componentId, bool recalc)
        {
            var dataDto = new GetComponentInternalStateRequestDtoData()
            {
                ComponentId = componentId,
                Recalc = recalc
            };
            var request = new GetComponentInternalStateRequestDto()
            {
                Data = dataDto,
                Token = GetAccessTokenDto()
            };
            var responseDto = DtoService.GetComponentInternalState(request);
            var response = CreateResponse<GetComponentInternalStateResponse>(responseDto);
            response.InternalData = DataConverter.GetStatusDataInfo(responseDto.Data);
            return response;
        }

        public SetComponentEnableResponse SetComponentEnable(Guid componentId)
        {
            var dataDto = new SetComponentEnableRequestDtoData()
            {
                ComponentId = componentId
            };
            var request = new SetComponentEnableRequestDto()
            {
                Data = dataDto,
                Token = GetAccessTokenDto()
            };
            var responseDto = DtoService.SetComponentEnable(request);
            var response = CreateResponse<SetComponentEnableResponse>(responseDto);
            response.InternalData = responseDto.Data;
            return response;
        }

        public SetComponentDisableResponse SetComponentDisable(Guid componentId, DateTime? toDate, string comment)
        {
            var dataDto = new SetComponentDisableRequestDtoData()
            {
                ComponentId = componentId,
                ToDate = toDate,
                Comment = comment
            };
            var request = new SetComponentDisableRequestDto()
            {
                Data = dataDto,
                Token = GetAccessTokenDto()
            };
            var responseDto = DtoService.SetComponentDisable(request);
            var response = CreateResponse<SetComponentDisableResponse>(responseDto);
            response.InternalData = responseDto.Data;
            return response;
        }

        public DeleteComponentResponse DeleteComponent(Guid componentId)
        {
            var dataDto = new DeleteComponentRequestDtoData()
            {
                ComponentId = componentId
            };
            var request = new DeleteComponentRequestDto()
            {
                Data = dataDto,
                Token = GetAccessTokenDto()
            };
            var responseDto = DtoService.DeleteComponent(request);
            var response = CreateResponse<DeleteComponentResponse>(responseDto);
            response.InternalData = responseDto.Data;
            return response;
        }

        public GetComponentTypeResponse GetComponentType(GetComponentTypeData data)
        {
            var dataDto = new GetComponentTypeRequestDtoData()
            {
                Id = data.Id,
                SystemName = data.SystemName
            };
            var request = new GetComponentTypeRequestDto()
            {
                Data = dataDto,
                Token = GetAccessTokenDto()
            };
            var responseDto = DtoService.GetComponentType(request);
            var response = CreateResponse<GetComponentTypeResponse>(responseDto);
            response.InternalData = DataConverter.GetComponentTypeInfo(responseDto.Data);
            return response;
        }

        public GetOrCreateComponentTypeResponse GetOrCreateComponentType(GetOrCreateComponentTypeData data)
        {
            var dataDto = DataConverter.GetOrCreateComponentTypeRequestDtoData(data);
            var request = new GetOrCreateComponentTypeRequestDto
            {
                Data = dataDto,
                Token = GetAccessTokenDto()
            };
            var responseDto = DtoService.GetOrCreateComponentType(request);
            var response = CreateResponse<GetOrCreateComponentTypeResponse>(responseDto);
            response.InternalData = DataConverter.GetComponentTypeInfo(responseDto.Data);
            return response;
        }

        public UpdateComponentTypeResponse UpdateComponentType(UpdateComponentTypeData data)
        {
            var dataDto = DataConverter.GetUpdateComponentTypeRequestDtoData(data);
            var request = new UpdateComponentTypeRequestDto
            {
                Data = dataDto,
                Token = GetAccessTokenDto()
            };
            var responseDto = DtoService.UpdateComponentType(request);
            var response = CreateResponse<UpdateComponentTypeResponse>(responseDto);
            response.InternalData = DataConverter.GetComponentTypeInfo(responseDto.Data);
            return response;
        }

        public SendEventResponse SendEvent(SendEventData data)
        {
            var dataDto = DataConverter.GetSendEventRequestDtoData(data);
            var request = new SendEventRequestDto
            {
                Data = dataDto,
                Token = GetAccessTokenDto()
            };
            var responseDto = DtoService.SendEvent(request);
            var response = CreateResponse<SendEventResponse>(responseDto);
            response.InternalData = DataConverter.GetsSendEventResponseDtoData(responseDto.Data);
            return response;
        }

        public GetEventByIdResponse GetEventById(Guid eventId)
        {
            var dataDto = new GetEventByIdRequestDtoData()
            {
                EventId = eventId
            };
            var request = new GetEventByIdRequestDto
            {
                Data = dataDto,
                Token = GetAccessTokenDto()
            };
            var responseDto = DtoService.GetEventById(request);
            var response = CreateResponse<GetEventByIdResponse>(responseDto);
            response.InternalData = DataConverter.GetEventInfo(responseDto.Data);
            return response;
        }

        public GetEventsResponse GetEvents(GetEventsData data)
        {
            var dataDto = DataConverter.GetEventsRequestDtoData(data);
            var request = new GetEventsRequestDto
            {
                Data = dataDto,
                Token = GetAccessTokenDto()
            };
            var responseDto = DtoService.GetEvents(request);
            var response = CreateResponse<GetEventsResponse>(responseDto);
            response.InternalData = DataConverter.GetEventInfoList(responseDto.Data);
            return response;
        }

        public JoinEventsResponse JoinEvents(List<JoinEventData> data)
        {
            var dataDto = DataConverter.GetJoinEventDtoList(data);
            var request = new JoinEventsRequestDto()
            {
                Data = dataDto,
                Token = GetAccessTokenDto()
            };
            var responseDto = DtoService.JoinEvents(request);
            var response = CreateResponse<JoinEventsResponse>(responseDto);
            response.InternalData = responseDto.Data;
            return response;
        }

        public GetOrCreateUnitTestResponse GetOrCreateUnitTest(Guid componentId, GetOrCreateUnitTestData data)
        {
            var dataDto = DataConverter.GetOrCreateUnitTestRequestDtoData(componentId, data);
            var request = new GetOrCreateUnitTestRequestDto()
            {
                Data = dataDto,
                Token = GetAccessTokenDto()
            };
            var responseDto = DtoService.GetOrCreateUnitTest(request);
            var response = CreateResponse<GetOrCreateUnitTestResponse>(responseDto);
            response.InternalData = DataConverter.GetUnitTestInfo(responseDto.Data);
            return response;
        }

        public GetOrCreateUnitTestTypeResponse GetOrCreateUnitTestType(GetOrCreateUnitTestTypeData data)
        {
            var dataDto = DataConverter.GetOrCreateUnitTestTypeRequestDtoData(data);
            var request = new GetOrCreateUnitTestTypeRequestDto()
            {
                Data = dataDto,
                Token = GetAccessTokenDto()
            };
            var responseDto = DtoService.GetOrCreateUnitTestType(request);
            var response = CreateResponse<GetOrCreateUnitTestTypeResponse>(responseDto);
            response.InternalData = DataConverter.GetUnitTestTypeInfo(responseDto.Data);
            return response;
        }

        public SendUnitTestResultResponse SendUnitTestResult(Guid unitTestId, SendUnitTestResultData data)
        {
            var dataDto = DataConverter.GetSendUnitTestResultRequestDtoData(unitTestId, data);
            var request = new SendUnitTestResultRequestDto()
            {
                Data = dataDto,
                Token = GetAccessTokenDto()
            };
            var responseDto = DtoService.SendUnitTestResult(request);
            var response = CreateResponse<SendUnitTestResultResponse>(responseDto);
            response.InternalData = responseDto.Data;
            return response;
        }

        public SendUnitTestResultsResponse SendUnitTestResults(SendUnitTestResultsData[] data)
        {
            var dataDto = DataConverter.GetSendUnitTestResultsRequestDtoData(data);
            var request = new SendUnitTestResultsRequestDto()
            {
                Data = dataDto,
                Token = GetAccessTokenDto()
            };
            var responseDto = DtoService.SendUnitTestResults(request);
            var response = CreateResponse<SendUnitTestResultsResponse>(responseDto);
            response.InternalData = responseDto.Data;
            return response;
        }

        public GetUnitTestStateResponse GetUnitTestState(Guid unitTestId)
        {
            var dataDto = new GetUnitTestStateRequestDtoData()
            {
                UnitTestId = unitTestId
            };
            var request = new GetUnitTestStateRequestDto()
            {
                Data = dataDto,
                Token = GetAccessTokenDto()
            };
            var responseDto = DtoService.GetUnitTestState(request);
            var response = CreateResponse<GetUnitTestStateResponse>(responseDto);
            response.InternalData = DataConverter.GetStatusDataInfo(responseDto.Data);
            return response;
        }

        public SetUnitTestEnableResponse SetUnitTestEnable(Guid unitTestId)
        {
            var dataDto = new SetUnitTestEnableRequestDtoData()
            {
                UnitTestId = unitTestId
            };
            var request = new SetUnitTestEnableRequestDto()
            {
                Data = dataDto,
                Token = GetAccessTokenDto()
            };
            var responseDto = DtoService.SetUnitTestEnable(request);
            var response = CreateResponse<SetUnitTestEnableResponse>(responseDto);
            response.InternalData = responseDto.Data;
            return response;
        }

        public SetUnitTestDisableResponse SetUnitTestDisable(Guid unitTestId, SetUnitTestDisableRequestData data)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }
            var dataDto = new SetUnitTestDisableRequestDtoData()
            {
                UnitTestId = unitTestId
            };
            var request = new SetUnitTestDisableRequestDto()
            {
                Data = dataDto,
                Token = GetAccessTokenDto()
            };
            var responseDto = DtoService.SetUnitTestDisable(request);
            var response = CreateResponse<SetUnitTestDisableResponse>(responseDto);
            response.InternalData = responseDto.Data;
            return response;
        }

        public SendMetricResponse SendMetric(Guid componentId, SendMetricData data)
        {
            var dataDto = DataConverter.GetSendMetricRequestDtoData(componentId, data);
            var request = new SendMetricRequestDto()
            {
                Data = dataDto,
                Token = GetAccessTokenDto()
            };
            var responseDto = DtoService.SendMetric(request);
            var response = CreateResponse<SendMetricResponse>(responseDto);
            response.InternalData = DataConverter.GetMetricInfo(responseDto.Data);
            return response;
        }

        public SendMetricsResponse SendMetrics(Guid componentId, List<SendMetricData> data)
        {
            var dataDto = DataConverter.GetSendMetricRequestDtoDataList(componentId, data);
            var request = new SendMetricsRequestDto()
            {
                Data = dataDto,
                Token = GetAccessTokenDto()
            };
            var responseDto = DtoService.SendMetrics(request);
            var response = CreateResponse<SendMetricsResponse>(responseDto);
            response.InternalData = responseDto.Data;
            return response;
        }

        public GetMetricsHistoryResponse GetMetricsHistory(Guid componentId, GetMetricsHistoryFilter filter)
        {
            var dataDto = DataConverter.GetMetricsHistoryRequestDtoData(componentId, filter);
            var request = new GetMetricsHistoryRequestDto()
            {
                Data = dataDto,
                Token = GetAccessTokenDto()
            };
            var responseDto = DtoService.GetMetricsHistory(request);
            var response = CreateResponse<GetMetricsHistoryResponse>(responseDto);
            response.InternalData = DataConverter.GetMetricInfoList(responseDto.Data);
            return response;
        }

        public GetMetricsResponse GetMetrics(Guid componentId)
        {
            var dataDto = new GetMetricsRequestDtoData()
            {
                ComponentId = componentId
            };
            var request = new GetMetricsRequestDto()
            {
                Data = dataDto,
                Token = GetAccessTokenDto()
            };
            var responseDto = DtoService.GetMetrics(request);
            var response = CreateResponse<GetMetricsResponse>(responseDto);
            response.InternalData = DataConverter.GetMetricInfoList(responseDto.Data);
            return response;
        }

        public GetMetricResponse GetMetric(Guid componentId, string metricName)
        {
            var dataDto = new GetMetricRequestDtoData()
            {
                ComponentId = componentId,
                Name = metricName
            };
            var request = new GetMetricRequestDto()
            {
                Data = dataDto,
                Token = GetAccessTokenDto()
            };
            var responseDto = DtoService.GetMetric(request);
            var response = CreateResponse<GetMetricResponse>(responseDto);
            response.InternalData = DataConverter.GetMetricInfo(responseDto.Data);
            return response;
        }

        public SendLogResponse SendLog(SendLogData data)
        {
            var dataDto = DataConverter.GetSendLogDto(data);
            var request = new SendLogRequestDto()
            {
                Data = dataDto,
                Token = GetAccessTokenDto()
            };
            var responseDto = DtoService.SendLog(request);
            var response = CreateResponse<SendLogResponse>(responseDto);
            response.InternalData = responseDto.Data;
            return response;
        }

        public SendLogsResponse SendLogs(SendLogData[] data)
        {
            var dataDto = DataConverter.GetSendLogDtoList(data);
            var request = new SendLogsRequestDto()
            {
                Data = dataDto,
                Token = GetAccessTokenDto()
            };
            var responseDto = DtoService.SendLogs(request);
            var response = CreateResponse<SendLogsResponse>(responseDto);
            response.InternalData = responseDto.Data;
            return response;
        }

        public GetLogsResponse GetLogs(Guid componentId, GetLogsFilter filter)
        {
            var dataDto = DataConverter.GetLogsRequestDtoData(componentId, filter);
            var request = new GetLogsRequestDto()
            {
                Data = dataDto,
                Token = GetAccessTokenDto()
            };
            var responseDto = DtoService.GetLogs(request);
            var response = CreateResponse<GetLogsResponse>(responseDto);
            response.InternalData = DataConverter.GetLogInfoList(responseDto.Data);
            return response;
        }

        public GetLogConfigResponse GetLogConfig(Guid componentId)
        {
            var dataDto = new GetLogConfigRequestDtoData()
            {
                ComponentId = componentId
            };
            var request = new GetLogConfigRequestDto()
            {
                Data = dataDto,
                Token = GetAccessTokenDto()
            };
            var responseDto = DtoService.GetLogConfig(request);
            var response = CreateResponse<GetLogConfigResponse>(responseDto);
            response.InternalData = DataConverter.GetWebLogConfig(responseDto.Data);
            return response;
        }

        public GetChangedWebLogConfigsResponse GetChangedWebLogConfigs(DateTime lastUpdateDate, List<Guid> componentIds)
        {
            var dataDto = new GetChangedWebLogConfigsRequestDtoData()
            {
                LastUpdateDate = lastUpdateDate,
                ComponentIds = componentIds
            };
            var request = new GetChangedWebLogConfigsRequestDto()
            {
                Data = dataDto,
                Token = GetAccessTokenDto()
            };
            var responseDto = DtoService.GetChangedWebLogConfigs(request);
            var response = CreateResponse<GetChangedWebLogConfigsResponse>(responseDto);
            response.InternalData = DataConverter.GetWebLogConfigList(responseDto.Data);
            return response;
        }
    }
}
