using System;

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
            if (CreateData.Data.UnitTestTypeControl!=null && CreateData.Data.UnitTestTypeControl.IsFake())
            {
                return null;
            }
            var unitTestTypeId = CreateData.Data.UnitTestTypeControl.Info.Id;

            // data
            var data = new GetOrCreateUnitTestData(CreateData.Data.SystemName)
            {
                UnitTestTypeControl = CreateData.Data.UnitTestTypeControl,
                DisplayName = CreateData.Data.DisplayName
            };
            var response = Client.ApiService.GetOrCreateUnitTest(componentId, data);
            if (response.Success)
            {
                return new UnitTestControlOnline(
                    CreateData.Component,
                    CreateData.Data.UnitTestTypeControl,
                    response.Data);
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

        public override GetUnitTestStateResponse GetState()
        {
            var control = ControlActivator.GetControl();
            return control.GetState();
        }

        public override SetUnitTestDisableResponse Disable(SetUnitTestDisableRequestData data)
        {
            var control = ControlActivator.GetControl();
            return control.Disable(data);
        }

        public override SendUnitTestResultResponse SendResult(SendUnitTestResultData data)
        {
            var control = ControlActivator.GetControl();
            return control.SendResult(data);
        }

        public override UnitTestInfo Info
        {
            get
            {
                var control = ControlActivator.GetControl();
                return control.Info;
            }
        }

        public override SetUnitTestEnableResponse Enable()
        {
            var control = ControlActivator.GetControl();
            return control.Enable();
        }
    }
}
