using System;
using Zidium.Api.Dto;
using Zidium.Core.Common.Helpers;

namespace Zidium.Core.Api.Dispatcher
{
    public class DispatcherClient
    {
        #region Others

        public string ProgramName { get; protected set; }

        public DispatcherClient(string programName)
        {
            ProgramName = programName;
        }

        protected TRequest GetRequest<TRequest>()
            where TRequest : RequestDto, new()
        {
            var token = SystemAccountHelper.GetApiToken();
            var request = new TRequest()
            {
                Token = token
            };
            return request;
        }

        public GetServerTimeResponseDto GetServerTime()
        {
            var dispatcher = DispatcherHelper.GetDispatcherService();
            return dispatcher.GetServerTime();
        }

        public SendSmsResponse SendSms(SendSmsRequestData data)
        {
            var request = GetRequest<SendSmsRequest>();
            request.Data = data;
            var dispatcher = DispatcherHelper.GetDispatcherService();
            return dispatcher.SendSms(request);
        }

        public GetLogicSettingsResponse GetLogicSettings()
        {
            var request = GetRequest<GetLogicSettingsRequest>();
            var dispatcher = DispatcherHelper.GetDispatcherService();
            return dispatcher.GetLogicSettings(request);
        }

        #endregion

        #region Components

        public GetRootComponentResponseDto GetRoot()
        {
            var request = GetRequest<GetRootComponentRequestDto>();
            var dispatcher = DispatcherHelper.GetDispatcherService();
            return dispatcher.GetRootComponent(request);
        }

        public GetOrCreateComponentResponseDto GetOrCreateComponent(GetOrCreateComponentRequestDataDto data)
        {
            var request = GetRequest<GetOrCreateComponentRequestDto>();
            request.Data = data;
            var dispatcher = DispatcherHelper.GetDispatcherService();
            return dispatcher.GetOrCreateComponent(request);
        }

        public CreateComponentResponse CreateComponent(CreateComponentRequestData data)
        {
            var request = GetRequest<CreateComponentRequest>();
            request.Data = data;
            var dispatcher = DispatcherHelper.GetDispatcherService();
            return dispatcher.CreateComponent(request);
        }

        public GetComponentTotalStateResponseDto GetComponentTotalState(Guid componentId, bool recalc = false)
        {
            var request = GetRequest<GetComponentTotalStateRequestDto>();
            request.Data = new GetComponentTotalStateRequestDataDto()
            {
                ComponentId = componentId,
                Recalc = recalc
            };
            var dispatcher = DispatcherHelper.GetDispatcherService();
            return dispatcher.GetComponentTotalState(request);
        }

        public GetComponentInternalStateResponseDto GetComponentInternalState(Guid componentId)
        {
            var request = GetRequest<GetComponentInternalStateRequestDto>();
            request.Data = new GetComponentInternalStateRequestDataDto()
            {
                ComponentId = componentId
            };
            var dispatcher = DispatcherHelper.GetDispatcherService();
            return dispatcher.GetComponentInternalState(request);
        }

        public UpdateComponentStateResponse UpdateComponentState(Guid componentId)
        {
            var request = GetRequest<UpdateComponentStateRequest>();
            request.Data = new UpdateComponentStateRequestData()
            {
                ComponentId = componentId
            };
            var dispatcher = DispatcherHelper.GetDispatcherService();
            return dispatcher.UpdateComponentState(request);
        }

        public SetComponentEnableResponseDto SetComponentEnable(Guid componentId)
        {
            var request = GetRequest<SetComponentEnableRequestDto>();
            request.Data = new SetComponentEnableRequestDataDto()
            {
                ComponentId = componentId
            };
            var dispatcher = DispatcherHelper.GetDispatcherService();
            return dispatcher.SetComponentEnable(request);
        }

        public SetComponentDisableResponseDto SetComponentDisable(SetComponentDisableRequestDataDto data)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }
            var request = GetRequest<SetComponentDisableRequestDto>();
            request.Data = data;
            var dispatcher = DispatcherHelper.GetDispatcherService();
            return dispatcher.SetComponentDisable(request);
        }

        public DeleteComponentResponseDto DeleteComponent(Guid componentId)
        {
            var request = GetRequest<DeleteComponentRequestDto>();
            request.Data = new DeleteComponentRequestDataDto()
            {
                ComponentId = componentId
            };
            var dispatcher = DispatcherHelper.GetDispatcherService();
            return dispatcher.DeleteComponent(request);
        }

        public UpdateEventsStatusesResponse UpdateEventsStatuses(int maxCount)
        {
            var request = GetRequest<UpdateEventsStatusesRequest>();
            request.Data = new UpdateEventsStatusesRequestData
            {
                MaxCount = maxCount
            };
            var dispatcher = DispatcherHelper.GetDispatcherService();
            return dispatcher.UpdateEventsStatuses(request);
        }

        public UpdateComponentResponseDto UpdateComponent(UpdateComponentRequestDataDto data)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }
            var request = GetRequest<UpdateComponentRequestDto>();
            request.Data = data;
            var dispatcher = DispatcherHelper.GetDispatcherService();
            return dispatcher.UpdateComponent(request);
        }

        public GetComponentByIdResponseDto GetComponentById(Guid id)
        {
            var request = GetRequest<GetComponentByIdRequestDto>();
            request.Data = new GetComponentByIdRequestDataDto()
            {
                ComponentId = id
            };
            var dispatcher = DispatcherHelper.GetDispatcherService();
            return dispatcher.GetComponentById(request);
        }

        public GetComponentAndChildIdsResponse GetComponentAndChildIds(Guid componentId)
        {
            var request = GetRequest<GetComponentAndChildIdsRequest>();
            request.Data = new GetComponentAndChildIdsRequestData()
            {
                ComponentId = componentId
            };
            var dispatcher = DispatcherHelper.GetDispatcherService();
            return dispatcher.GetComponentAndChildIds(request);
        }

        #endregion

        #region ComponentTypes

        public GetOrCreateComponentTypeResponseDto GetOrCreateComponentType(GetOrCreateComponentTypeRequestDataDto data)
        {
            var request = GetRequest<GetOrCreateComponentTypeRequestDto>();
            request.Data = data;
            var dispatcher = DispatcherHelper.GetDispatcherService();
            return dispatcher.GetOrCreateComponentType(request);
        }

        public UpdateComponentTypeResponseDto UpdateComponentType(UpdateComponentTypeRequestDataDto data)
        {
            var request = GetRequest<UpdateComponentTypeRequestDto>();
            request.Data = data;
            var dispatcher = DispatcherHelper.GetDispatcherService();
            return dispatcher.UpdateComponentType(request);
        }

        public DeleteComponentTypeResponse DeleteComponentType(Guid componentTypeId)
        {
            var request = GetRequest<DeleteComponentTypeRequest>();
            request.Data = new DeleteComponentTypeRequestData()
            {
                ComponentTypeId = componentTypeId
            };
            var dispatcher = DispatcherHelper.GetDispatcherService();
            return dispatcher.DeleteComponentType(request);
        }

        #endregion

        #region События

        public SendEventResponseDto SendEvent(SendEventRequestDataDto data)
        {
            var request = GetRequest<SendEventRequestDto>();
            request.Data = data;
            var dispatcher = DispatcherHelper.GetDispatcherService();
            return dispatcher.SendEvent(request);
        }

        public GetEventByIdResponseDto GetEventById(Guid eventId)
        {
            var data = new GetEventByIdRequestDataDto()
            {
                EventId = eventId
            };
            var request = GetRequest<GetEventByIdRequestDto>();
            request.Data = data;
            var dispatcher = DispatcherHelper.GetDispatcherService();
            return dispatcher.GetEventById(request);
        }

        #endregion

        #region Типы событий

        public UpdateEventTypeResponse UpdateEventType(UpdateEventTypeRequestData data)
        {
            var request = GetRequest<UpdateEventTypeRequest>();
            request.Data = data;
            var dispatcher = DispatcherHelper.GetDispatcherService();
            return dispatcher.UpdateEventType(request);
        }

        #endregion

        #region Проверки

        public GetOrCreateUnitTestResponseDto GetOrCreateUnitTest(GetOrCreateUnitTestRequestDataDto data)
        {
            var request = GetRequest<GetOrCreateUnitTestRequestDto>();
            request.Data = data;
            var dispatcher = DispatcherHelper.GetDispatcherService();
            return dispatcher.GetOrCreateUnitTest(request);
        }

        public GetUnitTestTypeByIdResponse GetUnitTestTypeById(Guid id)
        {
            var request = GetRequest<GetUnitTestTypeByIdRequest>();
            request.Data = new GetUnitTestTypeByIdRequestData()
            {
                Id = id
            };
            var dispatcher = DispatcherHelper.GetDispatcherService();
            return dispatcher.GetUnitTestTypeById(request);
        }

        public UpdateUnitTestResponse UpdateUnitTest(UpdateUnitTestRequestData data)
        {
            var request = GetRequest<UpdateUnitTestRequest>();
            request.Data = data;
            var dispatcher = DispatcherHelper.GetDispatcherService();
            return dispatcher.UpdateUnitTest(request);
        }

        public GetOrCreateUnitTestTypeResponseDto GetOrCreateUnitTestType(GetOrCreateUnitTestTypeRequestDataDto data)
        {
            var request = GetRequest<GetOrCreateUnitTestTypeRequestDto>();
            request.Data = data;
            var dispatcher = DispatcherHelper.GetDispatcherService();
            return dispatcher.GetOrCreateUnitTestType(request);
        }

        public UpdateUnitTestTypeResponse UpdateUnitTestType(UpdateUnitTestTypeRequestData data)
        {
            var request = GetRequest<UpdateUnitTestTypeRequest>();
            request.Data = data;
            var dispatcher = DispatcherHelper.GetDispatcherService();
            return dispatcher.UpdateUnitTestType(request);
        }

        public DeleteUnitTestTypeResponse DeleteUnitTestType(DeleteUnitTestTypeRequestData data)
        {
            var request = GetRequest<DeleteUnitTestTypeRequest>();
            request.Data = data;
            var dispatcher = DispatcherHelper.GetDispatcherService();
            return dispatcher.DeleteUnitTestType(request);
        }

        public SetUnitTestNextTimeResponse SetUnitTestNextTime(SetUnitTestNextTimeRequestData data)
        {
            var request = GetRequest<SetUnitTestNextTimeRequest>();
            request.Data = data;
            var dispatcher = DispatcherHelper.GetDispatcherService();
            return dispatcher.SetUnitTestNextTime(request);
        }

        public SetUnitTestNextStepProcessTimeResponse SetUnitTestNextStepProcessTime(SetUnitTestNextStepProcessTimeRequestData data)
        {
            var request = GetRequest<SetUnitTestNextStepProcessTimeRequest>();
            request.Data = data;
            var dispatcher = DispatcherHelper.GetDispatcherService();
            return dispatcher.SetUnitTestNextStepProcessTime(request);
        }

        public DeleteUnitTestResponse DeleteUnitTest(Guid unitTestId)
        {
            var request = GetRequest<DeleteUnitTestRequest>();
            request.Data = new DeleteUnitTestRequestData()
            {
                UnitTestId = unitTestId
            };
            var dispatcher = DispatcherHelper.GetDispatcherService();
            return dispatcher.DeleteUnitTest(request);
        }

        public GetUnitTestStateResponseDto GetUnitTestState(Guid unitTestId)
        {
            var request = GetRequest<GetUnitTestStateRequestDto>();
            request.Data = new GetUnitTestStateRequestDataDto()
            {
                UnitTestId = unitTestId
            };
            var dispatcher = DispatcherHelper.GetDispatcherService();
            return dispatcher.GetUnitTestState(request);
        }

        public SendUnitTestResultResponseDto SendUnitTestResult(
            SendUnitTestResultRequestDataDto data)
        {
            var request = GetRequest<SendUnitTestResultRequestDto>();
            request.Data = data;
            var dispatcher = DispatcherHelper.GetDispatcherService();
            return dispatcher.SendUnitTestResult(request);
        }

        public SendUnitTestResultsResponseDto SendUnitTestResults(
            SendUnitTestResultRequestDataDto[] data)
        {
            var request = GetRequest<SendUnitTestResultsRequestDto>();
            request.Data = data;
            var dispatcher = DispatcherHelper.GetDispatcherService();
            return dispatcher.SendUnitTestResults(request);
        }

        public RecalcUnitTestsResultsResponse RecalcUnitTestsResults(int maxCount)
        {
            var request = GetRequest<RecalcUnitTestsResultsRequest>();
            request.Data = new RecalcUnitTestsResultsRequestData
            {
                MaxCount = maxCount
            };
            var dispatcher = DispatcherHelper.GetDispatcherService();
            return dispatcher.RecalcUnitTestsResults(request);
        }

        public SetUnitTestEnableResponseDto SetUnitTestEnable(Guid unitTestId)
        {
            var request = GetRequest<SetUnitTestEnableRequestDto>();
            request.Data = new SetUnitTestEnableRequestDataDto()
            {
                UnitTestId = unitTestId
            };
            var dispatcher = DispatcherHelper.GetDispatcherService();
            return dispatcher.SetUnitTestEnable(request);
        }

        public SetUnitTestDisableResponseDto SetUnitTestDisable(SetUnitTestDisableRequestDataDto data)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }
            var request = GetRequest<SetUnitTestDisableRequestDto>();
            request.Data = data;
            var dispatcher = DispatcherHelper.GetDispatcherService();
            return dispatcher.SetUnitTestDisable(request);
        }

        public AddPingUnitTestResponse AddPingUnitTest(AddPingUnitTestRequestData data)
        {
            var request = GetRequest<AddPingUnitTestRequest>();
            request.Data = data;
            var dispatcher = DispatcherHelper.GetDispatcherService();
            return dispatcher.AddPingUnitTest(request);
        }

        public AddHttpUnitTestResponse AddHttpUnitTest(AddHttpUnitTestRequestData data)
        {
            var request = GetRequest<AddHttpUnitTestRequest>();
            request.Data = data;
            var dispatcher = DispatcherHelper.GetDispatcherService();
            return dispatcher.AddHttpUnitTest(request);
        }

        #endregion

        #region Метрики

        public GetMetricResponseDto GetMetric(Guid componentId, string name)
        {
            var request = GetRequest<GetMetricRequestDto>();
            request.Data = new GetMetricRequestDataDto()
            {
                ComponentId = componentId,
                Name = name
            };
            var dispatcher = DispatcherHelper.GetDispatcherService();
            return dispatcher.GetMetric(request);
        }

        public GetMetricsResponseDto GetMetrics(Guid componentId)
        {
            var request = GetRequest<GetMetricsRequestDto>();
            request.Data = new GetMetricsRequestDataDto()
            {
                ComponentId = componentId
            };
            var dispatcher = DispatcherHelper.GetDispatcherService();
            return dispatcher.GetMetrics(request);
        }

        public DeleteMetricResponse DeleteMetric(DeleteMetricRequestData data)
        {
            var request = GetRequest<DeleteMetricRequest>();
            request.Data = data;
            var dispatcher = DispatcherHelper.GetDispatcherService();
            return dispatcher.DeleteMetric(request);
        }

        public UpdateMetricsResponse CalculateMetrics(int maxCount)
        {
            var request = GetRequest<UpdateMetricsRequest>();
            request.Data = new UpdateMetricsRequestData()
            {
                MaxCount = maxCount
            };
            var dispatcher = DispatcherHelper.GetDispatcherService();
            return dispatcher.UpdateMetrics(request);
        }

        public SendMetricResponseDto SendMetric(SendMetricRequestDataDto data)
        {
            var request = GetRequest<SendMetricRequestDto>();
            request.Data = data;
            var dispatcher = DispatcherHelper.GetDispatcherService();
            return dispatcher.SendMetric(request);
        }

        public SetMetricDisableResponse SetMetricDisable(SetMetricDisableRequestData data)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }
            var request = GetRequest<SetMetricDisableRequest>();
            request.Data = data;
            var dispatcher = DispatcherHelper.GetDispatcherService();
            return dispatcher.SetMetricDisable(request);
        }

        public SetMetricEnableResponse SetMetricEnable(Guid metricId)
        {
            var request = GetRequest<SetMetricEnableRequest>();
            request.Data = new SetMetricEnableRequestData()
            {
                MetricId = metricId
            };
            var dispatcher = DispatcherHelper.GetDispatcherService();
            return dispatcher.SetMetricEnable(request);
        }

        public UpdateMetricResponse UpdateMetric(UpdateMetricRequestData data)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }
            var request = GetRequest<UpdateMetricRequest>();
            request.Data = data;
            var dispatcher = DispatcherHelper.GetDispatcherService();
            return dispatcher.UpdateMetric(request);
        }

        public CreateMetricResponse CreateMetric(CreateMetricRequestData data)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }
            var request = GetRequest<CreateMetricRequest>();
            request.Data = data;
            var dispatcher = DispatcherHelper.GetDispatcherService();
            return dispatcher.CreateMetric(request);
        }

        #endregion

        #region MetricTypes

        public CreateMetricTypeResponse CreateMetricType(CreateMetricTypeRequestData data)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }
            var request = GetRequest<CreateMetricTypeRequest>();
            request.Data = data;
            var dispatcher = DispatcherHelper.GetDispatcherService();
            return dispatcher.CreateMetricType(request);
        }

        public UpdateMetricTypeResponse UpdateMetricType(UpdateMetricTypeRequestData data)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }
            var request = GetRequest<UpdateMetricTypeRequest>();
            request.Data = data;
            var dispatcher = DispatcherHelper.GetDispatcherService();
            return dispatcher.UpdateMetricType(request);
        }

        public DeleteMetricTypeResponse DeleteMetricType(Guid metricTypeId)
        {
            var request = GetRequest<DeleteMetricTypeRequest>();
            request.Data = new DeleteMetricTypeRequestData()
            {
                MetricTypeId = metricTypeId
            };
            var dispatcher = DispatcherHelper.GetDispatcherService();
            return dispatcher.DeleteMetricType(request);
        }

        #endregion

        #region Лог

        public SendLogResponseDto SendLog(SendLogRequestDataDto data)
        {
            var request = GetRequest<SendLogRequestDto>();
            request.Data = data;
            var dispatcher = DispatcherHelper.GetDispatcherService();
            return dispatcher.SendLog(request);
        }

        public GetLogsResponseDto GetLogs(GetLogsRequestDataDto data)
        {
            var request = GetRequest<GetLogsRequestDto>();
            request.Data = data;
            var dispatcher = DispatcherHelper.GetDispatcherService();
            return dispatcher.GetLogs(request);
        }

        #endregion

        #region Limits

        public GetAccountLimitsResponse GetAccountLimits(int archiveDays)
        {
            var request = GetRequest<GetAccountLimitsRequest>();
            request.Data = new GetAccountLimitsRequestData()
            {
                ArchiveDays = archiveDays
            };
            var dispatcher = DispatcherHelper.GetDispatcherService();
            return dispatcher.GetAccountLimits(request);
        }

        #endregion

        #region Подписки

        public CreateSubscriptionResponse CreateSubscription(CreateSubscriptionRequestData data)
        {
            var request = GetRequest<CreateSubscriptionRequest>();
            request.Data = data;
            var dispatcher = DispatcherHelper.GetDispatcherService();
            return dispatcher.CreateSubscription(request);
        }

        public CreateUserDefaultSubscriptionResponse CreateUserDefaultSubscription(CreateUserDefaultSubscriptionRequestData data)
        {
            var request = GetRequest<CreateUserDefaultSubscriptionRequest>();
            request.Data = data;
            var dispatcher = DispatcherHelper.GetDispatcherService();
            return dispatcher.CreateUserDefaultSubscription(request);
        }

        public UpdateSubscriptionResponse UpdateSubscription(UpdateSubscriptionRequestData data)
        {
            var request = GetRequest<UpdateSubscriptionRequest>();
            request.Data = data;
            var dispatcher = DispatcherHelper.GetDispatcherService();
            return dispatcher.UpdateSubscription(request);
        }

        public DeleteSubscriptionResponse DeleteSubscription(Guid subscriptionId)
        {
            var request = GetRequest<DeleteSubscriptionRequest>();
            request.Data = new DeleteSubscriptionRequestData()
            {
                SubscriptionId = subscriptionId
            };
            var dispatcher = DispatcherHelper.GetDispatcherService();
            return dispatcher.DeleteSubscription(request);
        }

        #endregion

    }
}
