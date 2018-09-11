using System;
using System.Collections.Generic;

namespace Zidium.Api
{
    public interface IApiService
    {
        AccessToken AccessToken { get; }

        void SetAccessToken(AccessToken accessToken);

        #region Разное

        bool IsFake { get; }

        /// <summary>
        /// Метод для проверки соединения.
        /// Возвращает строку, которая была отправлена в запросе (эхо).
        /// </summary>
        /// <returns></returns>
        GetEchoResponse GetEcho(string message);

        /// <summary>
        /// Получает время сервера АПП
        /// </summary>
        /// <returns></returns>
        GetServerTimeResponse GetServerTime();

        #endregion

        #region Контролы компонентов

        GetRootControlDataResponse GetRootControlData();

        GetComponentControlByIdResponse GetComponentControlById(Guid componentId);

        /// <summary>
        /// Возвращает или создает компонент
        /// </summary>
        /// <returns></returns>
        GetOrCreateComponentResponse GetOrCreateComponent(Guid parentId, GetOrCreateComponentData data);

        #endregion

        #region Компоненты

        /// <summary>
        /// Возвращает корень дерева компонентов аккаунта
        /// </summary>
        /// <returns></returns>
        GetRootComponentResponse GetRootComponent();

        /// <summary>
        /// Возвращает информацию по компоненту по его ИД
        /// </summary>
        /// <param name="componentId"></param>
        /// <returns></returns>
        GetComponentByIdResponse GetComponentById(Guid componentId);

        GetComponentBySystemNameResponse GetComponentBySystemName(string systemName);

        /// <summary>
        /// Возвращает информацию о компоненте по его системному имени ИЛИ по ИД
        /// </summary>;
        /// <returns></returns>
        GetComponentBySystemNameResponse GetComponentBySystemName(GetComponentBySystemNameData data);

        /// <summary>
        /// Возвращает список дочерних компонентов
        /// </summary>
        /// <returns></returns>
        GetChildComponentsResponse GetChildComponents(Guid componentId);

        /// <summary>
        /// Обновляет компонент
        /// </summary>
        /// <returns></returns>
        UpdateComponentResponse UpdateComponent(Guid componentId, UpdateComponentData data);

        /// <summary>
        /// Получает текущий статус компонента
        /// </summary>
        /// <param name="componentId"></param>
        /// <param name="recalc"></param>
        /// <returns></returns>
        GetComponentTotalStateResponse GetComponentTotalState(Guid componentId, bool recalc);

        GetComponentInternalStateResponse GetComponentInternalState(Guid componentId, bool recalc);

        SetComponentEnableResponse SetComponentEnable(Guid componentId);

        SetComponentDisableResponse SetComponentDisable(Guid componentId, DateTime? toDate, string comment);

        DeleteComponentResponse DeleteComponent(Guid componentId);

        #endregion

        #region Типы компонентов

        /// <summary>
        /// Получение типа компонента по системному имени
        /// </summary>
        /// <returns></returns>
        GetComponentTypeResponse GetComponentType(GetComponentTypeData data);

        /// <summary>
        /// Получение типа компонента по системному имени или создание
        /// </summary>
        /// <returns></returns>
        GetOrCreateComponentTypeResponse GetOrCreateComponentType(GetOrCreateComponentTypeData data);

        /// <summary>
        /// Обновление типа компонента по системному имени
        /// </summary>
        /// <returns></returns>
        UpdateComponentTypeResponse UpdateComponentType(UpdateComponentTypeData data);

        #endregion

        #region События

        /// <summary>
        /// Отправка информации о событии
        /// </summary>
        /// <returns></returns>
        SendEventResponse SendEvent(SendEventData data);

        GetEventByIdResponse GetEventById(Guid eventId);

        /// <summary>
        /// Получение событий компонента по критериям
        /// </summary>
        /// <returns></returns>
        GetEventsResponse GetEvents(GetEventsData data);

        /// <summary>
        /// Отправляет данные для склейки событий.
        /// Склеивается сразу пачка событий.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        JoinEventsResponse JoinEvents(List<JoinEventData> data);

        #endregion

        #region Проверки

        GetOrCreateUnitTestResponse GetOrCreateUnitTest(Guid componentId, GetOrCreateUnitTestData data);

        GetOrCreateUnitTestTypeResponse GetOrCreateUnitTestType(GetOrCreateUnitTestTypeData data);

        SendUnitTestResultResponse SendUnitTestResult(Guid unitTestId, SendUnitTestResultData data);

        SendUnitTestResultsResponse SendUnitTestResults(SendUnitTestResultsData[] data);

        GetUnitTestStateResponse GetUnitTestState(Guid unitTestId);

        SetUnitTestEnableResponse SetUnitTestEnable(Guid unitTestId);

        SetUnitTestDisableResponse SetUnitTestDisable(Guid unitTestId, SetUnitTestDisableRequestData data);

        #endregion

        #region Метрики

        SendMetricResponse SendMetric(Guid componentId, SendMetricData data);

        SendMetricsResponse SendMetrics(Guid componentId, List<SendMetricData> data);

        GetMetricsHistoryResponse GetMetricsHistory(Guid componentId, GetMetricsHistoryFilter filter);

        GetMetricsResponse GetMetrics(Guid componentId);

        GetMetricResponse GetMetric(Guid componentId, string metricName);

        #endregion

        #region Лог

        SendLogResponse SendLog(SendLogData data);

        SendLogsResponse SendLogs(SendLogData[] data);

        GetLogsResponse GetLogs(Guid componentId, GetLogsFilter filter);

        GetLogConfigResponse GetLogConfig(Guid componentId);

        GetChangedWebLogConfigsResponse GetChangedWebLogConfigs(DateTime lastUpdateDate, List<Guid> componentIds);

        #endregion
    }
}
