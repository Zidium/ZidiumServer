namespace Zidium.Api.Dto
{
    /// <summary>
    /// Интерфейс веб-сервиса системы мониторинга
    /// </summary>
    public interface IDtoService
    {
        #region Разное

        /// <summary>
        /// Метод для проверки соединения.
        /// Проверка авторизационной информации.
        /// Получение строки, которая была отправлена в запросе (эхо).
        /// </summary>
        /// <returns></returns>
        GetEchoResponseDto GetEcho(GetEchoRequestDto requestDto);

        /// <summary>
        /// Получение даты и времени на стороне сервиса
        /// </summary>
        /// <returns></returns>
        GetServerTimeResponseDto GetServerTime();

        #endregion

        #region Контролы компонентов

        /// <summary>
        /// Получение контрола корневого компонента (устаревший метод)
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        GetRootControlDataResponseDto GetRootControlData(GetRootControlDataRequestDto request);

        /// <summary>
        /// Получает контрол компонента по его ИД
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        GetComponentControlByIdResponseDto GetComponentControlById(GetComponentControlByIdRequestDto request);

        /// <summary>
        /// Получение или создание контрола компонент (устаревший метод)
        /// </summary>
        /// <returns></returns>
        GetOrCreateComponentResponseDto GetOrCreateComponent(GetOrCreateComponentRequestDto request);

        #endregion

        #region Компоненты

        /// <summary>
        /// Получение корня дерева компонентов аккаунта
        /// </summary>
        /// <returns></returns>
        GetRootComponentResponseDto GetRootComponent(GetRootComponentRequestDto request);

        /// <summary>
        /// Получение или создание компонента
        /// </summary>
        /// <returns></returns>
        GetOrAddComponentResponseDto GetOrAddComponent(GetOrAddComponentRequestDto request);

        /// <summary>
        /// Получение информации о компоненте по его Id
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        GetComponentByIdResponseDto GetComponentById(GetComponentByIdRequestDto request);

        /// <summary>
        /// Получение информации о компоненте по его системному имени ИЛИ по Id
        /// </summary>;
        /// <returns></returns>
        GetComponentBySystemNameResponseDto GetComponentBySystemName(GetComponentBySystemNameRequestDto request);

        /// <summary>
        /// Получение списка дочерних компонентов
        /// </summary>
        /// <returns></returns>
        GetChildComponentsResponseDto GetChildComponents(GetChildComponentsRequestDto request);

        /// <summary>
        /// Обновление компонента
        /// </summary>
        /// <returns></returns>
        UpdateComponentResponseDto UpdateComponent(UpdateComponentRequestDto request);

        /// <summary>
        /// Получение итогового состояния компонента
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        GetComponentTotalStateResponseDto GetComponentTotalState(GetComponentTotalStateRequestDto request);

        /// <summary>
        /// Получение внутреннего состояния компонента
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        GetComponentInternalStateResponseDto GetComponentInternalState(GetComponentInternalStateRequestDto request);

        /// <summary>
        /// Включение компонента
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        SetComponentEnableResponseDto SetComponentEnable(SetComponentEnableRequestDto request);

        /// <summary>
        /// Отключение компонента
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        SetComponentDisableResponseDto SetComponentDisable(SetComponentDisableRequestDto request);

        /// <summary>
        /// Удаление компонента
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        DeleteComponentResponseDto DeleteComponent(DeleteComponentRequestDto request);

        #endregion

        #region Типы компонентов

        /// <summary>
        /// Получение типа компонента по системному имени
        /// </summary>
        /// <returns></returns>
        GetComponentTypeResponseDto GetComponentType(GetComponentTypeRequestDto request);

        /// <summary>
        /// Получение типа компонента по системному имени или создание
        /// </summary>
        /// <returns></returns>
        GetOrCreateComponentTypeResponseDto GetOrCreateComponentType(GetOrCreateComponentTypeRequestDto request);

        /// <summary>
        /// Обновление типа компонента по системному имени
        /// </summary>
        /// <returns></returns>
        UpdateComponentTypeResponseDto UpdateComponentType(UpdateComponentTypeRequestDto request);

        #endregion

        #region События

        /// <summary>
        /// Отправка информации о событии
        /// </summary>
        /// <returns></returns>
        SendEventResponseDto SendEvent(SendEventRequestDto request);

        /// <summary>
        /// Получение информации о событии по Id
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        GetEventByIdResponseDto GetEventById(GetEventByIdRequestDto request);

        /// <summary>
        /// Получение событий компонента по критериям
        /// </summary>
        /// <returns></returns>
        GetEventsResponseDto GetEvents(GetEventsRequestDto request);

        /// <summary>
        /// Отправка данных для склейки событий.
        /// Склеивается сразу пачка событий.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        JoinEventsResponseDto JoinEvents(JoinEventsRequestDto request);

        #endregion

        #region Проверки

        /// <summary>
        /// Получение проверки по системному имени или создание
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        GetOrCreateUnitTestResponseDto GetOrCreateUnitTest(GetOrCreateUnitTestRequestDto request);

        /// <summary>
        /// Получение типа проверки по системному имени или создание
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        GetOrCreateUnitTestTypeResponseDto GetOrCreateUnitTestType(GetOrCreateUnitTestTypeRequestDto request);

        /// <summary>
        /// Отправка результата проверки
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        SendUnitTestResultResponseDto SendUnitTestResult(SendUnitTestResultRequestDto request);

        /// <summary>
        /// Отправка набора результатов проверок
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        SendUnitTestResultsResponseDto SendUnitTestResults(SendUnitTestResultsRequestDto request);

        /// <summary>
        /// Получение состояния проверки
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        GetUnitTestStateResponseDto GetUnitTestState(GetUnitTestStateRequestDto request);

        /// <summary>
        /// Включение проверки
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        SetUnitTestEnableResponseDto SetUnitTestEnable(SetUnitTestEnableRequestDto request);

        /// <summary>
        /// Отключение проверки
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        SetUnitTestDisableResponseDto SetUnitTestDisable(SetUnitTestDisableRequestDto request);

        #endregion

        #region Метрики

        /// <summary>
        /// Отправка одной метрики
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        SendMetricResponseDto SendMetric(SendMetricRequestDto request);

        /// <summary>
        /// Отправка набора метрик
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        SendMetricsResponseDto SendMetrics(SendMetricsRequestDto request);

        /// <summary>
        /// Получение истории значений метрик
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        GetMetricsHistoryResponseDto GetMetricsHistory(GetMetricsHistoryRequestDto request);

        /// <summary>
        /// Получение метрики по названию типа
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        GetMetricResponseDto GetMetric(GetMetricRequestDto request);

        /// <summary>
        /// Получение списка метрик компонента
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        GetMetricsResponseDto GetMetrics(GetMetricsRequestDto request);

        #endregion

        #region Лог

        /// <summary>
        /// Отправка одной записи лога
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        SendLogResponseDto SendLog(SendLogRequestDto request);

        /// <summary>
        /// Отправка набора записей лога
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        SendLogsResponseDto SendLogs(SendLogsRequestDto request);

        /// <summary>
        /// Получение записей лога по критериям
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        GetLogsResponseDto GetLogs(GetLogsRequestDto request);

        /// <summary>
        /// Получение настроек лога по компоненту из личного кабинета
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        GetLogConfigResponseDto GetLogConfig(GetLogConfigRequestDto request);

        /// <summary>
        /// Получение всех изменившихся настроек лога из личного кабинета
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        GetChangedWebLogConfigsResponseDto GetChangedWebLogConfigs(GetChangedWebLogConfigsRequestDto request);

        #endregion
    }
}
