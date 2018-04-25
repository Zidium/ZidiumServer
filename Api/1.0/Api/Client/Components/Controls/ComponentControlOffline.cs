using System;
using System.Collections.Generic;

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

        public override ComponentInfo Info
        {
            get { return null; }
        }

        public override GetComponentByIdResponse GetParent()
        {
            return ResponseHelper.GetOfflineResponse<GetComponentByIdResponse>();
        }

        public override GetChildComponentsResponse GetChildComponents()
        {
            return ResponseHelper.GetOfflineResponse<GetChildComponentsResponse>();
        }

        public override UpdateComponentResponse Update(UpdateComponentData data)
        {
            return ResponseHelper.GetOfflineResponse<UpdateComponentResponse>();
        }

        public override DeleteComponentResponse Delete()
        {
            return ResponseHelper.GetOfflineResponse<DeleteComponentResponse>();
        }

        public override GetOrCreateUnitTestResponse GetOrCreateUnitTest(GetOrCreateUnitTestData data)
        {
            return ResponseHelper.GetOfflineResponse<GetOrCreateUnitTestResponse>();
        }

        public override GetLogsResponse GetLogs(GetLogsFilter filter)
        {
            return ResponseHelper.GetOfflineResponse<GetLogsResponse>();
        }

        public override GetLogConfigResponse GetWebLogConfig()
        {
            return ResponseHelper.GetOfflineResponse<GetLogConfigResponse>();
        }

        public override GetComponentTotalStateResponse GetTotalState(bool recalc)
        {
            return ResponseHelper.GetOfflineResponse<GetComponentTotalStateResponse>();
        }

        public override GetComponentInternalStateResponse GetInternalState(bool recalc)
        {
            return ResponseHelper.GetOfflineResponse<GetComponentInternalStateResponse>();
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

        public override GetMetricsResponse GetMetrics()
        {
            return ResponseHelper.GetOfflineResponse<GetMetricsResponse>();
        }

        public override GetMetricsHistoryResponse GetMetricsHistory(GetMetricsHistoryFilter filter)
        {
            return ResponseHelper.GetOfflineResponse<GetMetricsHistoryResponse>();
        }

        public override SendMetricsResponse SendMetrics(List<SendMetricData> data)
        {
            return ResponseHelper.GetOfflineResponse<SendMetricsResponse>();
        }

        public override SendMetricResponse SendMetric(SendMetricData data)
        {
            return ResponseHelper.GetOfflineResponse<SendMetricResponse>();
        }

        public override GetMetricResponse GetMetric(string name)
        {
            return ResponseHelper.GetOfflineResponse<GetMetricResponse>();
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

        public override SetComponentEnableResponse Enable()
        {
            return ResponseHelper.GetOfflineResponse<SetComponentEnableResponse>();
        }

        public override SetComponentDisableResponse Disable(string comment)
        {
            return ResponseHelper.GetOfflineResponse<SetComponentDisableResponse>();
        }

        public override SetComponentDisableResponse Disable(string comment, DateTime date)
        {
            return ResponseHelper.GetOfflineResponse<SetComponentDisableResponse>();
        }
    }
}
