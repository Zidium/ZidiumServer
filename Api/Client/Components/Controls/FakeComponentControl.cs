using System;
using System.Collections.Generic;
using Zidium.Api.Dto;

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

        public ComponentDto Info
        {
            get { return null; }
        }

        public WebLogConfig WebLogConfig
        {
            get { return new WebLogConfig(); }
        }

        public GetComponentByIdResponseDto GetParent()
        {
            return ResponseHelper.GetOfflineResponse<GetComponentByIdResponseDto>();
        }

        public GetChildComponentsResponseDto GetChildComponents()
        {
            return ResponseHelper.GetOfflineResponse<GetChildComponentsResponseDto>();
        }

        public IComponentControl GetOrCreateChildComponentControl(GetOrCreateComponentData data)
        {
            return new FakeComponentControl();
        }

        public UpdateComponentResponseDto Update(UpdateComponentData data)
        {
            return ResponseHelper.GetOfflineResponse<UpdateComponentResponseDto>();
        }

        public IComponentControl GetOrCreateChildFolderControl(GetOrCreateFolderData data)
        {
            return new FakeComponentControl();
        }

        public GetOrCreateComponentResponseDto GetOrCreateChildFolder(GetOrCreateFolderData data)
        {
            return ResponseHelper.GetOfflineResponse<GetOrCreateComponentResponseDto>();
        }

        public SetComponentDisableResponseDto Disable(string comment, DateTime date)
        {
            return ResponseHelper.GetOfflineResponse<SetComponentDisableResponseDto>();
        }

        public DeleteComponentResponseDto Delete()
        {
            return ResponseHelper.GetOfflineResponse<DeleteComponentResponseDto>();
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

        public GetOrCreateUnitTestResponseDto GetOrCreateUnitTest(string typeSystemName, string instanceSystemName)
        {
            return ResponseHelper.GetOfflineResponse<GetOrCreateUnitTestResponseDto>();
        }

        public GetOrCreateUnitTestResponseDto GetOrCreateUnitTest(IUnitTestTypeControl unitTestTypeControl, string systemName)
        {
            return ResponseHelper.GetOfflineResponse<GetOrCreateUnitTestResponseDto>();
        }

        public GetOrCreateUnitTestResponseDto GetOrCreateUnitTest(GetOrCreateUnitTestData data)
        {
            return ResponseHelper.GetOfflineResponse<GetOrCreateUnitTestResponseDto>();
        }

        public GetMetricsResponseDto GetCounter(string name)
        {
            return ResponseHelper.GetOfflineResponse<GetMetricsResponseDto>();
        }

        public GetMetricsHistoryResponseDto GetCounters(GetMetricsHistoryFilter filter)
        {
            return ResponseHelper.GetOfflineResponse<GetMetricsHistoryResponseDto>();
        }

        public ILog Log
        {
            get { return new FakeLog(); }
        }

        public GetLogsResponseDto GetLogs(GetLogsFilter filter)
        {
            return ResponseHelper.GetOfflineResponse<GetLogsResponseDto>();
        }

        public GetLogConfigResponseDto GetWebLogConfig()
        {
            return ResponseHelper.GetOfflineResponse<GetLogConfigResponseDto>();
        }

        public GetComponentTotalStateResponseDto GetTotalState(bool recalc)
        {
            return ResponseHelper.GetOfflineResponse<GetComponentTotalStateResponseDto>();
        }

        public GetComponentInternalStateResponseDto GetInternalState(bool recalc)
        {
            return ResponseHelper.GetOfflineResponse<GetComponentInternalStateResponseDto>();
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

        public SendEventResponseDto SendApplicationError(string errorTypeSystemName)
        {
            return ResponseHelper.GetOfflineResponse<SendEventResponseDto>();
        }

        public SendEventResponseDto SendApplicationError(string errorTypeSystemName, Exception exception)
        {
            return ResponseHelper.GetOfflineResponse<SendEventResponseDto>();
        }

        public SendEventResponseDto SendApplicationError(Exception exception)
        {
            return ResponseHelper.GetOfflineResponse<SendEventResponseDto>();
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

        public SendUnitTestResultsResponseDto SendUnitTestResults(List<SendUnitTestResultRequestDataDto> data)
        {
            return ResponseHelper.GetOfflineResponse<SendUnitTestResultsResponseDto>();
        }

        public GetMetricsResponseDto GetMetrics()
        {
            return ResponseHelper.GetOfflineResponse<GetMetricsResponseDto>();
        }

        public GetMetricsHistoryResponseDto GetMetricsHistory(GetMetricsHistoryFilter filter)
        {
            return ResponseHelper.GetOfflineResponse<GetMetricsHistoryResponseDto>();
        }

        public SendMetricsResponseDto SendMetrics(List<SendMetricData> data)
        {
            return ResponseHelper.GetOfflineResponse<SendMetricsResponseDto>();
        }

        public SendMetricResponseDto SendMetric(SendMetricData data)
        {
            return ResponseHelper.GetOfflineResponse<SendMetricResponseDto>();
        }

        public SendMetricResponseDto SendMetric(string name, double? value, TimeSpan actualInterval)
        {
            return ResponseHelper.GetOfflineResponse<SendMetricResponseDto>();
        }

        public SendMetricResponseDto SendMetric(string name, double? value)
        {
            return ResponseHelper.GetOfflineResponse<SendMetricResponseDto>();
        }

        public GetMetricResponseDto GetMetric(string name)
        {
            return ResponseHelper.GetOfflineResponse<GetMetricResponseDto>();
        }

        public SetComponentEnableResponseDto Enable()
        {
            return ResponseHelper.GetOfflineResponse<SetComponentEnableResponseDto>();
        }

        public SetComponentDisableResponseDto Disable(string comment)
        {
            return ResponseHelper.GetOfflineResponse<SetComponentDisableResponseDto>();
        }

        public SendEventResponseDto SendComponentEvent(string eventTypeSystemName)
        {
            return ResponseHelper.GetOfflineResponse<SendEventResponseDto>();
        }

        public SendEventResponseDto SendComponentEvent(string eventTypeSystemName, string message)
        {
            return ResponseHelper.GetOfflineResponse<SendEventResponseDto>();
        }

    }
}
