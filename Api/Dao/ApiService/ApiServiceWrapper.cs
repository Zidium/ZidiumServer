using System;
using System.Collections.Generic;
using Zidium.Api.Dto;
using Zidium.Api.Logs;

namespace Zidium.Api
{
    public class ApiServiceWrapper : IApiService
    {
        internal Client Client { get; set; }

        public IApiService ApiServiceInternal { get; protected set; }

        public Action<ResponseDto> ProcessResponseAction { get; set; }

        public ApiServiceWrapper(Client client, IApiService service)
        {
            if (client == null)
            {
                throw new ArgumentNullException("client");
            }
            if (service == null)
            {
                throw new ArgumentNullException("service");
            }
            Client = client;
            ApiServiceInternal = service;
        }

        protected IInternalLog Log
        {
            get { return Client.InternalLog; }
        }

        protected void ProcessResponse(string action, ResponseDto response)
        {
            var lastResponse = new ResponseInfo(action, response);
            Client.SetLastResponse(lastResponse);
            if (response.Success)
            {
                if (Log.IsTraceEnabled) Log.Trace("Success end " + action);
            }
            else // todo нужно проверять код ошибки
            {
                if (Log.IsTraceEnabled) Log.Trace("Failed end " + action + "; errorCode " + response.Code + "; " + response.ErrorMessage);

                // ошибка "событие из будущего"
                if (response.Code == ResponseCode.FutureEvent)
                {
                    Client.CalculateServerTimeDifference();
                }
            }

            // используется в юнит-тестах
            ProcessResponseAction?.Invoke(response);
        }

        protected TResponse Execute<TResponse>(string actionName, Func<TResponse> action)
            where TResponse : ResponseDto, new()
        {
            TResponse response = null;
            try
            {
                if (Log.IsTraceEnabled) Log.Trace("Begin " + actionName);
                response = action();
            }
            catch (ResponseException exception)
            {
                response = new TResponse()
                {
                    ErrorMessage = exception.Response.ErrorMessage,
                    Code = exception.Response.Code
                };
            }
            catch (Exception exception)
            {
                Log.Error("Exception: " + exception.Message, exception);
                response = new TResponse()
                {
                    ErrorMessage = exception.Message,
                    Code = ResponseCode.ClientError
                };
            }
            finally
            {
                if (response != null)
                {
                    ProcessResponse(actionName, response);
                }
            }
            return response;
        }

        public AccessTokenDto AccessToken
        {
            get { return ApiServiceInternal.AccessToken; }
        }

        public void SetAccessToken(AccessTokenDto accessToken)
        {
            ApiServiceInternal.SetAccessToken(accessToken);
        }

        public GetEchoResponseDto GetEcho(string message)
        {
            return Execute("GetEcho", () => ApiServiceInternal.GetEcho(message));
        }

        public GetServerTimeResponseDto GetServerTime()
        {
            return Execute("GetServerTime", () => ApiServiceInternal.GetServerTime());
        }

        public GetRootControlDataResponseDto GetRootControlData()
        {
            return Execute("GetRootControlData", () => ApiServiceInternal.GetRootControlData());
        }

        public GetComponentControlByIdResponseDto GetComponentControlById(Guid componentId)
        {
            return Execute("GetComponentControlById", () => ApiServiceInternal.GetComponentControlById(componentId));
        }

        public GetOrCreateComponentResponseDto GetOrCreateComponent(GetOrCreateComponentRequestDataDto data)
        {
            return Execute("GetOrCreateComponent", () => ApiServiceInternal.GetOrCreateComponent(data));
        }

        public GetRootComponentResponseDto GetRootComponent()
        {
            return Execute("GetRootComponent", () => ApiServiceInternal.GetRootComponent());
        }

        public GetComponentByIdResponseDto GetComponentById(Guid componentId)
        {
            return Execute("GetComponentById", () => ApiServiceInternal.GetComponentById(componentId));
        }

        public GetComponentBySystemNameResponseDto GetComponentBySystemName(Guid? parentId, string systemName)
        {
            return Execute("GetComponentBySystemName", () => ApiServiceInternal.GetComponentBySystemName(parentId, systemName));
        }

        public GetComponentBySystemNameResponseDto GetComponentBySystemName(GetComponentBySystemNameRequestDataDto data)
        {
            return Execute("GetComponentBySystemName", () => ApiServiceInternal.GetComponentBySystemName(data));
        }

        public GetChildComponentsResponseDto GetChildComponents(Guid componentId)
        {
            return Execute("GetChildComponents", () => ApiServiceInternal.GetChildComponents(componentId));
        }

        public UpdateComponentResponseDto UpdateComponent(UpdateComponentRequestDataDto data)
        {
            return Execute("UpdateComponent", () => ApiServiceInternal.UpdateComponent(data));
        }

        public GetComponentTotalStateResponseDto GetComponentTotalState(Guid componentId, bool recalc)
        {
            return Execute("GetComponentTotalState", () => ApiServiceInternal.GetComponentTotalState(componentId, recalc));
        }

        public GetComponentInternalStateResponseDto GetComponentInternalState(Guid componentId, bool recalc)
        {
            return Execute("GetComponentInternalState", () => ApiServiceInternal.GetComponentInternalState(componentId, recalc));
        }

        public SetComponentEnableResponseDto SetComponentEnable(Guid componentId)
        {
            return Execute("SetComponentEnable", () => ApiServiceInternal.SetComponentEnable(componentId));
        }

        public SetComponentDisableResponseDto SetComponentDisable(Guid componentId, DateTime? toDate, string comment)
        {
            return Execute("SetComponentDisable", () => ApiServiceInternal.SetComponentDisable(componentId, toDate, comment));
        }

        public DeleteComponentResponseDto DeleteComponent(Guid componentId)
        {
            return Execute("DeleteComponent", () => ApiServiceInternal.DeleteComponent(componentId));
        }

        public GetComponentTypeResponseDto GetComponentType(GetComponentTypeRequestDataDto data)
        {
            return Execute("GetComponentType", () => ApiServiceInternal.GetComponentType(data));
        }

        public GetOrCreateComponentTypeResponseDto GetOrCreateComponentType(GetOrCreateComponentTypeRequestDataDto data)
        {
            return Execute("GetOrCreateComponentType", () => ApiServiceInternal.GetOrCreateComponentType(data));
        }

        public UpdateComponentTypeResponseDto UpdateComponentType(UpdateComponentTypeRequestDataDto data)
        {
            return Execute("UpdateComponentType", () => ApiServiceInternal.UpdateComponentType(data));
        }

        public SendEventResponseDto SendEvent(SendEventRequestDataDto data)
        {
            return Execute("SendEvent", () => ApiServiceInternal.SendEvent(data));
        }

        public GetEventByIdResponseDto GetEventById(Guid eventId)
        {
            return Execute("GetEventById", () => ApiServiceInternal.GetEventById(eventId));
        }

        public GetEventsResponseDto GetEvents(GetEventsRequestDataDto data)
        {
            return Execute("GetEvents", () => ApiServiceInternal.GetEvents(data));
        }

        public JoinEventsResponseDto JoinEvents(List<JoinEventRequestDataDto> data)
        {
            return Execute("JoinEvents", () => ApiServiceInternal.JoinEvents(data));
        }

        public GetOrCreateUnitTestResponseDto GetOrCreateUnitTest(GetOrCreateUnitTestRequestDataDto data)
        {
            return Execute("GetOrCreateUnitTest", () => ApiServiceInternal.GetOrCreateUnitTest(data));
        }

        public GetOrCreateUnitTestTypeResponseDto GetOrCreateUnitTestType(GetOrCreateUnitTestTypeRequestDataDto data)
        {
            return Execute("GetOrCreateUnitTestType", () => ApiServiceInternal.GetOrCreateUnitTestType(data));
        }

        public SendUnitTestResultResponseDto SendUnitTestResult(SendUnitTestResultRequestDataDto data)
        {
            return Execute("SendUnitTestResult", () => ApiServiceInternal.SendUnitTestResult(data));
        }

        public SendUnitTestResultsResponseDto SendUnitTestResults(List<SendUnitTestResultRequestDataDto> data)
        {
            return Execute("SendUnitTestResults", () => ApiServiceInternal.SendUnitTestResults(data));
        }

        public GetUnitTestStateResponseDto GetUnitTestState(Guid unitTestId)
        {
            return Execute("GetUnitTestState", () => ApiServiceInternal.GetUnitTestState(unitTestId));
        }

        public SetUnitTestEnableResponseDto SetUnitTestEnable(Guid unitTestId)
        {
            return Execute("SetUnitTestEnable", () => ApiServiceInternal.SetUnitTestEnable(unitTestId));
        }

        public SetUnitTestDisableResponseDto SetUnitTestDisable(SetUnitTestDisableRequestDataDto data)
        {
            return Execute("SetUnitTestDisable", () => ApiServiceInternal.SetUnitTestDisable(data));
        }

        public SendMetricResponseDto SendMetric(SendMetricRequestDataDto data)
        {
            return Execute("SendMetric", () => ApiServiceInternal.SendMetric(data));
        }

        public SendMetricsResponseDto SendMetrics(List<SendMetricRequestDataDto> data)
        {
            return Execute("SendMetrics", () => ApiServiceInternal.SendMetrics(data));
        }

        public GetMetricsHistoryResponseDto GetMetricsHistory(GetMetricsHistoryRequestDataDto filter)
        {
            return Execute("GetMetricsHistory", () => ApiServiceInternal.GetMetricsHistory(filter));
        }

        public GetMetricsResponseDto GetMetrics(Guid componentId)
        {
            return Execute("GetMetrics", () => ApiServiceInternal.GetMetrics(componentId));
        }

        public GetMetricResponseDto GetMetric(Guid componentId, string metricName)
        {
            return Execute("GetMetric", () => ApiServiceInternal.GetMetric(componentId, metricName));
        }

        public SendLogResponseDto SendLog(SendLogRequestDataDto data)
        {
            return Execute("SendLog", () => ApiServiceInternal.SendLog(data));
        }

        public SendLogsResponseDto SendLogs(SendLogRequestDataDto[] data)
        {
            return Execute("SendLogs", () => ApiServiceInternal.SendLogs(data));
        }

        public GetLogsResponseDto GetLogs(GetLogsRequestDataDto filter)
        {
            return Execute("GetLogs", () => ApiServiceInternal.GetLogs(filter));
        }

        public GetLogConfigResponseDto GetLogConfig(Guid componentId)
        {
            return Execute("GetLogConfig", () => ApiServiceInternal.GetLogConfig(componentId));
        }

        public GetChangedWebLogConfigsResponseDto GetChangedWebLogConfigs(DateTime lastUpdateDate, Guid[] componentIds)
        {
            return Execute("GetChangedWebLogConfigs", () => ApiServiceInternal.GetChangedWebLogConfigs(lastUpdateDate, componentIds));
        }

        public bool IsFake
        {
            get { return ApiServiceInternal.IsFake; }
        }

    }
}
