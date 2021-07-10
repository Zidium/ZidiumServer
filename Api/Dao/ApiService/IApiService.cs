using System;
using System.Collections.Generic;
using Zidium.Api.Dto;

namespace Zidium.Api
{
    public interface IApiService
    {
        AccessTokenDto AccessToken { get; }

        void SetAccessToken(AccessTokenDto accessToken);

        bool IsFake { get; }

        #region Разное

        /// <summary>
        /// Метод для проверки соединения.
        /// Возвращает строку, которая была отправлена в запросе (эхо).
        /// </summary>
        /// <returns></returns>
        GetEchoResponseDto GetEcho(string message);

        /// <summary>
        /// Получает время сервера АПП
        /// </summary>
        /// <returns></returns>
        GetServerTimeResponseDto GetServerTime();

        #endregion

        #region Контролы компонентов

        GetRootControlDataResponseDto GetRootControlData();

        GetComponentControlByIdResponseDto GetComponentControlById(Guid componentId);

        /// <summary>
        /// Возвращает или создает компонент
        /// </summary>
        /// <returns></returns>
        GetOrCreateComponentResponseDto GetOrCreateComponent(GetOrCreateComponentRequestDataDto data);

        #endregion

        #region Компоненты

        /// <summary>
        /// Возвращает корень дерева компонентов аккаунта
        /// </summary>
        /// <returns></returns>
        GetRootComponentResponseDto GetRootComponent();

        /// <summary>
        /// Возвращает информацию по компоненту по его ИД
        /// </summary>
        /// <param name="componentId"></param>
        /// <returns></returns>
        GetComponentByIdResponseDto GetComponentById(Guid componentId);

        GetComponentBySystemNameResponseDto GetComponentBySystemName(Guid? parentId, string systemName);

        /// <summary>
        /// Возвращает информацию о компоненте по его системному имени ИЛИ по ИД
        /// </summary>;
        /// <returns></returns>
        GetComponentBySystemNameResponseDto GetComponentBySystemName(GetComponentBySystemNameRequestDataDto data);

        /// <summary>
        /// Возвращает список дочерних компонентов
        /// </summary>
        /// <returns></returns>
        GetChildComponentsResponseDto GetChildComponents(Guid componentId);

        /// <summary>
        /// Обновляет компонент
        /// </summary>
        /// <returns></returns>
        UpdateComponentResponseDto UpdateComponent(UpdateComponentRequestDataDto data);

        /// <summary>
        /// Получает текущий статус компонента
        /// </summary>
        /// <param name="componentId"></param>
        /// <param name="recalc"></param>
        /// <returns></returns>
        GetComponentTotalStateResponseDto GetComponentTotalState(Guid componentId, bool recalc);

        GetComponentInternalStateResponseDto GetComponentInternalState(Guid componentId, bool recalc);

        SetComponentEnableResponseDto SetComponentEnable(Guid componentId);

        SetComponentDisableResponseDto SetComponentDisable(Guid componentId, DateTime? toDate, string comment);

        DeleteComponentResponseDto DeleteComponent(Guid componentId);

        #endregion

        #region Типы компонентов

        /// <summary>
        /// Получение типа компонента по системному имени
        /// </summary>
        /// <returns></returns>
        GetComponentTypeResponseDto GetComponentType(GetComponentTypeRequestDataDto data);

        /// <summary>
        /// Получение типа компонента по системному имени или создание
        /// </summary>
        /// <returns></returns>
        GetOrCreateComponentTypeResponseDto GetOrCreateComponentType(GetOrCreateComponentTypeRequestDataDto data);

        /// <summary>
        /// Обновление типа компонента по системному имени
        /// </summary>
        /// <returns></returns>
        UpdateComponentTypeResponseDto UpdateComponentType(UpdateComponentTypeRequestDataDto data);

        #endregion

        #region События

        /// <summary>
        /// Отправка информации о событии
        /// </summary>
        /// <returns></returns>
        SendEventResponseDto SendEvent(SendEventRequestDataDto data);

        GetEventByIdResponseDto GetEventById(Guid eventId);

        /// <summary>
        /// Получение событий компонента по критериям
        /// </summary>
        /// <returns></returns>
        GetEventsResponseDto GetEvents(GetEventsRequestDataDto data);

        /// <summary>
        /// Отправляет данные для склейки событий.
        /// Склеивается сразу пачка событий.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        JoinEventsResponseDto JoinEvents(List<JoinEventRequestDataDto> data);

        #endregion

        #region Проверки

        GetOrCreateUnitTestResponseDto GetOrCreateUnitTest(GetOrCreateUnitTestRequestDataDto data);

        GetOrCreateUnitTestTypeResponseDto GetOrCreateUnitTestType(GetOrCreateUnitTestTypeRequestDataDto data);

        SendUnitTestResultResponseDto SendUnitTestResult(SendUnitTestResultRequestDataDto data);

        SendUnitTestResultsResponseDto SendUnitTestResults(SendUnitTestResultRequestDataDto[] data);

        GetUnitTestStateResponseDto GetUnitTestState(Guid unitTestId);

        SetUnitTestEnableResponseDto SetUnitTestEnable(Guid unitTestId);

        SetUnitTestDisableResponseDto SetUnitTestDisable(SetUnitTestDisableRequestDataDto data);

        #endregion

        #region Метрики

        SendMetricResponseDto SendMetric(SendMetricRequestDataDto data);

        SendMetricsResponseDto SendMetrics(List<SendMetricRequestDataDto> data);

        GetMetricsHistoryResponseDto GetMetricsHistory(GetMetricsHistoryRequestDataDto filter);

        GetMetricsResponseDto GetMetrics(Guid componentId);

        GetMetricResponseDto GetMetric(Guid componentId, string metricName);

        #endregion

        #region Лог

        SendLogResponseDto SendLog(SendLogRequestDataDto data);

        SendLogsResponseDto SendLogs(SendLogRequestDataDto[] data);

        GetLogsResponseDto GetLogs(GetLogsRequestDataDto filter);

        GetLogConfigResponseDto GetLogConfig(Guid componentId);

        GetChangedWebLogConfigsResponseDto GetChangedWebLogConfigs(DateTime lastUpdateDate, Guid[] componentIds);

        #endregion
    }
}
