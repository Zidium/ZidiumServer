using System;
using System.Collections.Generic;
using System.Linq;
using Zidium.Api.Dto;

namespace Zidium.Api
{
    internal class ComponentControlOnline : ComponentControlBase
    {
        public ComponentControlOnline(
            Client client,
            ComponentControlDataDto controlData)
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
            WebLogConfigInternal = new WebLogConfig() {
                ComponentId = controlData.WebLogConfig.ComponentId,
                Enabled = controlData.WebLogConfig.Enabled,
                IsDebugEnabled = controlData.WebLogConfig.IsDebugEnabled,
                IsErrorEnabled = controlData.WebLogConfig.IsErrorEnabled,
                IsFatalEnabled = controlData.WebLogConfig.IsFatalEnabled,
                IsInfoEnabled = controlData.WebLogConfig.IsInfoEnabled,
                IsTraceEnabled = controlData.WebLogConfig.IsTraceEnabled,
                IsWarningEnabled = controlData.WebLogConfig.IsWarningEnabled,
                LastUpdateDate = controlData.WebLogConfig.LastUpdateDate
            };
        }

        protected ComponentDto InfoInternal { get; set; }

        public override ComponentDto Info
        {
            get { return InfoInternal; }
        }

        public override bool IsRoot
        {
            get { return Info.Type.Id == SystemComponentType.Root.Id; }
        }

        public override bool IsFolder
        {
            get { return Info.Type.Id == SystemComponentType.Folder.Id; }
        }

        #region Компоненты и папки

        public override GetComponentByIdResponseDto GetParent()
        {
            if (Info.ParentId == null)
            {
                return ResponseHelper.GetClientErrorResponse<GetComponentByIdResponseDto>("ParentId is null");
            }
            return Client.ApiService.GetComponentById(Info.ParentId.Value);
        }

        public override GetChildComponentsResponseDto GetChildComponents()
        {
            return Client.ApiService.GetChildComponents(Info.Id);
        }

        public override UpdateComponentResponseDto Update(UpdateComponentData data)
        {
            return Client.ApiService.UpdateComponent(new UpdateComponentRequestDataDto()
            {
                Id = Info.Id,
                ParentId = data.ParentId,
                SystemName = data.SystemName,
                DisplayName = data.DisplayName,
                TypeId = data.TypeId,
                Version = data.Version,
                Properties = data.Properties?.ToDtoList()
            });
        }

        public override DeleteComponentResponseDto Delete()
        {
            return Client.ApiService.DeleteComponent(Info.Id);
        }

        #endregion

        public override void Dispose()
        {
            Client.WebLogManager.EndReloadConfig(this);
        }

        public override GetOrCreateUnitTestResponseDto GetOrCreateUnitTest(GetOrCreateUnitTestData data)
        {
            if (data.UnitTestTypeControl != null && data.UnitTestTypeControl.IsFake())
            {
                return ResponseHelper.GetOfflineResponse<GetOrCreateUnitTestResponseDto>();
            }
            return Client.ApiService.GetOrCreateUnitTest(new GetOrCreateUnitTestRequestDataDto()
            {
                ComponentId = Info.Id,
                SystemName = data.SystemName,
                DisplayName = data.DisplayName,
                UnitTestTypeId = data.UnitTestTypeControl.Info.Id
            });
        }

        public override GetLogsResponseDto GetLogs(GetLogsFilter filter)
        {
            return Client.ApiService.GetLogs(new GetLogsRequestDataDto()
            {
                ComponentId = Info.Id,
                Context = filter.Context,
                From = filter.From,
                To = filter.To,
                Levels = filter.Levels,
                Message = filter.Message,
                MaxCount = filter.MaxCount,
                PropertyName = filter.PropertyName,
                PropertyValue = filter.PropertyValue
            });
        }

        public override GetLogConfigResponseDto GetWebLogConfig()
        {
            return Client.ApiService.GetLogConfig(Info.Id);
        }

        public override GetComponentTotalStateResponseDto GetTotalState(bool recalc)
        {
            return Client.ApiService.GetComponentTotalState(Info.Id, recalc);
        }

        public override GetComponentInternalStateResponseDto GetInternalState(bool recalc)
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

        public override GetMetricsResponseDto GetMetrics()
        {
            return Client.ApiService.GetMetrics(Info.Id);
        }

        public override GetMetricsHistoryResponseDto GetMetricsHistory(GetMetricsHistoryFilter filter)
        {
            return Client.ApiService.GetMetricsHistory(new GetMetricsHistoryRequestDataDto()
            {
                ComponentId = Info.Id,
                Name = filter.Name,
                From = filter.From,
                To = filter.To,
                MaxCount = filter.MaxCount
            });
        }

        public override SendMetricsResponseDto SendMetrics(List<SendMetricData> data)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }
            return Client.ApiService.SendMetrics(data.Select(t => new SendMetricRequestDataDto()
            {
                ComponentId = Info.Id,
                Name = t.Name,
                Value = t.Value,
                ActualIntervalSecs = t.ActualInterval?.TotalSeconds
            }).ToList());
        }

        public override SendMetricResponseDto SendMetric(SendMetricData data)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }
            return Client.ApiService.SendMetric(new SendMetricRequestDataDto()
            {
                ComponentId = Info.Id,
                Name = data.Name,
                Value = data.Value,
                ActualIntervalSecs = data.ActualInterval?.TotalSeconds
            });
        }

        public override GetMetricResponseDto GetMetric(string name)
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

        public override SetComponentEnableResponseDto Enable()
        {
            return Client.ApiService.SetComponentEnable(Info.Id);
        }

        public override SetComponentDisableResponseDto Disable(string comment)
        {
            return Client.ApiService.SetComponentDisable(Info.Id, null, comment);
        }

        public override SetComponentDisableResponseDto Disable(string comment, DateTime date)
        {
            return Client.ApiService.SetComponentDisable(Info.Id, date, comment);
        }
    }
}
