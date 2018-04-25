using System;
using System.Collections.Generic;

namespace Zidium.Api
{
    internal class ComponentControlOnline : ComponentControlBase
    {
        public ComponentControlOnline(
            Client client,
            ComponentControlData controlData)
            : base(client)
        {
            if (client == null)
            {
                throw new ArgumentNullException("client");
            }
            if (controlData == null)
            {
                throw new ArgumentNullException("controlData");
            }
            Type = new ComponentTypeControlOnline(ClientInternal, controlData.Component.Type);
            InfoInternal = controlData.Component;
            WebLogConfigInternal = controlData.WebLogConfig;
        }

        protected ComponentInfo InfoInternal { get; set; }

        public override ComponentInfo Info
        {
            get { return InfoInternal; }
        }

        public override bool IsRoot
        {
            get { return Info.Type.IsRoot(); }
        }

        public override bool IsFolder
        {
            get { return Info.Type.IsFolder(); }
        }


        #region Компоненты и папки

        public override GetComponentByIdResponse GetParent()
        {
            if (Info.ParentId == null)
            {
                return ResponseHelper.GetClientErrorResponse<GetComponentByIdResponse>("ParentId is null");
            }
            return Client.ApiService.GetComponentById(Info.ParentId.Value);
        }

        public override GetChildComponentsResponse GetChildComponents()
        {
            return Client.ApiService.GetChildComponents(Info.Id);
        }

        public override UpdateComponentResponse Update(UpdateComponentData data)
        {
            return Client.ApiService.UpdateComponent(Info.Id, data);
        }

        public override DeleteComponentResponse Delete()
        {
            return Client.ApiService.DeleteComponent(Info.Id);
        }

        #endregion

        public override void Dispose()
        {
            Client.WebLogManager.EndReloadConfig(this);
        }

        public override GetOrCreateUnitTestResponse GetOrCreateUnitTest(GetOrCreateUnitTestData data)
        {
            if (data.UnitTestTypeControl!=null && data.UnitTestTypeControl.IsFake())
            {
                return ResponseHelper.GetOfflineResponse<GetOrCreateUnitTestResponse>();
            }
            return Client.ApiService.GetOrCreateUnitTest(Info.Id, data);
        }

        public override GetLogsResponse GetLogs(GetLogsFilter filter)
        {
            return Client.ApiService.GetLogs(Info.Id, filter);
        }

        public override GetLogConfigResponse GetWebLogConfig()
        {
            return Client.ApiService.GetLogConfig(Info.Id);
        }

        public override GetComponentTotalStateResponse GetTotalState(bool recalc)
        {
            return Client.ApiService.GetComponentTotalState(Info.Id, recalc);
        }

        public override GetComponentInternalStateResponse GetInternalState(bool recalc)
        {
            return Client.ApiService.GetComponentInternalState(Info.Id, recalc);
        }

        protected bool IsWebLogConfigRegistered { get; set; }

        protected WebLogConfig WebLogConfigInternal { get; set; }

        public override WebLogConfig WebLogConfig
        {
            get
            {
                // поставим веб-конфиг на периодическую перезагрузку
                if (IsWebLogConfigRegistered == false)
                {
                    lock (this)
                    {
                        if (IsWebLogConfigRegistered == false)
                        {
                            Client.WebLogManager.BeginReloadConfig(this);
                            IsWebLogConfigRegistered = true;
                        }
                    }
                }
                return WebLogConfigInternal;
            }
        }

        public override bool IsFake()
        {
            return false;
        }

        public override string SystemName
        {
            get { return Info.SystemName; }
        }

        public override string Version
        {
            get { return Info.Version; }
        }


        #region Метрики

        public override GetMetricsResponse GetMetrics()
        {
            return Client.ApiService.GetMetrics(Info.Id);
        }

        public override GetMetricsHistoryResponse GetMetricsHistory(GetMetricsHistoryFilter filter)
        {
            return Client.ApiService.GetMetricsHistory(Info.Id, filter);
        }

        public override SendMetricsResponse SendMetrics(List<SendMetricData> data)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }
            return Client.ApiService.SendMetrics(Info.Id, data);
        }

        public override SendMetricResponse SendMetric(SendMetricData data)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }
            return Client.ApiService.SendMetric(Info.Id, data);
        }

        public override GetMetricResponse GetMetric(string name)
        {
            return Client.ApiService.GetMetric(Info.Id, name);
        }

        #endregion


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
            return Client.ApiService.SetComponentEnable(Info.Id);
        }

        public override SetComponentDisableResponse Disable(string comment)
        {
            return Client.ApiService.SetComponentDisable(Info.Id, null, comment);
        }

        public override SetComponentDisableResponse Disable(string comment, DateTime date)
        {
            return Client.ApiService.SetComponentDisable(Info.Id, date, comment);
        }
    }
}
