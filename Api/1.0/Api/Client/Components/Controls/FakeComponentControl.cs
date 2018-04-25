using System;
using System.Collections.Generic;

namespace Zidium.Api
{
    /// <summary>
    /// Заглушка. 
    /// В Отличие от ComponentControlOffline экземпляр даного класса можно получить без IClient-а.
    /// </summary>
    public class FakeComponentControl : IComponentControl
    {
        public FakeComponentControl()
        {
        }

        public FakeComponentControl(string name)
        {
        }

        public IComponentTypeControl Type
        {
            get { return new FakeComponentTypeControl(); }
        }

        public bool IsRoot
        {
            get { return false; }
        }

        public bool IsFolder
        {
            get { return false; }
        }

        public string Version
        {
            get { return null; }
        }

        public ComponentInfo Info
        {
            get { return null; }
        }

        public WebLogConfig WebLogConfig
        {
            get { return new WebLogConfig(); }
        }

        public GetComponentByIdResponse GetParent()
        {
            return ResponseHelper.GetOfflineResponse<GetComponentByIdResponse>();
        }

        public GetChildComponentsResponse GetChildComponents()
        {
            return ResponseHelper.GetOfflineResponse<GetChildComponentsResponse>();
        }

        public IComponentControl GetOrCreateChildComponentControl(GetOrCreateComponentData data)
        {
            return new FakeComponentControl();
        }

        public UpdateComponentResponse Update(UpdateComponentData data)
        {
            return ResponseHelper.GetOfflineResponse<UpdateComponentResponse>();
        }

        public IComponentControl GetOrCreateChildFolderControl(GetOrCreateFolderData data)
        {
            return new FakeComponentControl();
        }

        public GetOrCreateComponentResponse GetOrCreateChildFolder(GetOrCreateFolderData data)
        {
            return ResponseHelper.GetOfflineResponse<GetOrCreateComponentResponse>();
        }

        public SetComponentDisableResponse Disable(string comment, DateTime date)
        {
            return ResponseHelper.GetOfflineResponse<SetComponentDisableResponse>();
        }

        public DeleteComponentResponse Delete()
        {
            return ResponseHelper.GetOfflineResponse<DeleteComponentResponse>();
        }

        public ApplicationErrorData CreateApplicationError(string errorTypeSystemName)
        {
            return new ApplicationErrorData(this, errorTypeSystemName);
        }

        public ApplicationErrorData CreateApplicationError(string errorTypeSystemName, string message)
        {
            return new ApplicationErrorData(this, errorTypeSystemName)
            {
                Message = message
            };
        }

        public ApplicationErrorData CreateApplicationError(string errorTypeSystemName, Exception exception)
        {
            return new ApplicationErrorData(this, errorTypeSystemName)
            {
                Message = exception.Message
            };
        }

        public ApplicationErrorData CreateApplicationError(Exception exception)
        {
            return new ApplicationErrorData(this, exception.Message)
            {
                Message = exception.Message
            };
        }

        public AddEventResult AddApplicationError(string errorTypeSystemName)
        {
            return new AddEventResult(new BufferEventData(this, CreateApplicationError(errorTypeSystemName)));
        }

        public ComponentEventData CreateComponentEvent(string typeSystemName)
        {
            return new ComponentEventData(this, typeSystemName);
        }

        public ComponentEventData CreateComponentEvent(string typeSystemName, string message)
        {
            return new ComponentEventData(this, typeSystemName)
            {
                Message = message
            };
        }

        public GetOrCreateUnitTestResponse GetOrCreateUnitTest(string typeSystemName, string instanceSystemName)
        {
            return ResponseHelper.GetOfflineResponse<GetOrCreateUnitTestResponse>();
        }

        public GetOrCreateUnitTestResponse GetOrCreateUnitTest(IUnitTestTypeControl unitTestTypeControl, string systemName)
        {
            return ResponseHelper.GetOfflineResponse<GetOrCreateUnitTestResponse>();
        }

        public GetOrCreateUnitTestResponse GetOrCreateUnitTest(GetOrCreateUnitTestData data)
        {
            return ResponseHelper.GetOfflineResponse<GetOrCreateUnitTestResponse>();
        }

        public GetMetricsResponse GetCounter(string name)
        {
            return ResponseHelper.GetOfflineResponse<GetMetricsResponse>();
        }

        public GetMetricsHistoryResponse GetCounters(GetMetricsHistoryFilter filter)
        {
            return ResponseHelper.GetOfflineResponse<GetMetricsHistoryResponse>();
        }

        public ILog Log
        {
            get { return new FakeLog(); }
        }

        public GetLogsResponse GetLogs(GetLogsFilter filter)
        {
            return ResponseHelper.GetOfflineResponse<GetLogsResponse>();
        }

        public GetLogConfigResponse GetWebLogConfig()
        {
            return ResponseHelper.GetOfflineResponse<GetLogConfigResponse>();
        }

        public GetComponentTotalStateResponse GetTotalState(bool recalc)
        {
            return ResponseHelper.GetOfflineResponse<GetComponentTotalStateResponse>();
        }

        public GetComponentInternalStateResponse GetInternalState(bool recalc)
        {
            return ResponseHelper.GetOfflineResponse<GetComponentInternalStateResponse>();
        }

        public IClient Client
        {
            get { return null; }
        }

        public string SystemName { get; protected set; }

        public bool IsFake()
        {
            return true;
        }

        public void Detach()
        {
        }

        public void Dispose()
        {
        }

        public AddEventResult AddApplicationError(string errorTypeSystemName, Exception exception)
        {
            return new AddEventResult(new BufferEventData(this, CreateApplicationError(errorTypeSystemName, exception)));
        }

        public AddEventResult AddApplicationError(string errorTypeSystemName, string message)
        {
            return new AddEventResult(new BufferEventData(this, CreateApplicationError(errorTypeSystemName, message)));
        }

        public AddEventResult AddApplicationError(Exception exception)
        {
            return new AddEventResult(new BufferEventData(this, CreateApplicationError(exception)));
        }

        public AddEventResult AddComponentEvent(string typeSystemName)
        {
            return new AddEventResult(new BufferEventData(this, CreateComponentEvent(typeSystemName)));
        }

        public AddEventResult AddComponentEvent(string typeSystemName, string message)
        {
            return new AddEventResult(new BufferEventData(this, CreateComponentEvent(typeSystemName, message)));
        }

        public IComponentControl GetOrCreateChildComponentControl(IComponentTypeControl type, string systemName)
        {
            return new FakeComponentControl();
        }

        public IComponentControl GetOrCreateChildComponentControl(string typeSystemName, string systemName)
        {
            return new FakeComponentControl();
        }

        public IComponentControl GetOrCreateChildComponentControl(IComponentTypeControl type, string systemName, string version)
        {
            return new FakeComponentControl();
        }

        public SendEventResponse SendApplicationError(string errorTypeSystemName)
        {
            return ResponseHelper.GetOfflineResponse<SendEventResponse>();
        }

        public SendEventResponse SendApplicationError(string errorTypeSystemName, Exception exception)
        {
            return ResponseHelper.GetOfflineResponse<SendEventResponse>();
        }

        public SendEventResponse SendApplicationError(Exception exception)
        {
            return ResponseHelper.GetOfflineResponse<SendEventResponse>();
        }

        public IComponentControl GetOrCreateChildFolderControl(string systemName)
        {
            return new FakeComponentControl();
        }

        public IUnitTestControl GetOrCreateUnitTestControl(string typeSystemName, string instanceSystemName)
        {
            return new FakeUnitTestControl();
        }

        public IUnitTestControl GetOrCreateUnitTestControl(IUnitTestTypeControl unitTestTypeControl, string systemName)
        {
            return new FakeUnitTestControl();
        }

        public IUnitTestControl GetOrCreateUnitTestControl(GetOrCreateUnitTestData data)
        {
            return new FakeUnitTestControl();
        }

        public IUnitTestControl GetOrCreateUnitTestControl(string systemName)
        {
            return new FakeUnitTestControl();
        }

        public GetMetricsResponse GetMetrics()
        {
            return ResponseHelper.GetOfflineResponse<GetMetricsResponse>();
        }

        public GetMetricsHistoryResponse GetMetricsHistory(GetMetricsHistoryFilter filter)
        {
            return ResponseHelper.GetOfflineResponse<GetMetricsHistoryResponse>();
        }

        public SendMetricsResponse SendMetrics(List<SendMetricData> data)
        {
            return ResponseHelper.GetOfflineResponse<SendMetricsResponse>();
        }

        public SendMetricResponse SendMetric(SendMetricData data)
        {
            return ResponseHelper.GetOfflineResponse<SendMetricResponse>();
        }

        public SendMetricResponse SendMetric(string name, double? value, TimeSpan actualInterval)
        {
            return ResponseHelper.GetOfflineResponse<SendMetricResponse>();
        }

        public SendMetricResponse SendMetric(string name, double? value)
        {
            return ResponseHelper.GetOfflineResponse<SendMetricResponse>();
        }

        public GetMetricResponse GetMetric(string name)
        {
            return ResponseHelper.GetOfflineResponse<GetMetricResponse>();
        }

        public SetComponentEnableResponse Enable()
        {
            return ResponseHelper.GetOfflineResponse<SetComponentEnableResponse>();
        }

        public SetComponentDisableResponse Disable(string comment)
        {
            return ResponseHelper.GetOfflineResponse<SetComponentDisableResponse>();
        }

        public SendEventResponse SendComponentEvent(string eventTypeSystemName)
        {
            return ResponseHelper.GetOfflineResponse<SendEventResponse>();
        }

        public SendEventResponse SendComponentEvent(string eventTypeSystemName, string message)
        {
            return ResponseHelper.GetOfflineResponse<SendEventResponse>();
        }
    }
}
