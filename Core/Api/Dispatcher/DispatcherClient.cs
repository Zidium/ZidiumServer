using System;
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

        protected TRequest GetRequest<TRequest>(Guid? accountId)
            where TRequest : Request, new()
        {
            var token = SystemAccountHelper.GetLocalToken(accountId);
            var request = new TRequest()
            {
                ProgramName = ProgramName,
                RequestId = Guid.NewGuid(),
                Token = token
            };
            return request;
        }

        public GetServerTimeResponse GetServerTime()
        {
            var dispatcher = DispatcherHelper.GetDispatcherService();
            return dispatcher.GetServerTime();
        }

        #endregion

        #region Components

        public GetRootComponentResponse GetRoot(Guid accountId)
        {
            var request = GetRequest<GetRootComponentRequest>(accountId);
            var dispatcher = DispatcherHelper.GetDispatcherService();
            return dispatcher.GetRootComponent(request);
        }

        public GetOrCreateComponentResponse GetOrCreateComponent(Guid accountId, GetOrCreateComponentRequestData data)
        {
            var request = GetRequest<GetOrCreateComponentRequest>(accountId);
            request.Data = data;
            var dispatcher = DispatcherHelper.GetDispatcherService();
            return dispatcher.GetOrCreateComponent(request);
        }

        public CreateComponentResponse CreateComponent(Guid accountId, CreateComponentRequestData data)
        {
            var request = GetRequest<CreateComponentRequest>(accountId);
            request.Data = data;
            var dispatcher = DispatcherHelper.GetDispatcherService();
            return dispatcher.CreateComponent(request);
        }

        public GetComponentTotalStateResponse GetComponentTotalState(Guid accountId, Guid componentId, bool recalc = false)
        {
            var request = GetRequest<GetComponentTotalStateRequest>(accountId);
            request.Data = new GetComponentTotalStateRequestData()
            {
                ComponentId = componentId,
                Recalc = recalc
            };
            var dispatcher = DispatcherHelper.GetDispatcherService();
            return dispatcher.GetComponentTotalState(request);
        }

        public GetComponentInternalStateResponse GetComponentInternalState(Guid accountId, Guid componentId)
        {
            var request = GetRequest<GetComponentInternalStateRequest>(accountId);
            request.Data = new GetComponentInternalStateRequestData()
            {
                ComponentId = componentId
            };
            var dispatcher = DispatcherHelper.GetDispatcherService();
            return dispatcher.GetComponentInternalState(request);
        }

        public UpdateComponentStateResponse UpdateComponentState(Guid accountId, Guid componentId)
        {
            var request = GetRequest<UpdateComponentStateRequest>(accountId);
            request.Data = new UpdateComponentStateRequestData()
            {
                ComponentId = componentId
            };
            var dispatcher = DispatcherHelper.GetDispatcherService();
            return dispatcher.UpdateComponentState(request);
        }

        public SetComponentEnableResponse SetComponentEnable(Guid accountId, Guid componentId)
        {
            var request = GetRequest<SetComponentEnableRequest>(accountId);
            request.Data = new SetComponentEnableRequestData()
            {
                ComponentId = componentId
            };
            var dispatcher = DispatcherHelper.GetDispatcherService();
            return dispatcher.SetComponentEnable(request);
        }

        public SetComponentDisableResponse SetComponentDisable(Guid accountId, SetComponentDisableRequestData data)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }
            var request = GetRequest<SetComponentDisableRequest>(accountId);
            request.Data = data;
            var dispatcher = DispatcherHelper.GetDispatcherService();
            return dispatcher.SetComponentDisable(request);
        }

        public DeleteComponentResponse DeleteComponent(Guid accountId, Guid componentId)
        {
            var request = GetRequest<DeleteComponentRequest>(accountId);
            request.Data = new DeleteComponentRequestData()
            {
                ComponentId = componentId
            };
            var dispatcher = DispatcherHelper.GetDispatcherService();
            return dispatcher.DeleteComponent(request);
        }

        public UpdateEventsStatusesResponse UpdateEventsStatuses(Guid accountId, int maxCount)
        {
            var request = GetRequest<UpdateEventsStatusesRequest>(accountId);
            request.Data = new UpdateEventsStatusesRequestData
            {
                MaxCount = maxCount
            };
            var dispatcher = DispatcherHelper.GetDispatcherService();
            return dispatcher.UpdateEventsStatuses(request);
        }

        public UpdateComponentResponse UpdateComponent(Guid accountId, UpdateComponentRequestData data)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }
            var request = GetRequest<UpdateComponentRequest>(accountId);
            request.Data = data;
            var dispatcher = DispatcherHelper.GetDispatcherService();
            return dispatcher.UpdateComponent(request);
        }

        public GetComponentByIdResponse GetComponentById(Guid accountId, Guid id)
        {
            var request = GetRequest<GetComponentByIdRequest>(accountId);
            request.Data = new GetComponentByIdRequestData()
            {
                ComponentId = id
            };
            var dispatcher = DispatcherHelper.GetDispatcherService();
            return dispatcher.GetComponentById(request);
        }

        public GetComponentAndChildIdsResponse GetComponentAndChildIds(Guid accountId, Guid componentId)
        {
            var request = GetRequest<GetComponentAndChildIdsRequest>(accountId);
            request.Data = new GetComponentAndChildIdsRequestData()
            {
                ComponentId = componentId
            };
            var dispatcher = DispatcherHelper.GetDispatcherService();
            return dispatcher.GetComponentAndChildIds(request);
        }

        #endregion

        #region ComponentTypes

        public GetOrCreateComponentTypeResponse GetOrCreateComponentType(Guid accountId, GetOrCreateComponentTypeRequestData data)
        {
            var request = GetRequest<GetOrCreateComponentTypeRequest>(accountId);
            request.Data = data;
            var dispatcher = DispatcherHelper.GetDispatcherService();
            return dispatcher.GetOrCreateComponentType(request);
        }

        public DeleteComponentTypeResponse DeleteComponentType(Guid accountId, Guid componentTypeId)
        {
            var request = GetRequest<DeleteComponentTypeRequest>(accountId);
            request.Data = new DeleteComponentTypeRequestData()
            {
                ComponentTypeId = componentTypeId
            };
            var dispatcher = DispatcherHelper.GetDispatcherService();
            return dispatcher.DeleteComponentType(request);
        }

        #endregion

        #region События

        public SendEventResponse SendEvent(Guid accountId, SendEventData data)
        {
            var request = GetRequest<SendEventRequest>(accountId);
            request.Data = data;
            var dispatcher = DispatcherHelper.GetDispatcherService();
            return dispatcher.SendEvent(request);
        }

        public GetEventByIdResponse GetEventById(Guid accountId, Guid eventId)
        {
            var data = new GetEventByIdRequestData()
            {
                EventId = eventId
            };
            var request = GetRequest<GetEventByIdRequest>(accountId);
            request.Data = data;
            var dispatcher = DispatcherHelper.GetDispatcherService();
            return dispatcher.GetEventById(request);
        }

        #endregion

        #region Типы событий

        public UpdateEventTypeResponse UpdateEventType(Guid accountId, UpdateEventTypeRequestData data)
        {
            var request = GetRequest<UpdateEventTypeRequest>(accountId);
            request.Data = data;
            var dispatcher = DispatcherHelper.GetDispatcherService();
            return dispatcher.UpdateEventType(request);
        }

        #endregion

        #region Проверки

        public GetOrCreateUnitTestResponse GetOrCreateUnitTest(Guid accountId, GetOrCreateUnitTestRequestData data)
        {
            var request = GetRequest<GetOrCreateUnitTestRequest>(accountId);
            request.Data = data;
            var dispatcher = DispatcherHelper.GetDispatcherService();
            return dispatcher.GetOrCreateUnitTest(request);
        }

        public GetUnitTestTypeByIdResponse GetUnitTestTypeById(Guid accountId, Guid id)
        {
            var request = GetRequest<GetUnitTestTypeByIdRequest>(accountId);
            request.Data = new GetUnitTestTypeByIdRequestData()
            {
                Id = id
            };
            var dispatcher = DispatcherHelper.GetDispatcherService();
            return dispatcher.GetUnitTestTypeById(request);
        }

        public UpdateUnitTestResponse UpdateUnitTest(Guid accountId, UpdateUnitTestRequestData data)
        {
            var request = GetRequest<UpdateUnitTestRequest>(accountId);
            request.Data = data;
            var dispatcher = DispatcherHelper.GetDispatcherService();
            return dispatcher.UpdateUnitTest(request);
        }

        public GetOrCreateUnitTestTypeResponse GetOrCreateUnitTestType(Guid accountId, GetOrCreateUnitTestTypeRequestData data)
        {
            var request = GetRequest<GetOrCreateUnitTestTypeRequest>(accountId);
            request.Data = data;
            var dispatcher = DispatcherHelper.GetDispatcherService();
            return dispatcher.GetOrCreateUnitTestType(request);
        }

        public UpdateUnitTestTypeResponse UpdateUnitTestType(Guid accountId, UpdateUnitTestTypeRequestData data)
        {
            var request = GetRequest<UpdateUnitTestTypeRequest>(accountId);
            request.Data = data;
            var dispatcher = DispatcherHelper.GetDispatcherService();
            return dispatcher.UpdateUnitTestType(request);
        }

        public DeleteUnitTestTypeResponse DeleteUnitTestType(Guid accountId, DeleteUnitTestTypeRequestData data)
        {
            var request = GetRequest<DeleteUnitTestTypeRequest>(accountId);
            request.Data = data;
            var dispatcher = DispatcherHelper.GetDispatcherService();
            return dispatcher.DeleteUnitTestType(request);
        }

        public SetUnitTestNextTimeResponse SetUnitTestNextTime(Guid accountId, SetUnitTestNextTimeRequestData data)
        {
            var request = GetRequest<SetUnitTestNextTimeRequest>(accountId);
            request.Data = data;
            var dispatcher = DispatcherHelper.GetDispatcherService();
            return dispatcher.SetUnitTestNextTime(request);
        }

        public DeleteUnitTestResponse DeleteUnitTest(Guid accountId, Guid unitTestId)
        {
            var request = GetRequest<DeleteUnitTestRequest>(accountId);
            request.Data = new DeleteUnitTestRequestData()
            {
                UnitTestId = unitTestId
            };
            var dispatcher = DispatcherHelper.GetDispatcherService();
            return dispatcher.DeleteUnitTest(request);
        }

        public GetUnitTestStateResponse GetUnitTestState(Guid accountId, Guid componentId, Guid unitTestId)
        {
            var request = GetRequest<GetUnitTestStateRequest>(accountId);
            request.Data = new GetUnitTestStateRequestData()
            {
                UnitTestId = unitTestId
            };
            var dispatcher = DispatcherHelper.GetDispatcherService();
            return dispatcher.GetUnitTestState(request);
        }

        public SendUnitTestResultResponse SendUnitTestResult(
            Guid accountId,
            SendUnitTestResultRequestData data)
        {
            var request = GetRequest<SendUnitTestResultRequest>(accountId);
            request.Data = data;
            var dispatcher = DispatcherHelper.GetDispatcherService();
            return dispatcher.SendUnitTestResult(request);
        }

        public SendHttpUnitTestBannerResponse SendHttpUnitTestBanner(Guid accountId, Guid unitTestId, bool hasBanner)
        {
            var request = GetRequest<SendHttpUnitTestBannerRequest>(accountId);
            request.Data = new SendHttpUnitTestBannerRequestData()
            {
                UnitTestId = unitTestId,
                HasBanner = hasBanner
            };
            var dispatcher = DispatcherHelper.GetDispatcherService();
            return dispatcher.SendHttpUnitTestBanner(request);
        }

        public RecalcUnitTestsResultsResponse RecalcUnitTestsResults(Guid accountId, int maxCount)
        {
            var request = GetRequest<RecalcUnitTestsResultsRequest>(accountId);
            request.Data = new RecalcUnitTestsResultsRequestData
            {
                MaxCount = maxCount
            };
            var dispatcher = DispatcherHelper.GetDispatcherService();
            return dispatcher.RecalcUnitTestsResults(request);
        }

        public SetUnitTestEnableResponse SetUnitTestEnable(Guid accountId, Guid unitTestId)
        {
            var request = GetRequest<SetUnitTestEnableRequest>(accountId);
            request.Data = new SetUnitTestEnableRequestData()
            {
                UnitTestId = unitTestId
            };
            var dispatcher = DispatcherHelper.GetDispatcherService();
            return dispatcher.SetUnitTestEnable(request);
        }

        public SetUnitTestDisableResponse SetUnitTestDisable(Guid accountId, SetUnitTestDisableRequestData data)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }
            var request = GetRequest<SetUnitTestDisableRequest>(accountId);
            request.Data = data;
            var dispatcher = DispatcherHelper.GetDispatcherService();
            return dispatcher.SetUnitTestDisable(request);
        }

        public AddPingUnitTestResponse AddPingUnitTest(Guid accountId, AddPingUnitTestRequestData data)
        {
            var request = GetRequest<AddPingUnitTestRequest>(accountId);
            request.Data = data;
            var dispatcher = DispatcherHelper.GetDispatcherService();
            return dispatcher.AddPingUnitTest(request);
        }

        public AddHttpUnitTestResponse AddHttpUnitTest(Guid accountId, AddHttpUnitTestRequestData data)
        {
            var request = GetRequest<AddHttpUnitTestRequest>(accountId);
            request.Data = data;
            var dispatcher = DispatcherHelper.GetDispatcherService();
            return dispatcher.AddHttpUnitTest(request);
        }

        #endregion

        #region Метрики

        public GetMetricsResponse GetMetrics(Guid accountId, Guid componentId)
        {
            var request = GetRequest<GetMetricsRequest>(accountId);
            request.Data = new GetMetricsRequestData()
            {
                ComponentId = componentId
            };
            var dispatcher = DispatcherHelper.GetDispatcherService();
            return dispatcher.GetMetrics(request);
        }

        public DeleteMetricResponse DeleteMetric(Guid accountId, DeleteMetricRequestData data)
        {
            var request = GetRequest<DeleteMetricRequest>(accountId);
            request.Data = data;
            var dispatcher = DispatcherHelper.GetDispatcherService();
            return dispatcher.DeleteMetric(request);
        }

        public UpdateMetricsResponse CalculateMetrics(Guid accountId, int maxCount)
        {
            var request = GetRequest<UpdateMetricsRequest>(accountId);
            request.Data = new UpdateMetricsRequestData()
            {
                MaxCount = maxCount
            };
            var dispatcher = DispatcherHelper.GetDispatcherService();
            return dispatcher.UpdateMetrics(request);
        }

        public SendMetricResponse SendMetric(Guid accountId, SendMetricRequestData data)
        {
            var request = GetRequest<SendMetricRequest>(accountId);
            request.Data = data;
            var dispatcher = DispatcherHelper.GetDispatcherService();
            return dispatcher.SendMetric(request);
        }

        public SetMetricDisableResponse SetMetricDisable(Guid accountId, SetMetricDisableRequestData data)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }
            var request = GetRequest<SetMetricDisableRequest>(accountId);
            request.Data = data;
            var dispatcher = DispatcherHelper.GetDispatcherService();
            return dispatcher.SetMetricDisable(request);
        }

        public SetMetricEnableResponse SetMetricEnable(Guid accountId, Guid metricId)
        {
            var request = GetRequest<SetMetricEnableRequest>(accountId);
            request.Data = new SetMetricEnableRequestData()
            {
                MetricId = metricId
            };
            var dispatcher = DispatcherHelper.GetDispatcherService();
            return dispatcher.SetMetricEnable(request);
        }

        public UpdateMetricResponse UpdateMetric(Guid accountId, UpdateMetricRequestData data)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }
            var request = GetRequest<UpdateMetricRequest>(accountId);
            request.Data = data;
            var dispatcher = DispatcherHelper.GetDispatcherService();
            return dispatcher.UpdateMetric(request);
        }

        public CreateMetricResponse CreateMetric(Guid accountId, CreateMetricRequestData data)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }
            var request = GetRequest<CreateMetricRequest>(accountId);
            request.Data = data;
            var dispatcher = DispatcherHelper.GetDispatcherService();
            return dispatcher.CreateMetric(request);
        }

        #endregion

        #region MetricTypes

        public CreateMetricTypeResponse CreateMetricType(Guid accountId, CreateMetricTypeRequestData data)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }
            var request = GetRequest<CreateMetricTypeRequest>(accountId);
            request.Data = data;
            var dispatcher = DispatcherHelper.GetDispatcherService();
            return dispatcher.CreateMetricType(request);
        }

        public UpdateMetricTypeResponse UpdateMetricType(Guid accountId, UpdateMetricTypeRequestData data)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }
            var request = GetRequest<UpdateMetricTypeRequest>(accountId);
            request.Data = data;
            var dispatcher = DispatcherHelper.GetDispatcherService();
            return dispatcher.UpdateMetricType(request);
        }

        public DeleteMetricTypeResponse DeleteMetricType(Guid accountId, Guid metricTypeId)
        {
            var request = GetRequest<DeleteMetricTypeRequest>(accountId);
            request.Data = new DeleteMetricTypeRequestData()
            {
                MetricTypeId = metricTypeId
            };
            var dispatcher = DispatcherHelper.GetDispatcherService();
            return dispatcher.DeleteMetricType(request);
        }

        #endregion

        #region Лог

        public SendLogResponse SendLog(Guid accountId, SendLogData data)
        {
            var request = GetRequest<SendLogRequest>(accountId);
            request.Data = data;
            var dispatcher = DispatcherHelper.GetDispatcherService();
            return dispatcher.SendLog(request);
        }

        #endregion

        #region Limits

        public GetAccountLimitsResponse GetAccountLimits(Guid accountId, int archiveDays)
        {
            var request = GetRequest<GetAccountLimitsRequest>(accountId);
            request.Data = new GetAccountLimitsRequestData()
            {
                ArchiveDays = archiveDays
            };
            var dispatcher = DispatcherHelper.GetDispatcherService();
            return dispatcher.GetAccountLimits(request);
        }

        public GetAllAccountsLimitsResponse GetAllAccountsLimits(int archiveDays)
        {
            var request = GetRequest<GetAllAccountsLimitsRequest>(null);
            request.Data = new GetAllAccountsLimitsRequestData()
            {
                ArchiveDays = archiveDays
            };
            var dispatcher = DispatcherHelper.GetDispatcherService();
            return dispatcher.GetAllAccountsLimits(request);
        }

        #endregion

        #region Платежи и баланс

        public AddYandexKassaPaymentResponse AddYandexKassaPayment(Guid accountId, AddYandexKassaPaymentRequestData data)
        {
            var request = GetRequest<AddYandexKassaPaymentRequest>(accountId);
            request.Data = data;
            var dispatcher = DispatcherHelper.GetDispatcherService();
            return dispatcher.AddYandexKassaPayment(request);            
        }

        public ProcessPartnerPaymentsResponse ProcessPartnerPayments(DateTime fromDate)
        {
            var data = new ProcessPartnerPaymentsRequestData()
            {
                FromDate = fromDate
            };
            var request = GetRequest<ProcessPartnerPaymentsRequest>(null);
            request.Data = data;
            var dispatcher = DispatcherHelper.GetDispatcherService();
            return dispatcher.ProcessPartnerPayments(request);
        }

        public MakeAccountPaidResponse MakeAccountPaidAndSetLimits(Guid accountId, TariffConfigurationInfo data)
        {
            var request = GetRequest<MakeAccountPaidRequest>(accountId);
            request.Data = data;
            var dispatcher = DispatcherHelper.GetDispatcherService();
            return dispatcher.MakeAccountPaidAndSetLimits(request);
        }

        #endregion

        #region Подписки

        public Zidium.Core.Api.CreateSubscriptionResponse CreateSubscription(Guid accountId, CreateSubscriptionRequestData data)
        {
            var request = GetRequest<CreateSubscriptionRequest>(accountId);
            request.Data = data;
            var dispatcher = DispatcherHelper.GetDispatcherService();
            return dispatcher.CreateSubscription(request);
        }

        public CreateUserDefaultSubscriptionResponse CreateUserDefaultSubscription(Guid accountId, CreateUserDefaultSubscriptionRequestData data)
        {
            var request = GetRequest<CreateUserDefaultSubscriptionRequest>(accountId);
            request.Data = data;
            var dispatcher = DispatcherHelper.GetDispatcherService();
            return dispatcher.CreateUserDefaultSubscription(request);
        }

        public UpdateSubscriptionResponse UpdateSubscription(Guid accountId, UpdateSubscriptionRequestData data)
        {
            var request = GetRequest<UpdateSubscriptionRequest>(accountId);
            request.Data = data;
            var dispatcher = DispatcherHelper.GetDispatcherService();
            return dispatcher.UpdateSubscription(request);
        }

        public SendSmsResponse SendSms(Guid accountId, SendSmsRequestData data)
        {
            var request = GetRequest<SendSmsRequest>(accountId);
            request.Data = data;
            var dispatcher = DispatcherHelper.GetDispatcherService();
            return dispatcher.SendSms(request);
        }

        #endregion

        #region Регистрация аккаунта

        public AccountRegistrationResponse AccountRegistrationStep1(AccountRegistrationStep1RequestData data)
        {
            var request = GetRequest<AccountRegistrationStep1Request>(null);
            request.Data = data;
            var dispatcher = DispatcherHelper.GetDispatcherService();
            return dispatcher.AccountRegistrationStep1(request);
        }

        public AccountRegistrationResponse AccountRegistrationStep2(AccountRegistrationStep2RequestData data)
        {
            var request = GetRequest<AccountRegistrationStep2Request>(null);
            request.Data = data;
            var dispatcher = DispatcherHelper.GetDispatcherService();
            return dispatcher.AccountRegistrationStep2(request);
        }

        public AccountRegistrationResponse AccountRegistrationStep3(AccountRegistrationStep3RequestData data)
        {
            var request = GetRequest<AccountRegistrationStep3Request>(null);
            request.Data = data;
            var dispatcher = DispatcherHelper.GetDispatcherService();
            return dispatcher.AccountRegistrationStep3(request);
        }

        public EndAccountRegistrationResponse EndAccountRegistration(EndAccountRegistrationRequestData data)
        {
            var request = GetRequest<EndAccountRegistrationRequest>(null);
            request.Data = data;
            var dispatcher = DispatcherHelper.GetDispatcherService();
            return dispatcher.EndAccountRegistration(request);
        }

        #endregion

        #region Базы данных

        /// <summary>
        /// Получение базы по Id
        /// </summary>
        public GetDatabaseByIdResponse GetDatabaseById(GetDatabaseByIdRequestData data)
        {
            var request = GetRequest<GetDatabaseByIdRequest>(null);
            request.Data = data;
            var dispatcher = DispatcherHelper.GetDispatcherService();
            return dispatcher.GetDatabaseById(request);
        }

        /// <summary>
        /// Получение списка всех баз
        /// </summary>
        public GetDatabasesResponse GetDatabases()
        {
            var request = GetRequest<GetDatabasesRequest>(null);
            var dispatcher = DispatcherHelper.GetDispatcherService();
            return dispatcher.GetDatabases(request);
        }

        /// <summary>
        /// Изменение признака неработоспособности базы
        /// </summary>
        public SetDatabaseIsBrokenResponse SetDatabaseIsBroken(SetDatabaseIsBrokenRequestData data)
        {
            var request = GetRequest<SetDatabaseIsBrokenRequest>(null);
            request.Data = data;
            var dispatcher = DispatcherHelper.GetDispatcherService();
            return dispatcher.SetDatabaseIsBroken(request);
        }

        #endregion

        #region Аккаунты

        /// <summary>
        /// Получение аккаунта по Id
        /// </summary>
        public GetAccountByIdResponse GetAccountById(GetAccountByIdRequestData data)
        {
            var request = GetRequest<GetAccountByIdRequest>(null);
            request.Data = data;
            var dispatcher = DispatcherHelper.GetDispatcherService();
            return dispatcher.GetAccountById(request);
        }

        /// <summary>
        /// Получение списка всех аккаунтов
        /// </summary>
        public GetAccountsResponse GetAccounts(GetAccountsRequestData data)
        {
            var request = GetRequest<GetAccountsRequest>(null);
            request.Data = data;
            var dispatcher = DispatcherHelper.GetDispatcherService();
            return dispatcher.GetAccounts(request);
        }

        /// <summary>
        /// Обновление аккаунта
        /// </summary>
        public UpdateAccountResponse UpdateAccount(UpdateAccountRequestData data)
        {
            var request = GetRequest<UpdateAccountRequest>(null);
            request.Data = data;
            var dispatcher = DispatcherHelper.GetDispatcherService();
            return dispatcher.UpdateAccount(request);
        }

        #endregion

    }
}
