using System;
using System.Collections.Generic;
using Zidium.Api.Dto;

namespace Zidium.Api
{
    /// <summary>
    /// Заглушка для контрола компонента.
    /// Используется на случай, если система мониторинга не используется приложением (или не удалось пройти инициализацию всех ).
    /// </summary>
    public class ComponentControlOffline : ComponentControlBase
    {
        public ComponentControlOffline(
            Client client,
            IComponentTypeControl type,
            string systemName,
            string version)
            : base(client)
        {
            SystemNameInternal = systemName;
            VersionInternal = version;
            Type = type;
        }

        public override string SystemName
        {
            get { return SystemNameInternal; }
        }

        protected string SystemNameInternal { get; set; }

        public override string Version
        {
            get { return VersionInternal; }
        }

        protected string VersionInternal { get; set; }

        public bool IsRootValue { get; set; }

        public override bool IsRoot
        {
            get { return IsRootValue; }
        }

        public bool IsFolderValue { get; set; }

        public override bool IsFolder
        {
            get { return IsFolderValue; }
        }

        public override ComponentDto Info
        {
            get { return null; }
        }

        public override GetComponentByIdResponseDto GetParent()
        {
            return ResponseHelper.GetOfflineResponse<GetComponentByIdResponseDto>();
        }

        public override GetChildComponentsResponseDto GetChildComponents()
        {
            return ResponseHelper.GetOfflineResponse<GetChildComponentsResponseDto>();
        }

        public override UpdateComponentResponseDto Update(UpdateComponentData data)
        {
            return ResponseHelper.GetOfflineResponse<UpdateComponentResponseDto>();
        }

        public override DeleteComponentResponseDto Delete()
        {
            return ResponseHelper.GetOfflineResponse<DeleteComponentResponseDto>();
        }

        public override GetOrCreateUnitTestResponseDto GetOrCreateUnitTest(GetOrCreateUnitTestData data)
        {
            return ResponseHelper.GetOfflineResponse<GetOrCreateUnitTestResponseDto>();
        }

        public override GetLogsResponseDto GetLogs(GetLogsFilter filter)
        {
            return ResponseHelper.GetOfflineResponse<GetLogsResponseDto>();
        }

        public override GetLogConfigResponseDto GetWebLogConfig()
        {
            return ResponseHelper.GetOfflineResponse<GetLogConfigResponseDto>();
        }

        public override GetComponentTotalStateResponseDto GetTotalState(bool recalc)
        {
            return ResponseHelper.GetOfflineResponse<GetComponentTotalStateResponseDto>();
        }

        public override GetComponentInternalStateResponseDto GetInternalState(bool recalc)
        {
            return ResponseHelper.GetOfflineResponse<GetComponentInternalStateResponseDto>();
        }

        public override void Dispose()
        {

        }

        public override WebLogConfig WebLogConfig
        {
            get
            {
                return new WebLogConfig()
                {
                    Enabled = false,
                    IsDebugEnabled = false,
                    IsErrorEnabled = false,
                    IsFatalEnabled = false,
                    IsInfoEnabled = false,
                    IsTraceEnabled = false,
                    IsWarningEnabled = false,
                    LastUpdateDate = DateTime.MinValue
                };
            }
        }

        public override bool IsFake()
        {
            return true;
        }

        public override GetMetricsResponseDto GetMetrics()
        {
            return ResponseHelper.GetOfflineResponse<GetMetricsResponseDto>();
        }

        public override GetMetricsHistoryResponseDto GetMetricsHistory(GetMetricsHistoryFilter filter)
        {
            return ResponseHelper.GetOfflineResponse<GetMetricsHistoryResponseDto>();
        }

        public override SendMetricsResponseDto SendMetrics(List<SendMetricData> data)
        {
            return ResponseHelper.GetOfflineResponse<SendMetricsResponseDto>();
        }

        public override SendMetricResponseDto SendMetric(SendMetricData data)
        {
            return ResponseHelper.GetOfflineResponse<SendMetricResponseDto>();
        }

        public override GetMetricResponseDto GetMetric(string name)
        {
            return ResponseHelper.GetOfflineResponse<GetMetricResponseDto>();
        }

        internal override IComponentControl GetOrCreateChildComponentControl(GetOrCreateComponentControlData data)
        {
            // данный метод вызывается только у wrapper-а
            throw new NotImplementedException();
        }

        public override IComponentControl GetOrCreateChildComponentControl(GetOrCreateComponentData data)
        {
            // данный метод вызывается только у wrapper-а
            throw new NotImplementedException();
        }

        public override IUnitTestControl GetOrCreateUnitTestControl(GetOrCreateUnitTestData data)
        {
            // данный метод вызывается только у wrapper-а
            throw new NotImplementedException();
        }

        public override SetComponentEnableResponseDto Enable()
        {
            return ResponseHelper.GetOfflineResponse<SetComponentEnableResponseDto>();
        }

        public override SetComponentDisableResponseDto Disable(string comment)
        {
            return ResponseHelper.GetOfflineResponse<SetComponentDisableResponseDto>();
        }

        public override SetComponentDisableResponseDto Disable(string comment, DateTime date)
        {
            return ResponseHelper.GetOfflineResponse<SetComponentDisableResponseDto>();
        }
    }
}
