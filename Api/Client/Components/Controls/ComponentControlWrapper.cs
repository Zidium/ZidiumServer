using System;
using System.Collections.Generic;
using Zidium.Api.Dto;

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
            var controlInfo = controlResponse.GetDataAndCheck();
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
                return new ComponentControlOnline(ClientInternal, response.Data);
            }
            return null;
        }

        protected IComponentControl CreateRootOffline()
        {
            var getTypeData = new GetOrCreateComponentTypeRequestDataDto()
            {
                SystemName = SystemComponentType.Root.SystemName
            };
            var type = Client.GetOrCreateComponentTypeControl(getTypeData);
            const string rootVersion = null;
            return new ComponentControlOffline(ClientInternal, type, "Root", rootVersion) //TODO Root - надо вынести в служебный класс + сделать его System.Components.Root
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
            if (GetOrCreateComponentData.Parent.IsFake())
            {
                return null;
            }

            var data = GetOrCreateComponentData.Data;

            if (data.ComponentTypeControl.IsFake())
            {
                return null;
            }
            var parentId = GetOrCreateComponentData.Parent.Info.Id;
            var response = Client.ApiService.GetOrCreateComponent(new GetOrCreateComponentRequestDataDto()
            {
                ParentComponentId = parentId,
                SystemName = data.SystemName,
                DisplayName = data.DisplayName,
                TypeId = data.ComponentTypeControl.Info.Id,
                Version = data.Version,
                Properties = data.Properties.ToDtoList()
            });
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
            var type = client.GetOrCreateComponentTypeControl(SystemComponentType.Root.SystemName);

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

        #region Metrics

        public override GetMetricsResponseDto GetMetrics()
        {
            var control = ControlActivator.GetControl();
            return control.GetMetrics();
        }

        public override GetMetricsHistoryResponseDto GetMetricsHistory(GetMetricsHistoryFilter filter)
        {
            var control = ControlActivator.GetControl();
            return control.GetMetricsHistory(filter);
        }

        public override SendMetricResponseDto SendMetric(SendMetricData data)
        {
            var control = ControlActivator.GetControl();
            return control.SendMetric(data);
        }

        public override SendMetricsResponseDto SendMetrics(List<SendMetricData> data)
        {
            var control = ControlActivator.GetControl();
            return control.SendMetrics(data);
        }

        public override GetMetricResponseDto GetMetric(string name)
        {
            var control = ControlActivator.GetControl();
            return control.GetMetric(name);
        }

        #endregion

        #region Logs

        public override GetLogsResponseDto GetLogs(GetLogsFilter filter)
        {
            var control = ControlActivator.GetControl();
            return control.GetLogs(filter);
        }

        public override GetLogConfigResponseDto GetWebLogConfig()
        {
            var control = ControlActivator.GetControl();
            return control.GetWebLogConfig();
        }

        #endregion

        public override GetComponentTotalStateResponseDto GetTotalState(bool recalc)
        {
            var control = ControlActivator.GetControl();
            return control.GetTotalState(recalc);
        }

        public override GetComponentInternalStateResponseDto GetInternalState(bool recalc)
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

        public override ComponentDto Info
        {
            get
            {
                var control = ControlActivator.GetControl();
                return control.Info;
            }
        }

        public override GetComponentByIdResponseDto GetParent()
        {
            var control = ControlActivator.GetControl();
            return control.GetParent();
        }

        public override GetChildComponentsResponseDto GetChildComponents()
        {
            var control = ControlActivator.GetControl();
            return control.GetChildComponents();
        }

        public override UpdateComponentResponseDto Update(UpdateComponentData data)
        {
            var control = ControlActivator.GetControl();
            var response = control.Update(data);
            return response;
        }

        public override DeleteComponentResponseDto Delete()
        {
            var control = ControlActivator.GetControl();
            var response = control.Delete();
            return response;
        }

        public override GetOrCreateUnitTestResponseDto GetOrCreateUnitTest(GetOrCreateUnitTestData data)
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

        public override SetComponentEnableResponseDto Enable()
        {
            var control = ControlActivator.GetControl();
            return control.Enable();
        }

        public override SetComponentDisableResponseDto Disable(string comment)
        {
            var control = ControlActivator.GetControl();
            return control.Disable(comment);
        }

        public override SetComponentDisableResponseDto Disable(string comment, DateTime date)
        {
            var control = ControlActivator.GetControl();
            return control.Disable(comment, date);
        }

    }
}
