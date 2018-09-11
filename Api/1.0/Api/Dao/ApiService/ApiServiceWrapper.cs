using System;
using System.Collections.Generic;
using Zidium.Api.Logs;

namespace Zidium.Api
{
    public class ApiServiceWrapper : IApiService
    {
        internal Client Client { get; set; }

        public IApiService ApiServiceInternal { get; protected set; }

        public Action<Response> ProcessResponseAction { get; set; }

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
        
        protected void ProcessResponse(string action, Response response)
        {
            var lastResponse = new ResponseInfo(action, response);
            Client.SetLastResponse(lastResponse);
            if (response.Success)
            {
                Log.Trace("Success end " + action);
            }
            else // todo нужно проверять код ошибки
            {
                Log.Trace("Failed end " + action + "; errorCode " + response.Code + "; " + response.ErrorMessage);

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
            where TResponse : Response, new()
        {
            TResponse response = null;
            try
            {
                Log.Trace("Begin " + actionName);
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

        public AccessToken AccessToken
        {
            get { return ApiServiceInternal.AccessToken; }
        }

        public void SetAccessToken(AccessToken accessToken)
        {
            ApiServiceInternal.SetAccessToken(accessToken);
        }

        public bool IsFake
        {
            get { return ApiServiceInternal.IsFake; }
        }

        public GetEchoResponse GetEcho(string message)
        {
            return Execute("GetEcho", () => ApiServiceInternal.GetEcho(message)); 
        }

        public GetServerTimeResponse GetServerTime()
        {
            return Execute("GetServerTime", () => ApiServiceInternal.GetServerTime()); 
        }

        public GetRootControlDataResponse GetRootControlData()
        {
            return Execute("GetRootControlData", () => ApiServiceInternal.GetRootControlData());
        }

        public GetOrCreateComponentResponse GetOrCreateComponent(Guid parentId, GetOrCreateComponentData data)
        {
            return Execute("GetOrCreateComponent", () => ApiServiceInternal.GetOrCreateComponent(parentId, data));
        }

        public GetComponentControlByIdResponse GetComponentControlById(Guid id)
        {
            return Execute("GetComponentControlById", () => ApiServiceInternal.GetComponentControlById(id));
        }

        public GetRootComponentResponse GetRootComponent()
        {
            return Execute("GetRootComponent", () => ApiServiceInternal.GetRootComponent());
        }

        public GetComponentByIdResponse GetComponentById(Guid componentId)
        {
            return Execute("GetComponentById", () => ApiServiceInternal.GetComponentById(componentId));
        }

        public GetComponentBySystemNameResponse GetComponentBySystemName(string systemName)
        {
            return Execute("GetComponentBySystemName", () => ApiServiceInternal.GetComponentBySystemName(systemName));
        }

        public GetComponentBySystemNameResponse GetComponentBySystemName(GetComponentBySystemNameData data)
        {
            return Execute("GetComponentBySystemName", () => ApiServiceInternal.GetComponentBySystemName(data));
        }

        public GetChildComponentsResponse GetChildComponents(Guid componentId)
        {
            return Execute("GetChildComponents", () => ApiServiceInternal.GetChildComponents(componentId));
        }

        public UpdateComponentResponse UpdateComponent(Guid componentId, UpdateComponentData data)
        {
            return Execute("UpdateComponent", () => ApiServiceInternal.UpdateComponent(componentId, data));
        }

        public GetComponentTotalStateResponse GetComponentTotalState(Guid componentId, bool recalc)
        {
            return Execute("GetComponentTotalState", () => ApiServiceInternal.GetComponentTotalState(componentId, recalc));
        }

        public GetComponentInternalStateResponse GetComponentInternalState(Guid componentId, bool recalc)
        {
            return Execute("GetComponentInternalState", () => ApiServiceInternal.GetComponentInternalState(componentId, recalc));
        }

        public SetComponentEnableResponse SetComponentEnable(Guid componentId)
        {
            return Execute("SetComponentEnable", () => ApiServiceInternal.SetComponentEnable(componentId));
        }

        public SetComponentDisableResponse SetComponentDisable(Guid componentId, DateTime? toDate, string comment)
        {
            return Execute("SetComponentDisable", () => ApiServiceInternal.SetComponentDisable(componentId, toDate, comment));
        }

        public DeleteComponentResponse DeleteComponent(Guid componentId)
        {
            return Execute("DeleteComponent", () => ApiServiceInternal.DeleteComponent(componentId));
        }

        public GetComponentTypeResponse GetComponentType(GetComponentTypeData data)
        {
            return Execute("GetComponentType", () => ApiServiceInternal.GetComponentType(data));
        }

        public GetOrCreateComponentTypeResponse GetOrCreateComponentType(GetOrCreateComponentTypeData data)
        {
            return Execute("GetOrCreateComponentType", () => ApiServiceInternal.GetOrCreateComponentType(data));
        }

        public UpdateComponentTypeResponse UpdateComponentType(UpdateComponentTypeData data)
        {
            return Execute("UpdateComponentType", () => ApiServiceInternal.UpdateComponentType(data));
        }

        public SendEventResponse SendEvent(SendEventData data)
        {
            return Execute("SendEvent", () => ApiServiceInternal.SendEvent(data));
        }

        public GetEventByIdResponse GetEventById(Guid eventId)
        {
            return Execute("GetEventById", () => ApiServiceInternal.GetEventById(eventId));
        }

        public GetEventsResponse GetEvents(GetEventsData data)
        {
            return Execute("GetEvents", () => ApiServiceInternal.GetEvents(data));
        }

        public JoinEventsResponse JoinEvents(List<JoinEventData> data)
        {
            return Execute("JoinEvents", () => ApiServiceInternal.JoinEvents(data));
        }

        public GetOrCreateUnitTestResponse GetOrCreateUnitTest(Guid componentId, GetOrCreateUnitTestData data)
        {
            return Execute("GetOrCreateUnitTest", () => ApiServiceInternal.GetOrCreateUnitTest(componentId, data));
        }

        public GetOrCreateUnitTestTypeResponse GetOrCreateUnitTestType(GetOrCreateUnitTestTypeData data)
        {
            return Execute("GetOrCreateUnitTestType", () => ApiServiceInternal.GetOrCreateUnitTestType(data));
        }

        public SendUnitTestResultResponse SendUnitTestResult(Guid unitTestId, SendUnitTestResultData data)
        {
            return Execute("SendUnitTestResult", () => ApiServiceInternal.SendUnitTestResult(unitTestId, data));
        }

        public SendUnitTestResultsResponse SendUnitTestResults(SendUnitTestResultsData[] data)
        {
            return Execute("SendUnitTestResults", () => ApiServiceInternal.SendUnitTestResults(data));
        }

        public GetUnitTestStateResponse GetUnitTestState(Guid unitTestId)
        {
            return Execute("GetUnitTestState", () => ApiServiceInternal.GetUnitTestState(unitTestId));
        }

        public SetUnitTestDisableResponse SetUnitTestDisable(Guid unitTestId, SetUnitTestDisableRequestData data)
        {
            return Execute("SetUnitTestDisable", () => ApiServiceInternal.SetUnitTestDisable(unitTestId, data));
        }

        public SetUnitTestEnableResponse SetUnitTestEnable(Guid unitTestId)
        {
            return Execute("SetUnitTestEnable", () => ApiServiceInternal.SetUnitTestEnable(unitTestId));
        }

        public SendMetricResponse SendMetric(Guid componentId, SendMetricData data)
        {
            return Execute("SendMetric", () => ApiServiceInternal.SendMetric(componentId, data));
        }

        public SendMetricsResponse SendMetrics(Guid componentId, List<SendMetricData> data)
        {
            return Execute("SendMetrics", () => ApiServiceInternal.SendMetrics(componentId, data));
        }

        public GetMetricsHistoryResponse GetMetricsHistory(Guid componentId, GetMetricsHistoryFilter filter)
        {
            return Execute("GetMetricsHistory", () => ApiServiceInternal.GetMetricsHistory(componentId, filter));
        }

        public GetMetricsResponse GetMetrics(Guid componentId)
        {
            return Execute("GetMetrics", () => ApiServiceInternal.GetMetrics(componentId));
        }

        public GetMetricResponse GetMetric(Guid componentId, string metricName)
        {
            return Execute("GetMetric", () => ApiServiceInternal.GetMetric(componentId, metricName));
        }

        public SendLogResponse SendLog(SendLogData data)
        {
            return Execute("SendLog", () => ApiServiceInternal.SendLog(data));
        }

        public SendLogsResponse SendLogs(SendLogData[] data)
        {
            return Execute("SendLogs", () => ApiServiceInternal.SendLogs(data));
        }

        public GetLogsResponse GetLogs(Guid componentId, GetLogsFilter filter)
        {
            return Execute("GetLogs", () => ApiServiceInternal.GetLogs(componentId, filter));
        }

        public GetLogConfigResponse GetLogConfig(Guid componentId)
        {
            return Execute("GetLogConfig", () => ApiServiceInternal.GetLogConfig(componentId));
        }

        public GetChangedWebLogConfigsResponse GetChangedWebLogConfigs(DateTime lastUpdateDate, List<Guid> componentIds)
        {
            return Execute("GetLogConfig", () => ApiServiceInternal.GetChangedWebLogConfigs(lastUpdateDate, componentIds));
        }
    }
}
