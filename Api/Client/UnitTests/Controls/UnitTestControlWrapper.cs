using System;
using Zidium.Api.Dto;

namespace Zidium.Api
{
    public class UnitTestControlWrapper : UnitTestControlBase
    {
        internal GetOrCreateUnitTestControlData CreateData { get; set; }

        protected ControlActivator<IUnitTestControl> ControlActivator { get; set; }

        internal UnitTestControlWrapper(
            ComponentControlWrapper component,
            IUnitTestTypeControl type,
            string systemName,
            GetOrCreateUnitTestControlData createData)
            : base(component, type, systemName)
        {
            if (createData == null)
            {
                throw new ArgumentNullException("createData");
            }
            CreateData = createData;
            ControlActivator = new ControlActivator<IUnitTestControl>(CreateOnlineControlOrNull, CreateOfflineControl);
        }

        protected IUnitTestControl CreateOnlineControlOrNull()
        {
            // componentId
            if (CreateData.Component.IsFake())
            {
                return null;
            }
            var componentId = CreateData.Component.Info.Id;

            // unitTestTypeId
            if (CreateData.Data.UnitTestTypeControl != null && CreateData.Data.UnitTestTypeControl.IsFake())
            {
                return null;
            }
            var unitTestTypeId = CreateData.Data.UnitTestTypeControl.Info.Id;

            // data
            var response = Client.ApiService.GetOrCreateUnitTest(new GetOrCreateUnitTestRequestDataDto()
            {
                ComponentId = componentId,
                SystemName = CreateData.Data.SystemName,
                DisplayName = CreateData.Data.DisplayName,
                UnitTestTypeId = unitTestTypeId
            });
            if (response.Success)
            {
                return new UnitTestControlOnline(
                    CreateData.Component,
                    CreateData.Data.UnitTestTypeControl,
                    response.GetDataAndCheck());
            }
            return null;
        }

        protected IUnitTestControl CreateOfflineControl()
        {
            return new UnitTestControlOffline(
                CreateData.Component,
                CreateData.Data.UnitTestTypeControl,
                SystemName);
        }


        public override bool IsFake()
        {
            return false;
        }

        public override GetUnitTestStateResponseDto GetState()
        {
            var control = ControlActivator.GetControl();
            return control.GetState();
        }

        public override SetUnitTestDisableResponseDto Disable(SetUnitTestDisableRequestData data)
        {
            var control = ControlActivator.GetControl();
            return control.Disable(data);
        }

        public override SendUnitTestResultResponseDto SendResult(SendUnitTestResultData data)
        {
            var control = ControlActivator.GetControl();
            return control.SendResult(data);
        }

        public override UnitTestDto Info
        {
            get
            {
                var control = ControlActivator.GetControl();
                return control.Info;
            }
        }

        public override SetUnitTestEnableResponseDto Enable()
        {
            var control = ControlActivator.GetControl();
            return control.Enable();
        }
    }
}
