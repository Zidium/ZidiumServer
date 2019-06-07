using System;
using System.Collections.Generic;
using System.Linq;
using Zidium.Api;
using Zidium.Api.Dto;

namespace ApiAdapter
{
    public class ApiToDispatcherAdapter : IDtoService
    {
        public string Ip { get; protected set; }

        public string AccountName { get; set; }
        
        #region Служебные

        protected Zidium.Core.Api.IDispatcherService Dispatcher;

        public ApiToDispatcherAdapter(
            Zidium.Core.Api.IDispatcherService dispatcher,
            string ip,
            string accountName)
        {
            if (dispatcher == null)
            {
                throw new ArgumentNullException("dispatcher");
            }
            Dispatcher = dispatcher;
            Ip = ip;
            AccountName = accountName;
        }

        protected void SetRequestContextData(Zidium.Core.Api.Request request)
        {
            request.Ip = Ip;
            if (request.Token != null)
            {
                request.ProgramName = request.Token.ProgramName;
                request.Token.AccountName = AccountName;
            }
            request.RequestId = Guid.NewGuid();
        }

        protected TRequest GetCoreRequest<TRequest>(Request request)
            where TRequest : Zidium.Core.Api.Request, new()
        {
            var coreRequest = new TRequest();
            if (request != null)
            {
                coreRequest.Token = AdapterDataConverter.ConvertToCore(request.Token);
            }
            SetRequestContextData(coreRequest);
            return coreRequest;
        }

        protected TResponse GetApiResponse<TResponse>(Response response)
            where TResponse : Response, new()
        {
            var coreResponse = new TResponse();
            coreResponse.ErrorMessage = response.ErrorMessage;
            coreResponse.Code = response.Code;
            return coreResponse;
        }

        #endregion

        #region Разное

        public GetEchoResponseDto GetEcho(GetEchoRequestDto request)
        {
            var coreRequest = GetCoreRequest<Zidium.Core.Api.GetEchoRequest>(request);
            coreRequest.Data = AdapterDataConverter.ConvertToCore(request.Data);

            var coreResponse = Dispatcher.GetEcho(coreRequest);

            var response = GetApiResponse<GetEchoResponseDto>(coreResponse);
            response.Data = coreResponse.InternalData;
            return response;
        }

        public GetServerTimeResponseDto GetServerTime()
        {
            var coreResponse = Dispatcher.GetServerTime();

            var response = GetApiResponse<GetServerTimeResponseDto>(coreResponse);
            response.Data = AdapterDataConverter.ConvertToApi(coreResponse.InternalData);
            return response;
        }

        #endregion

        #region Components

        public GetRootControlDataResponseDto GetRootControlData(GetRootControlDataRequestDto request)
        {
            var coreRequest = GetCoreRequest<Zidium.Core.Api.GetRootControlDataRequest>(request);
            coreRequest.Data = request.Data;

            var coreResponse = Dispatcher.GetRootControlData(coreRequest);

            var response = GetApiResponse<GetRootControlDataResponseDto>(coreResponse);
            response.Data = AdapterDataConverter.ConvertToApi(coreResponse.InternalData);
            return response;
        }

        public GetComponentControlByIdResponseDto GetComponentControlById(GetComponentControlByIdRequestDto request)
        {
            var coreRequest = GetCoreRequest<Zidium.Core.Api.GetComponentControlByIdRequest>(request);
            coreRequest.Data = AdapterDataConverter.ConvertToCore(request.Data);

            var coreResponse = Dispatcher.GetComponentControlById(coreRequest);

            var response = GetApiResponse<GetComponentControlByIdResponseDto>(coreResponse);
            response.Data = AdapterDataConverter.ConvertToApi(coreResponse.InternalData);
            return response;
        }

        public GetRootComponentResponseDto GetRootComponent(GetRootComponentRequestDto request)
        {
            var coreRequest = GetCoreRequest<Zidium.Core.Api.GetRootComponentRequest>(request);
            coreRequest.Data = request.Data;

            var coreResponse = Dispatcher.GetRootComponent(coreRequest);

            var response = GetApiResponse<GetRootComponentResponseDto>(coreResponse);
            response.Data = AdapterDataConverter.ConvertToApi(coreResponse.InternalData);
            return response;
            
        }

        public GetOrAddComponentResponseDto GetOrAddComponent(GetOrAddComponentRequestDto request)
        {
            var coreRequest = GetCoreRequest<Zidium.Core.Api.GetOrAddComponentRequest>(request);
            coreRequest.Data = AdapterDataConverter.ConvertToCore(request.Data);

            var coreResponse = Dispatcher.GetOrAddComponent(coreRequest);

            var response = GetApiResponse<GetOrAddComponentResponseDto>(coreResponse);
            response.Data = AdapterDataConverter.ConvertToApi(coreResponse.InternalData);
            return response;
        }

        public GetComponentByIdResponseDto GetComponentById(GetComponentByIdRequestDto request)
        {
            var coreRequest = GetCoreRequest<Zidium.Core.Api.GetComponentByIdRequest>(request);
            coreRequest.Data = AdapterDataConverter.ConvertToCore(request.Data);

            var coreResponse = Dispatcher.GetComponentById(coreRequest);

            var response = GetApiResponse<GetComponentByIdResponseDto>(coreResponse);
            response.Data = AdapterDataConverter.ConvertToApi(coreResponse.InternalData);
            return response;
        }

        public GetComponentBySystemNameResponseDto GetComponentBySystemName(GetComponentBySystemNameRequestDto request)
        {
            var coreRequest = GetCoreRequest<Zidium.Core.Api.GetComponentBySystemNameRequest>(request);
            coreRequest.Data = AdapterDataConverter.ConvertToCore(request.Data);

            var coreResponse = Dispatcher.GetComponentBySystemName(coreRequest);

            var response = GetApiResponse<GetComponentBySystemNameResponseDto>(coreResponse);
            response.Data = AdapterDataConverter.ConvertToApi(coreResponse.InternalData);
            return response;
        }

        public GetChildComponentsResponseDto GetChildComponents(GetChildComponentsRequestDto request)
        {
            var coreRequest = GetCoreRequest<Zidium.Core.Api.GetChildComponentsRequest>(request);
            coreRequest.Data = AdapterDataConverter.ConvertToCore(request.Data);

            var coreResponse = Dispatcher.GetChildComponents(coreRequest);

            var response = GetApiResponse<GetChildComponentsResponseDto>(coreResponse);
            response.Data = AdapterDataConverter.ConvertToApi(coreResponse.InternalData);
            return response;
        }

        public GetOrCreateComponentResponseDto GetOrCreateComponent(GetOrCreateComponentRequestDto request)
        {
            var coreRequest = GetCoreRequest<Zidium.Core.Api.GetOrCreateComponentRequest>(request);
            coreRequest.Data = AdapterDataConverter.ConvertToCore(request.Data);

            var coreResponse = Dispatcher.GetOrCreateComponent(coreRequest);

            var response = GetApiResponse<GetOrCreateComponentResponseDto>(coreResponse);
            response.Data = AdapterDataConverter.ConvertToApi(coreResponse.InternalData);
            return response;
        }

        public UpdateComponentResponseDto UpdateComponent(UpdateComponentRequestDto request)
        {
            var coreRequest = GetCoreRequest<Zidium.Core.Api.UpdateComponentRequest>(request);
            coreRequest.Data = AdapterDataConverter.ConvertToCore(request.Data);

            var coreResponse = Dispatcher.UpdateComponent(coreRequest);

            var response = GetApiResponse<UpdateComponentResponseDto>(coreResponse);
            response.Data = AdapterDataConverter.ConvertToApi(coreResponse.InternalData);
            return response;
        }

        public GetComponentTotalStateResponseDto GetComponentTotalState(GetComponentTotalStateRequestDto request)
        {
            var coreRequest = GetCoreRequest<Zidium.Core.Api.GetComponentTotalStateRequest>(request);
            coreRequest.Data = AdapterDataConverter.ConvertToCore(request.Data);

            var coreResponse = Dispatcher.GetComponentTotalState(coreRequest);

            var response = GetApiResponse<GetComponentTotalStateResponseDto>(coreResponse);
            response.Data = AdapterDataConverter.ConvertToApi(coreResponse.InternalData);
            return response;
        }

        public GetComponentInternalStateResponseDto GetComponentInternalState(GetComponentInternalStateRequestDto request)
        {
            var coreRequest = GetCoreRequest<Zidium.Core.Api.GetComponentInternalStateRequest>(request);
            coreRequest.Data = AdapterDataConverter.ConvertToCore(request.Data);

            var coreResponse = Dispatcher.GetComponentInternalState(coreRequest);

            var response = GetApiResponse<GetComponentInternalStateResponseDto>(coreResponse);
            response.Data = AdapterDataConverter.ConvertToApi(coreResponse.InternalData);
            return response;
        }

        public SetComponentEnableResponseDto SetComponentEnable(SetComponentEnableRequestDto request)
        {
            var coreRequest = GetCoreRequest<Zidium.Core.Api.SetComponentEnableRequest>(request);
            coreRequest.Data = AdapterDataConverter.ConvertToCore(request.Data);

            var coreResponse = Dispatcher.SetComponentEnable(coreRequest);

            var response = GetApiResponse<SetComponentEnableResponseDto>(coreResponse);
            response.Data = coreResponse.InternalData;
            return response;
        }

        public SetComponentDisableResponseDto SetComponentDisable(SetComponentDisableRequestDto request)
        {
            var coreRequest = GetCoreRequest<Zidium.Core.Api.SetComponentDisableRequest>(request);
            coreRequest.Data = AdapterDataConverter.ConvertToCore(request.Data);

            var coreResponse = Dispatcher.SetComponentDisable(coreRequest);

            var response = GetApiResponse<SetComponentDisableResponseDto>(coreResponse);
            response.Data = coreResponse.InternalData;
            return response;
        }

        public DeleteComponentResponseDto DeleteComponent(DeleteComponentRequestDto request)
        {
            var coreRequest = GetCoreRequest<Zidium.Core.Api.DeleteComponentRequest>(request);
            coreRequest.Data = AdapterDataConverter.ConvertToCore(request.Data);

            var coreResponse = Dispatcher.DeleteComponent(coreRequest);

            var response = GetApiResponse<DeleteComponentResponseDto>(coreResponse);
            response.Data = coreResponse.InternalData;
            return response;
        }

        public GetComponentTypeResponseDto GetComponentType(GetComponentTypeRequestDto request)
        {
            var coreRequest = GetCoreRequest<Zidium.Core.Api.GetComponentTypeRequest>(request);
            coreRequest.Data = AdapterDataConverter.ConvertToCore(request.Data);

            var coreResponse = Dispatcher.GetComponentType(coreRequest);

            var response = GetApiResponse<GetComponentTypeResponseDto>(coreResponse);
            response.Data = AdapterDataConverter.ConvertToApi(coreResponse.InternalData);
            return response;
        }

        public GetOrCreateComponentTypeResponseDto GetOrCreateComponentType(GetOrCreateComponentTypeRequestDto request)
        {
            var coreRequest = GetCoreRequest<Zidium.Core.Api.GetOrCreateComponentTypeRequest>(request);
            coreRequest.Data = AdapterDataConverter.ConvertToCore(request.Data);

            var coreResponse = Dispatcher.GetOrCreateComponentType(coreRequest);

            var response = GetApiResponse<GetOrCreateComponentTypeResponseDto>(coreResponse);
            response.Data = AdapterDataConverter.ConvertToApi(coreResponse.InternalData);
            return response;
        }

        public UpdateComponentTypeResponseDto UpdateComponentType(UpdateComponentTypeRequestDto request)
        {
            var coreRequest = GetCoreRequest<Zidium.Core.Api.UpdateComponentTypeRequest>(request);
            coreRequest.Data = AdapterDataConverter.ConvertToCore(request.Data);

            var coreResponse = Dispatcher.UpdateComponentType(coreRequest);

            var response = GetApiResponse<UpdateComponentTypeResponseDto>(coreResponse);
            response.Data = AdapterDataConverter.ConvertToApi(coreResponse.InternalData);
            return response;
        }

        #endregion

        #region events

        public SendEventResponseDto SendEvent(SendEventRequestDto request)
        {
            var coreRequest = GetCoreRequest<Zidium.Core.Api.SendEventRequest>(request);
            coreRequest.Data = AdapterDataConverter.ConvertToCore(request.Data);

            var coreResponse = Dispatcher.SendEvent(coreRequest);

            var response = GetApiResponse<SendEventResponseDto>(coreResponse);
            response.Data = AdapterDataConverter.ConvertToApi(coreResponse.InternalData);
            return response;
        }

        public JoinEventsResponseDto JoinEvents(JoinEventsRequestDto request)
        {
            var coreRequest = GetCoreRequest<Zidium.Core.Api.JoinEventsRequest>(request);
            coreRequest.Data = AdapterDataConverter.ConvertToCore(request.Data);

            var coreResponse = Dispatcher.JoinEvents(coreRequest);

            var response = GetApiResponse<JoinEventsResponseDto>(coreResponse);
            response.Data = coreResponse.InternalData;
            return response;
        }

        public GetEventByIdResponseDto GetEventById(GetEventByIdRequestDto request)
        {
            var coreRequest = GetCoreRequest<Zidium.Core.Api.GetEventByIdRequest>(request);
            coreRequest.Data = AdapterDataConverter.ConvertToCore(request.Data);

            var coreResponse = Dispatcher.GetEventById(coreRequest);

            var response = GetApiResponse<GetEventByIdResponseDto>(coreResponse);
            response.Data = AdapterDataConverter.ConvertToApi(coreResponse.InternalData);
            return response;
        }

        public GetEventsResponseDto GetEvents(GetEventsRequestDto request)
        {
            var coreRequest = GetCoreRequest<Zidium.Core.Api.GetEventsRequest>(request);
            coreRequest.Data = AdapterDataConverter.ConvertToCore(request.Data);

            var coreResponse = Dispatcher.GetEvents(coreRequest);

            var response = GetApiResponse<GetEventsResponseDto>(coreResponse);
            response.Data = AdapterDataConverter.ConvertToApi(coreResponse.InternalData);
            return response;
        }

        #endregion

        #region unit tests

        public GetOrCreateUnitTestTypeResponseDto GetOrCreateUnitTestType(GetOrCreateUnitTestTypeRequestDto request)
        {
            var coreRequest = GetCoreRequest<Zidium.Core.Api.GetOrCreateUnitTestTypeRequest>(request);
            coreRequest.Data = AdapterDataConverter.ConvertToCore(request.Data);

            var coreResponse = Dispatcher.GetOrCreateUnitTestType(coreRequest);

            var response = GetApiResponse<GetOrCreateUnitTestTypeResponseDto>(coreResponse);
            response.Data = AdapterDataConverter.ConvertToApi(coreResponse.InternalData);
            return response;
        }

        public SendUnitTestResultResponseDto SendUnitTestResult(SendUnitTestResultRequestDto request)
        {
            var coreRequest = GetCoreRequest<Zidium.Core.Api.SendUnitTestResultRequest>(request);
            coreRequest.Data = AdapterDataConverter.ConvertToCore(request.Data);

            var coreResponse = Dispatcher.SendUnitTestResult(coreRequest);

            var response = GetApiResponse<SendUnitTestResultResponseDto>(coreResponse);
            response.Data = coreResponse.InternalData;
            return response;
        }

        public SendUnitTestResultsResponseDto SendUnitTestResults(SendUnitTestResultsRequestDto request)
        {
            var coreRequest = GetCoreRequest<Zidium.Core.Api.SendUnitTestResultsRequest>(request);
            coreRequest.Data = AdapterDataConverter.ConvertToCore(request.Data);

            var coreResponse = Dispatcher.SendUnitTestResults(coreRequest);

            var response = GetApiResponse<SendUnitTestResultsResponseDto>(coreResponse);
            response.Data = coreResponse.InternalData;
            return response;
        }

        public GetUnitTestStateResponseDto GetUnitTestState(GetUnitTestStateRequestDto request)
        {
            var coreRequest = GetCoreRequest<Zidium.Core.Api.GetUnitTestStateRequest>(request);
            coreRequest.Data = AdapterDataConverter.ConvertToCore(request.Data);

            var coreResponse = Dispatcher.GetUnitTestState(coreRequest);

            var response = GetApiResponse<GetUnitTestStateResponseDto>(coreResponse);
            response.Data = AdapterDataConverter.ConvertToApi(coreResponse.InternalData);
            return response;
        }

        public SetUnitTestEnableResponseDto SetUnitTestEnable(SetUnitTestEnableRequestDto request)
        {
            var coreRequest = GetCoreRequest<Zidium.Core.Api.SetUnitTestEnableRequest>(request);
            coreRequest.Data = AdapterDataConverter.ConvertToCore(request.Data);

            var coreResponse = Dispatcher.SetUnitTestEnable(coreRequest);

            var response = GetApiResponse<SetUnitTestEnableResponseDto>(coreResponse);
            response.Data = coreResponse.InternalData;
            return response;
        }

        public SetUnitTestDisableResponseDto SetUnitTestDisable(SetUnitTestDisableRequestDto request)
        {
            var coreRequest = GetCoreRequest<Zidium.Core.Api.SetUnitTestDisableRequest>(request);
            coreRequest.Data = AdapterDataConverter.ConvertToCore(request.Data);

            var coreResponse = Dispatcher.SetUnitTestDisable(coreRequest);

            var response = GetApiResponse<SetUnitTestDisableResponseDto>(coreResponse);
            response.Data = coreResponse.InternalData;
            return response;
        }

        public GetOrCreateUnitTestResponseDto GetOrCreateUnitTest(GetOrCreateUnitTestRequestDto request)
        {
            var coreRequest = GetCoreRequest<Zidium.Core.Api.GetOrCreateUnitTestRequest>(request);
            coreRequest.Data = AdapterDataConverter.ConvertToCore(request.Data);

            var coreResponse = Dispatcher.GetOrCreateUnitTest(coreRequest);

            var response = GetApiResponse<GetOrCreateUnitTestResponseDto>(coreResponse);
            response.Data = AdapterDataConverter.ConvertToApi(coreResponse.InternalData);
            return response;
        }

        #endregion

        #region Counters

        protected List<TOut> ConvertList<TIn, TOut>(List<TIn> list, Func<TIn, TOut> converter)
        {
            if (list == null)
            {
                return new List<TOut>();
            }
            return list.Select(converter).ToList();
        } 

        public SendMetricsResponseDto SendMetrics(SendMetricsRequestDto request)
        {
            var coreRequest = GetCoreRequest<Zidium.Core.Api.SendMetricsRequest>(request);
            coreRequest.Data = ConvertList(request.Data, AdapterDataConverter.ConvertToCore);

            var coreResponse = Dispatcher.SendMetrics(coreRequest);

            var response = GetApiResponse<SendMetricsResponseDto>(coreResponse);
            response.Data = coreResponse.InternalData;
            return response;
        }

        public GetMetricsHistoryResponseDto GetMetricsHistory(GetMetricsHistoryRequestDto request)
        {
            var coreRequest = GetCoreRequest<Zidium.Core.Api.GetMetricsHistoryRequest>(request);
            coreRequest.Data = AdapterDataConverter.ConvertToCore(request.Data);

            var coreResponse = Dispatcher.GetMetricsHistory(coreRequest);

            var response = GetApiResponse<GetMetricsHistoryResponseDto>(coreResponse);
            response.Data = AdapterDataConverter.ConvertToApi(coreResponse.InternalData);
            return response;
        }

        public GetMetricsResponseDto GetMetrics(GetMetricsRequestDto request)
        {
            var coreRequest = GetCoreRequest<Zidium.Core.Api.GetMetricsRequest>(request);
            coreRequest.Data = AdapterDataConverter.ConvertToCore(request.Data);

            var coreResponse = Dispatcher.GetMetrics(coreRequest);

            var response = GetApiResponse<GetMetricsResponseDto>(coreResponse);
            response.Data = AdapterDataConverter.ConvertToApi(coreResponse.InternalData);
            return response;
        }

        public SendMetricResponseDto SendMetric(SendMetricRequestDto request)
        {
            var coreRequest = GetCoreRequest<Zidium.Core.Api.SendMetricRequest>(request);
            coreRequest.Data = AdapterDataConverter.ConvertToCore(request.Data);

            var coreResponse = Dispatcher.SendMetric(coreRequest);

            var response = GetApiResponse<SendMetricResponseDto>(coreResponse);
            response.Data = AdapterDataConverter.ConvertToApi(coreResponse.InternalData);
            return response;
        }

        public GetMetricResponseDto GetMetric(GetMetricRequestDto request)
        {
            var coreRequest = GetCoreRequest<Zidium.Core.Api.GetMetricRequest>(request);
            coreRequest.Data = AdapterDataConverter.ConvertToCore(request.Data);

            var coreResponse = Dispatcher.GetMetric(coreRequest);

            var response = GetApiResponse<GetMetricResponseDto>(coreResponse);
            response.Data = AdapterDataConverter.ConvertToApi(coreResponse.InternalData);
            return response;
        }

        #endregion

        #region logs

        public SendLogResponseDto SendLog(SendLogRequestDto request)
        {
            var coreRequest = GetCoreRequest<Zidium.Core.Api.SendLogRequest>(request);
            coreRequest.Data = AdapterDataConverter.ConvertToCore(request.Data);

            var coreResponse = Dispatcher.SendLog(coreRequest);

            var response = GetApiResponse<SendLogResponseDto>(coreResponse);
            response.Data = coreResponse.InternalData;
            return response;
        }

        public GetLogsResponseDto GetLogs(GetLogsRequestDto request)
        {
            var coreRequest = GetCoreRequest<Zidium.Core.Api.GetLogsRequest>(request);
            coreRequest.Data = AdapterDataConverter.ConvertToCore(request.Data);

            var coreResponse = Dispatcher.GetLogs(coreRequest);

            var response = GetApiResponse<GetLogsResponseDto>(coreResponse);
            response.Data = AdapterDataConverter.ConvertToApi(coreResponse.InternalData);
            return response;
        }

        public GetLogConfigResponseDto GetLogConfig(GetLogConfigRequestDto request)
        {
            var coreRequest = GetCoreRequest<Zidium.Core.Api.GetLogConfigRequest>(request);
            coreRequest.Data = AdapterDataConverter.ConvertToCore(request.Data);

            var coreResponse = Dispatcher.GetLogConfig(coreRequest);

            var response = GetApiResponse<GetLogConfigResponseDto>(coreResponse);
            response.Data = AdapterDataConverter.ConvertToApi(coreResponse.InternalData);
            return response;
        }

        public GetChangedWebLogConfigsResponseDto GetChangedWebLogConfigs(GetChangedWebLogConfigsRequestDto request)
        {
            var coreRequest = GetCoreRequest<Zidium.Core.Api.GetChangedWebLogConfigsRequest>(request);
            coreRequest.Data = AdapterDataConverter.ConvertToCore(request.Data);

            var coreResponse = Dispatcher.GetChangedWebLogConfigs(coreRequest);

            var response = GetApiResponse<GetChangedWebLogConfigsResponseDto>(coreResponse);
            var coreResponseData = coreResponse.InternalData;
            response.Data = ConvertList(coreResponseData, AdapterDataConverter.ConvertToApi);
            return response;
        }

        public SendLogsResponseDto SendLogs(SendLogsRequestDto request)
        {
            var coreRequest = GetCoreRequest<Zidium.Core.Api.SendLogsRequest>(request);
            coreRequest.Data = AdapterDataConverter.ConvertToCore(request.Data);

            var coreResponse = Dispatcher.SendLogs(coreRequest);

            var response = GetApiResponse<SendLogsResponseDto>(coreResponse);
            response.Data = coreResponse.InternalData;
            return response;
        }

        #endregion
    }
}
