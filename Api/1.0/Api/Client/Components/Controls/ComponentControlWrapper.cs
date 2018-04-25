using System;
using System.Collections.Generic;

namespace Zidium.Api
{
    /// <inheritdoc />
    /// <summary>
    /// Обертка над IComponentControl. Её всегда возвращает IClient
    /// </summary>
    internal class ComponentControlWrapper : ComponentControlBase
    {
        protected GetOrCreateComponentControlData GetOrCreateComponentData { get; set; }

        protected ComponentControlOnline GetByIdOnline(Guid id)
        {
            var controlResponse = Client.ApiService.GetComponentControlById(id);
            var controlInfo = controlResponse.Data;
            return new ComponentControlOnline(ClientInternal, controlInfo);
        }

        protected IComponentControl GetByIdOffline(Guid id)
        {
            return new ComponentControlOffline(
                ClientInternal,
                Type,
                id.ToString(),
                null);
        }

        protected IComponentControl GetDefaultControlOnline()
        {
            var data = Client.Config.DefaultComponent;
            if (data.Id.HasValue)
            {
                return GetByIdOnline(data.Id.Value);

            }
            throw new Exception("Не указан defaultComponentId в конфигурационном файле");
        }

        protected IComponentControl CreateRootOnline()
        {
            var response = ClientInternal.ApiService.GetRootControlData();
            if (response.Success && response.Data != null)
            {
                return new ComponentControlOnline(ClientInternal,response.Data);
            }
            return null;
        }

        protected IComponentControl CreateRootOffline()
        {
            var getTypeData = new GetOrCreateComponentTypeData(SystemComponentType.Root.SystemName);
            var type = Client.GetOrCreateComponentTypeControl(getTypeData);
            const string rootVersion = null;
            return new ComponentControlOffline(ClientInternal, type, "Root", rootVersion) //todo Root - надо вынестив служебный класс + сделать его System.Components.Root
            {
                IsRootValue = true
            };
        }


        protected IComponentControl CreateFolderOnline()
        {
            return CreateComponentOnline();
        }

        protected IComponentControl CreateFolderOffline()
        {
            return new ComponentControlOffline(
                ClientInternal,
                Type,
                GetOrCreateComponentData.Data.SystemName,
                GetOrCreateComponentData.Data.Version)
            {
                IsFolderValue = true
            };
        }


        protected IComponentControl CreateComponentOnline()
        {
            if (GetOrCreateComponentData.Data.ComponentTypeControl.IsFake())
            {
                return null;
            }
            if (GetOrCreateComponentData.Parent.IsFake())
            {
                return null;
            }
            var parentId = GetOrCreateComponentData.Parent.Info.Id;
            var response = Client.ApiService.GetOrCreateComponent(parentId, GetOrCreateComponentData.Data);
            if (response.Success && response.Data != null)
            {
                return new ComponentControlOnline(ClientInternal, response.Data);
            }
            return null;
        }

        protected IComponentControl CreateComponentOffline()
        {
            return new ComponentControlOffline(
                ClientInternal,
                Type,
                GetOrCreateComponentData.Data.SystemName,
                GetOrCreateComponentData.Data.Version);
        }


        protected ControlActivator<IComponentControl> ControlActivator { get; set; }

        internal override IComponentControl GetOrCreateChildComponentControl(GetOrCreateComponentControlData data)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }
            lock (this)
            {
                var wrapper = new ComponentControlWrapper(data.Parent.ClientInternal, data.Data.ComponentTypeControl);
                wrapper.GetOrCreateComponentData = data;

                wrapper.ControlActivator = new ControlActivator<IComponentControl>(
                    wrapper.CreateComponentOnline,
                    wrapper.CreateComponentOffline);

                return wrapper;
            }
        }

        #region Создание оберток

        protected ComponentControlWrapper(
            Client client,
            IComponentTypeControl type) : base(client)
        {
            Type = type;
        }

        public static ComponentControlWrapper CreateRoot(Client client)
        {
            var getTypeData = new GetOrCreateComponentTypeData(SystemComponentType.Root.SystemName);
            var type = client.GetOrCreateComponentTypeControl(getTypeData);

            var wrapper = new ComponentControlWrapper(client, type);

            wrapper.ControlActivator = new ControlActivator<IComponentControl>(
                wrapper.CreateRootOnline,
                wrapper.CreateRootOffline);

            return wrapper;
        }

        public static ComponentControlWrapper GetDefault(Client client)
        {
            var type = new ComponentTypeControlOffline(client, "fake");

            var wrapper = new ComponentControlWrapper(client, type);

            wrapper.ControlActivator = new ControlActivator<IComponentControl>(
                wrapper.GetDefaultControlOnline,
                () => wrapper.GetByIdOffline(Guid.Empty));

            return wrapper;
        }

        public static ComponentControlWrapper GetById(Client client, Guid id)
        {
            var type = new ComponentTypeControlOffline(client, "fake");

            var wrapper = new ComponentControlWrapper(client, type);

            wrapper.ControlActivator = new ControlActivator<IComponentControl>(
                () => wrapper.GetByIdOnline(id),
                () => wrapper.GetByIdOffline(id));

            return wrapper;
        }


        public static ComponentControlWrapper CreateFolder(
            ComponentControlWrapper parent,
            GetOrCreateFolderData getOrCreateFolder)
        {
            if (parent == null)
            {
                throw new ArgumentNullException("parent");
            }
            if (getOrCreateFolder == null)
            {
                throw new ArgumentNullException("getOrCreateFolder");
            }

            var folderType = parent.Client.GetFolderComponentTypeControl();
            var getOrCreateComponentData = new GetOrCreateComponentData(getOrCreateFolder.SystemName, folderType)
            {
                DisplayName = getOrCreateFolder.DisplayName,
                Version = null
            };


            var controlData = new GetOrCreateComponentControlData(parent, getOrCreateComponentData);

            var wrapper = new ComponentControlWrapper(
                controlData.Parent.ClientInternal,
                controlData.Data.ComponentTypeControl);

            wrapper.GetOrCreateComponentData = controlData;

            wrapper.ControlActivator = new ControlActivator<IComponentControl>(
                wrapper.CreateFolderOnline,
                wrapper.CreateFolderOffline);

            return wrapper;
        }

        #endregion


        #region Counters

        public override GetMetricsResponse GetMetrics()
        {
            var control = ControlActivator.GetControl();
            return control.GetMetrics();
        }

        public override GetMetricsHistoryResponse GetMetricsHistory(GetMetricsHistoryFilter filter)
        {
            var control = ControlActivator.GetControl();
            return control.GetMetricsHistory(filter);
        }

        public override SendMetricResponse SendMetric(SendMetricData data)
        {
            var control = ControlActivator.GetControl();
            return control.SendMetric(data);
        }

        public override SendMetricsResponse SendMetrics(List<SendMetricData> data)
        {
            var control = ControlActivator.GetControl();
            return control.SendMetrics(data);
        }

        public override GetMetricResponse GetMetric(string name)
        {
            var control = ControlActivator.GetControl();
            return control.GetMetric(name);
        }

        #endregion

        #region Logs

        public override GetLogsResponse GetLogs(GetLogsFilter filter)
        {
            var control = ControlActivator.GetControl();
            return control.GetLogs(filter);
        }

        public override GetLogConfigResponse GetWebLogConfig()
        {
            var control = ControlActivator.GetControl();
            return control.GetWebLogConfig();
        }

        #endregion


        public override GetComponentTotalStateResponse GetTotalState(bool recalc)
        {
            var control = ControlActivator.GetControl();
            return control.GetTotalState(recalc);
        }

        public override GetComponentInternalStateResponse GetInternalState(bool recalc)
        {
            var control = ControlActivator.GetControl();
            return control.GetInternalState(recalc);
        }

        public override void Dispose()
        {
            var control = ControlActivator.GetInternalControl();
            control.Dispose();
        }

        public override bool IsRoot
        {
            get
            {
                var control = ControlActivator.GetControl();
                return control.IsRoot;
            }
        }

        public override bool IsFolder
        {
            get
            {
                var control = ControlActivator.GetControl();
                return control.IsFolder;
            }
        }

        public override ComponentInfo Info
        {
            get
            {
                var control = ControlActivator.GetControl();
                return control.Info;
            }
        }

        public override GetComponentByIdResponse GetParent()
        {
            var control = ControlActivator.GetControl();
            return control.GetParent();
        }

        public override GetChildComponentsResponse GetChildComponents()
        {
            var control = ControlActivator.GetControl();
            return control.GetChildComponents();
        }

        public override UpdateComponentResponse Update(UpdateComponentData data)
        {
            var control = ControlActivator.GetControl();
            var response = control.Update(data);
            return response;
        }

        public override DeleteComponentResponse Delete()
        {
            var control = ControlActivator.GetControl();
            var response = control.Delete();
            return response;
        }

        public override GetOrCreateUnitTestResponse GetOrCreateUnitTest(GetOrCreateUnitTestData data)
        {
            var control = ControlActivator.GetControl();
            return control.GetOrCreateUnitTest(data);
        }

        public override WebLogConfig WebLogConfig
        {
            get
            {
                var control = ControlActivator.GetControl();
                return control.WebLogConfig;
            }
        }

        public override bool IsFake()
        {
            var control = ControlActivator.GetControl();
            return control.IsFake();
        }

        public override string SystemName
        {
            get
            {
                var control = ControlActivator.GetInternalControl();
                return control.SystemName;
            }
        }

        public override string Version
        {
            get
            {
                var control = ControlActivator.GetControl();
                return control.Version;
            }
        }

        public override IComponentControl GetOrCreateChildComponentControl(GetOrCreateComponentData data)
        {
            var controlData = new GetOrCreateComponentControlData(this, data);
            return GetOrCreateChildComponentControl(controlData);
        }

        public override IUnitTestControl GetOrCreateUnitTestControl(GetOrCreateUnitTestData data)
        {
            lock (this)
            {
                var createData = new GetOrCreateUnitTestControlData(this, data);

                var wrapper = new UnitTestControlWrapper(
                    createData.Component,
                    createData.Data.UnitTestTypeControl,
                    createData.Data.SystemName,
                    createData);

                return wrapper;
            }
        }

        public override SetComponentEnableResponse Enable()
        {
            var control = ControlActivator.GetControl();
            return control.Enable();
        }

        public override SetComponentDisableResponse Disable(string comment)
        {
            var control = ControlActivator.GetControl();
            return control.Disable(comment);
        }

        public override SetComponentDisableResponse Disable(string comment, DateTime date)
        {
            var control = ControlActivator.GetControl();
            return control.Disable(comment, date);
        }
    }
}
