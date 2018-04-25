namespace Zidium.Core.Api
{
    public interface IDispatcherService
    {
        #region Разное

        /// <summary>
        /// Используется для тестирования соединения. 
        /// Возвращает строку, которая подавалась на вход.
        /// </summary>
        /// <returns></returns>
        GetEchoResponse GetEcho(GetEchoRequest request);

        /// <summary>
        /// Возвращает текущее время сервера
        /// </summary>
        /// <returns></returns>
        GetServerTimeResponse GetServerTime();

        SaveAllCachesResponse SaveAllCaches(SaveAllCachesRequest request);

        #endregion

        #region Компоненты и папки

        GetRootControlDataResponse GetRootControlData(GetRootControlDataRequest request);

        CreateComponentResponse CreateComponent(CreateComponentRequest request);

        GetOrCreateComponentResponse GetOrCreateComponent(GetOrCreateComponentRequest request);

        GetComponentControlByIdResponse GetComponentControlById(GetComponentControlByIdRequest request);

        GetOrAddComponentResponse GetOrAddComponent(GetOrAddComponentRequest request);

        GetRootComponentResponse GetRootComponent(GetRootComponentRequest request);

        GetComponentByIdResponse GetComponentById(GetComponentByIdRequest request);

        GetComponentBySystemNameResponse GetComponentBySystemName(GetComponentBySystemNameRequest request);

        UpdateComponentResponse UpdateComponent(UpdateComponentRequest request);

        GetChildComponentsResponse GetChildComponents(GetChildComponentsRequest request);

        SetComponentEnableResponse SetComponentEnable(SetComponentEnableRequest request);

        SetComponentDisableResponse SetComponentDisable(SetComponentDisableRequest request);

        DeleteComponentResponse DeleteComponent(DeleteComponentRequest request);

        GetComponentAndChildIdsResponse GetComponentAndChildIds(GetComponentAndChildIdsRequest request);

        #endregion

        #region Типы компонентов

        /// <summary>
        /// Получение типа компонента
        /// </summary>
        /// <returns></returns>
        GetComponentTypeResponse GetComponentType(GetComponentTypeRequest request);

        /// <summary>
        /// Получение или создание типа компонента
        /// </summary>
        /// <returns></returns>
        GetOrCreateComponentTypeResponse GetOrCreateComponentType(GetOrCreateComponentTypeRequest request);

        /// <summary>
        /// Обновление типа компонента
        /// </summary>
        /// <returns></returns>
        UpdateComponentTypeResponse UpdateComponentType(UpdateComponentTypeRequest request);

        DeleteComponentTypeResponse DeleteComponentType(DeleteComponentTypeRequest request);

        #endregion

        #region События

        /// <summary>
        /// Отправка сообщения по компоненту
        /// </summary>
        /// <returns></returns>
        SendEventResponse SendEvent(SendEventRequest request);

        GetEventByIdResponse GetEventById(GetEventByIdRequest request);

        GetEventsResponse GetEvents(GetEventsRequest request);

        /// <summary>
        /// Отправляет данные для склейки событий.
        /// Склеивается сразу пачка событий.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        JoinEventsResponse JoinEvents(JoinEventsRequest request);
        
        #endregion
        
        #region Типы событий

        UpdateEventTypeResponse UpdateEventType(UpdateEventTypeRequest request);

        #endregion

        #region Метрики

        CreateMetricResponse CreateMetric(CreateMetricRequest request);

        SendMetricResponse SendMetric(SendMetricRequest request);

        SendMetricsResponse SendMetrics(SendMetricsRequest request);

        GetMetricResponse GetMetric(GetMetricRequest request);

        GetMetricsResponse GetMetrics(GetMetricsRequest request);

        GetMetricsHistoryResponse GetMetricsHistory(GetMetricsHistoryRequest request);

        DeleteMetricResponse DeleteMetric(DeleteMetricRequest request);

        DeleteMetricTypeResponse DeleteMetricType(DeleteMetricTypeRequest typeRequest);

        UpdateMetricsResponse UpdateMetrics(UpdateMetricsRequest request);

        SetMetricEnableResponse SetMetricEnable(SetMetricEnableRequest request);

        SetMetricDisableResponse SetMetricDisable(SetMetricDisableRequest request);

        UpdateMetricResponse UpdateMetric(UpdateMetricRequest request);

        #endregion

        #region Типы метрик

        UpdateMetricTypeResponse UpdateMetricType(UpdateMetricTypeRequest request);

        CreateMetricTypeResponse CreateMetricType(CreateMetricTypeRequest request);

        #endregion

        #region Лог

        SendLogResponse SendLog(SendLogRequest request);

        GetLogsResponse GetLogs(GetLogsRequest request);

        GetLogConfigResponse GetLogConfig(GetLogConfigRequest request);

        SendLogsResponse SendLogs(SendLogsRequest request);

        GetChangedWebLogConfigsResponse GetChangedWebLogConfigs(GetChangedWebLogConfigsRequest request);

        #endregion

        #region Подписки

        CreateSubscriptionResponse CreateSubscription(CreateSubscriptionRequest request);

        CreateUserDefaultSubscriptionResponse CreateUserDefaultSubscription(CreateUserDefaultSubscriptionRequest request);

        UpdateSubscriptionResponse UpdateSubscription(UpdateSubscriptionRequest request);

        SetSubscriptionDisableResponse SetSubscriptionDisable(SetSubscriptionDisableRequest request);

        SetSubscriptionEnableResponse SetSubscriptionEnable(SetSubscriptionEnableRequest request);

        SendSmsResponse SendSms(SendSmsRequest request);

        #endregion

        #region Регистрация аккаунта

        /// <summary>
        /// Регистрация аккаунта - шаг 1
        /// </summary>
        /// <returns></returns>
        AccountRegistrationResponse AccountRegistrationStep1(AccountRegistrationStep1Request request);

        /// <summary>
        /// Регистрация аккаунта - шаг 2
        /// </summary>
        /// <returns></returns>
        AccountRegistrationResponse AccountRegistrationStep2(AccountRegistrationStep2Request request);

        /// <summary>
        /// Регистрация аккаунта - шаг 3
        /// </summary>
        /// <returns></returns>
        AccountRegistrationResponse AccountRegistrationStep3(AccountRegistrationStep3Request request);

        /// <summary>
        /// Завершает регистрацию аккаунта
        /// </summary>
        /// <returns></returns>
        EndAccountRegistrationResponse EndAccountRegistration(EndAccountRegistrationRequest request);

        #endregion

        #region Проверки

        SetUnitTestNextTimeResponse SetUnitTestNextTime(SetUnitTestNextTimeRequest request);

        GetOrCreateUnitTestTypeResponse GetOrCreateUnitTestType(GetOrCreateUnitTestTypeRequest request);

        UpdateUnitTestTypeResponse UpdateUnitTestType(UpdateUnitTestTypeRequest request);

        DeleteUnitTestTypeResponse DeleteUnitTestType(DeleteUnitTestTypeRequest request);

        GetOrCreateUnitTestResponse GetOrCreateUnitTest(GetOrCreateUnitTestRequest request);

        UpdateUnitTestResponse UpdateUnitTest(UpdateUnitTestRequest request);

        DeleteUnitTestResponse DeleteUnitTest(DeleteUnitTestRequest request);

        SendUnitTestResultResponse SendUnitTestResult(SendUnitTestResultRequest request);

        GetUnitTestStateResponse GetUnitTestState(GetUnitTestStateRequest request);

        SendHttpUnitTestBannerResponse SendHttpUnitTestBanner(SendHttpUnitTestBannerRequest request);

        SetUnitTestEnableResponse SetUnitTestEnable(SetUnitTestEnableRequest request);

        SetUnitTestDisableResponse SetUnitTestDisable(SetUnitTestDisableRequest request);

        AddPingUnitTestResponse AddPingUnitTest(AddPingUnitTestRequest request);

        AddHttpUnitTestResponse AddHttpUnitTest(AddHttpUnitTestRequest request);
        
        RecalcUnitTestsResultsResponse RecalcUnitTestsResults(RecalcUnitTestsResultsRequest request);

        #endregion

        #region Статус компонента

        GetComponentTotalStateResponse GetComponentTotalState(GetComponentTotalStateRequest request);

        GetComponentInternalStateResponse GetComponentInternalState(GetComponentInternalStateRequest request);

        /// <summary>
        /// Заново вычисляет статус компонента
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        UpdateComponentStateResponse UpdateComponentState(UpdateComponentStateRequest request);

        /// <summary>
        /// Расчитывает заново неактуальные статусы событий
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        UpdateEventsStatusesResponse UpdateEventsStatuses(UpdateEventsStatusesRequest request);

        #endregion

        #region Платежи и баланс

        AddYandexKassaPaymentResponse AddYandexKassaPayment(AddYandexKassaPaymentRequest request);

        ProcessPartnerPaymentsResponse ProcessPartnerPayments(ProcessPartnerPaymentsRequest request);

        #endregion

        #region Тарифы и лимиты

        /// <summary>
        /// Возвращает лимиты аккаунта
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        GetAccountLimitsResponse GetAccountLimits(GetAccountLimitsRequest request);

        /// <summary>
        /// Возвращает лимиты всех аккаунтов
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        GetAllAccountsLimitsResponse GetAllAccountsLimits(GetAllAccountsLimitsRequest request);

        /// <summary>
        /// Переводит аккаунт на бесплатный тариф
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        MakeAccountFreeResponse MakeAccountFree(MakeAccountFreeRequest request);

        /// <summary>
        /// Переводит аккаунт на платный тариф и устанавливает лимиты
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        MakeAccountPaidResponse MakeAccountPaidAndSetLimits(MakeAccountPaidRequest request);

        /// <summary>
        /// Прямая установка указанных лимитов
        /// Только для юнит-тестов
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        SetAccountLimitsResponse SetAccountLimits(SetAccountLimitsRequest request);

        #endregion

        #region Аккаунты

        /// <summary>
        /// Получение аккаунта по Id
        /// </summary>
        GetAccountByIdResponse GetAccountById(GetAccountByIdRequest request);

        /// <summary>
        /// Получение списка всех аккаунтов
        /// </summary>
        GetAccountsResponse GetAccounts(GetAccountsRequest request);

        /// <summary>
        /// Обновление аккаунта
        /// </summary>
        UpdateAccountResponse UpdateAccount(UpdateAccountRequest request);

        #endregion

        #region Базы данных

        /// <summary>
        /// Получение базы по Id
        /// </summary>
        GetDatabaseByIdResponse GetDatabaseById(GetDatabaseByIdRequest request);

        /// <summary>
        /// Получение списка всех баз
        /// </summary>
        GetDatabasesResponse GetDatabases(GetDatabasesRequest request);

        /// <summary>
        /// Изменение признака неработоспособности базы
        /// </summary>
        SetDatabaseIsBrokenResponse SetDatabaseIsBroken(SetDatabaseIsBrokenRequest request);

        #endregion
    }
}
