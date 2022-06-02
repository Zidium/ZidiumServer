using Zidium.Api.Dto;

namespace Zidium.Core.Api
{
    public interface IDispatcherService : IDtoService
    {
        #region Разное

        SaveAllCachesResponse SaveAllCaches(SaveAllCachesRequest request);

        GetLogicSettingsResponse GetLogicSettings(GetLogicSettingsRequest request);

        #endregion

        #region Компоненты и папки

        CreateComponentResponse CreateComponent(CreateComponentRequest request);

        GetComponentAndChildIdsResponse GetComponentAndChildIds(GetComponentAndChildIdsRequest request);

        #endregion

        #region Типы компонентов

        DeleteComponentTypeResponse DeleteComponentType(DeleteComponentTypeRequest request);

        #endregion

        #region Типы событий

        UpdateEventTypeResponse UpdateEventType(UpdateEventTypeRequest request);

        #endregion

        #region Метрики

        CreateMetricResponse CreateMetric(CreateMetricRequest request);

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

        #region Подписки

        CreateSubscriptionResponse CreateSubscription(CreateSubscriptionRequest request);

        CreateUserDefaultSubscriptionResponse CreateUserDefaultSubscription(CreateUserDefaultSubscriptionRequest request);

        UpdateSubscriptionResponse UpdateSubscription(UpdateSubscriptionRequest request);

        SetSubscriptionDisableResponse SetSubscriptionDisable(SetSubscriptionDisableRequest request);

        SetSubscriptionEnableResponse SetSubscriptionEnable(SetSubscriptionEnableRequest request);

        DeleteSubscriptionResponse DeleteSubscription(DeleteSubscriptionRequest request);

        SendSmsResponse SendSms(SendSmsRequest request);

        #endregion

        #region Проверки

        SetUnitTestNextTimeResponse SetUnitTestNextTime(SetUnitTestNextTimeRequest request);

        SetUnitTestNextStepProcessTimeResponse SetUnitTestNextStepProcessTime(SetUnitTestNextStepProcessTimeRequest request);

        GetUnitTestTypeByIdResponse GetUnitTestTypeById(GetUnitTestTypeByIdRequest request);

        UpdateUnitTestTypeResponse UpdateUnitTestType(UpdateUnitTestTypeRequest request);

        DeleteUnitTestTypeResponse DeleteUnitTestType(DeleteUnitTestTypeRequest request);

        UpdateUnitTestResponse UpdateUnitTest(UpdateUnitTestRequest request);

        DeleteUnitTestResponse DeleteUnitTest(DeleteUnitTestRequest request);

        AddPingUnitTestResponse AddPingUnitTest(AddPingUnitTestRequest request);

        AddHttpUnitTestResponse AddHttpUnitTest(AddHttpUnitTestRequest request);

        RecalcUnitTestsResultsResponse RecalcUnitTestsResults(RecalcUnitTestsResultsRequest request);

        #endregion

        #region Статус компонента

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

        #region Лимиты

        /// <summary>
        /// Возвращает лимиты аккаунта
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        GetAccountLimitsResponse GetAccountLimits(GetAccountLimitsRequest request);

        #endregion

        #region Api keys

        GetApiKeysResponse GetApiKeys(RequestDto request);

        ResponseDto AddApiKey(AddApiKeyRequest request);

        ResponseDto UpdateApiKey(UpdateApiKeyRequest request);

        ResponseDto DeleteApiKey(DeleteApiKeyRequest request);

        GetApiKeyByIdResponse GetApiKeyById(GetApiKeyByIdRequest request);

        #endregion

    }
}
