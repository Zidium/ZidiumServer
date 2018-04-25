using System;
using System.Collections.Generic;

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
        ComponentInfo Info { get; }

        WebLogConfig WebLogConfig { get; }

        #endregion Разное

        #region Компоненты и папки
        
        GetComponentByIdResponse GetParent();

        GetChildComponentsResponse GetChildComponents();

        IComponentControl GetOrCreateChildComponentControl(string typeSystemName, string systemName);

        IComponentControl GetOrCreateChildComponentControl(GetOrCreateComponentData data);

        IComponentControl GetOrCreateChildComponentControl(IComponentTypeControl type, string systemName);

        IComponentControl GetOrCreateChildComponentControl(IComponentTypeControl type, string systemName, string version);

        UpdateComponentResponse Update(UpdateComponentData data);
        
        IComponentControl GetOrCreateChildFolderControl(string systemName);

        IComponentControl GetOrCreateChildFolderControl(GetOrCreateFolderData data);

        SetComponentEnableResponse Enable();

        SetComponentDisableResponse Disable(string comment);

        SetComponentDisableResponse Disable(string comment, DateTime date);

        DeleteComponentResponse Delete();

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

        
        SendEventResponse SendApplicationError(string errorTypeSystemName);

        SendEventResponse SendApplicationError(string errorTypeSystemName, Exception exception);

        SendEventResponse SendApplicationError(Exception exception);

        #endregion
        
        #region ComponentEvent

        ComponentEventData CreateComponentEvent(string typeSystemName);

        ComponentEventData CreateComponentEvent(string typeSystemName, string message);

        AddEventResult AddComponentEvent(string typeSystemName);

        AddEventResult AddComponentEvent(string typeSystemName, string message);

        SendEventResponse SendComponentEvent(string typeSystemName);

        SendEventResponse SendComponentEvent(string typeSystemName, string message);

        #endregion

        #region Юнит-тесты

        GetOrCreateUnitTestResponse GetOrCreateUnitTest(string systemName, string displayName);

        GetOrCreateUnitTestResponse GetOrCreateUnitTest(IUnitTestTypeControl unitTestTypeControl, string systemName);

        GetOrCreateUnitTestResponse GetOrCreateUnitTest(GetOrCreateUnitTestData data);

        // контролы

        IUnitTestControl GetOrCreateUnitTestControl(string systemName);

        IUnitTestControl GetOrCreateUnitTestControl(string systemName, string displayName);

        IUnitTestControl GetOrCreateUnitTestControl(IUnitTestTypeControl unitTestTypeControl, string systemName);

        IUnitTestControl GetOrCreateUnitTestControl(GetOrCreateUnitTestData data);
        
        #endregion

        #region Метрики

        GetMetricResponse GetMetric(string name);

        GetMetricsResponse GetMetrics();

        GetMetricsHistoryResponse GetMetricsHistory(GetMetricsHistoryFilter filter);

        SendMetricsResponse SendMetrics(List<SendMetricData> data);

        SendMetricResponse SendMetric(SendMetricData data);

        SendMetricResponse SendMetric(string name, double? value, TimeSpan actualInterval);

        SendMetricResponse SendMetric(string name, double? value);

        #endregion

        #region Лог

        ILog Log { get; }

        GetLogsResponse GetLogs(GetLogsFilter filter);
        
        GetLogConfigResponse GetWebLogConfig();

        #endregion

        #region Статус компонента

        GetComponentTotalStateResponse GetTotalState(bool recalc);

        GetComponentInternalStateResponse GetInternalState(bool recalc);

        #endregion
    }
}
