using System;
using System.Collections.Generic;
using Zidium.Api.Dto;

namespace Zidium.Api
{
    /// <summary>
    /// Интерфейс для отправки данных от имени компонента.
    /// Это обертка над IApiService, которая в вызовы IApiService подставляет ИД текущего компонента
    /// </summary>
    public interface IComponentControl : IObjectControl, IDisposable
    {
        #region Разное

        IComponentTypeControl Type { get; }

        bool IsRoot { get; }

        bool IsFolder { get; }

        string Version { get; }

        /// <summary>
        /// Информация о компоненте
        /// </summary>
        ComponentDto Info { get; }

        WebLogConfig WebLogConfig { get; }

        #endregion Разное

        #region Компоненты и папки

        GetComponentByIdResponseDto GetParent();

        GetChildComponentsResponseDto GetChildComponents();

        IComponentControl GetOrCreateChildComponentControl(string typeSystemName, string systemName);

        IComponentControl GetOrCreateChildComponentControl(GetOrCreateComponentData data);

        IComponentControl GetOrCreateChildComponentControl(IComponentTypeControl type, string systemName);

        IComponentControl GetOrCreateChildComponentControl(IComponentTypeControl type, string systemName, string version);

        UpdateComponentResponseDto Update(UpdateComponentData data);

        IComponentControl GetOrCreateChildFolderControl(string systemName);

        IComponentControl GetOrCreateChildFolderControl(GetOrCreateFolderData data);

        SetComponentEnableResponseDto Enable();

        SetComponentDisableResponseDto Disable(string comment);

        SetComponentDisableResponseDto Disable(string comment, DateTime date);

        DeleteComponentResponseDto Delete();

        #endregion

        #region  ApplicationError

        ApplicationErrorData CreateApplicationError(string errorTypeSystemName);

        ApplicationErrorData CreateApplicationError(string errorTypeSystemName, string message);

        ApplicationErrorData CreateApplicationError(string errorTypeSystemName, Exception exception);

        ApplicationErrorData CreateApplicationError(Exception exception);


        AddEventResult AddApplicationError(string errorTypeSystemName);

        AddEventResult AddApplicationError(string errorTypeSystemName, Exception exception);

        AddEventResult AddApplicationError(string errorTypeSystemName, string message);

        AddEventResult AddApplicationError(Exception exception);


        SendEventResponseDto SendApplicationError(string errorTypeSystemName);

        SendEventResponseDto SendApplicationError(string errorTypeSystemName, Exception exception);

        SendEventResponseDto SendApplicationError(Exception exception);

        #endregion

        #region ComponentEvent

        ComponentEventData CreateComponentEvent(string typeSystemName);

        ComponentEventData CreateComponentEvent(string typeSystemName, string message);

        AddEventResult AddComponentEvent(string typeSystemName);

        AddEventResult AddComponentEvent(string typeSystemName, string message);

        SendEventResponseDto SendComponentEvent(string typeSystemName);

        SendEventResponseDto SendComponentEvent(string typeSystemName, string message);

        #endregion

        #region Проверки

        GetOrCreateUnitTestResponseDto GetOrCreateUnitTest(string systemName, string displayName);

        GetOrCreateUnitTestResponseDto GetOrCreateUnitTest(IUnitTestTypeControl unitTestTypeControl, string systemName);

        GetOrCreateUnitTestResponseDto GetOrCreateUnitTest(GetOrCreateUnitTestData data);        

        // контролы

        IUnitTestControl GetOrCreateUnitTestControl(string systemName);

        IUnitTestControl GetOrCreateUnitTestControl(string systemName, string displayName);

        IUnitTestControl GetOrCreateUnitTestControl(IUnitTestTypeControl unitTestTypeControl, string systemName);

        IUnitTestControl GetOrCreateUnitTestControl(GetOrCreateUnitTestData data);

        #endregion

        #region Метрики

        GetMetricResponseDto GetMetric(string name);

        GetMetricsResponseDto GetMetrics();

        GetMetricsHistoryResponseDto GetMetricsHistory(GetMetricsHistoryFilter filter);

        SendMetricsResponseDto SendMetrics(List<SendMetricData> data);

        SendMetricResponseDto SendMetric(SendMetricData data);

        SendMetricResponseDto SendMetric(string name, double? value, TimeSpan actualInterval);

        SendMetricResponseDto SendMetric(string name, double? value);

        #endregion

        #region Лог

        ILog Log { get; }

        GetLogsResponseDto GetLogs(GetLogsFilter filter);

        GetLogConfigResponseDto GetWebLogConfig();

        #endregion

        #region Статус компонента

        GetComponentTotalStateResponseDto GetTotalState(bool recalc);

        GetComponentInternalStateResponseDto GetInternalState(bool recalc);

        #endregion
    }
}
