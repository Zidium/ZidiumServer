using System;
using Zidium.Api.Dto;

namespace Zidium.Api
{
    public class UnitTestTypeControlWrapper : UnitTestTypeControlBase
    {
        protected GetOrCreateUnitTestTypeData CreateData { get; set; }

        protected ControlActivator<IUnitTestTypeControl> ControlActivator { get; set; }

        public UnitTestTypeControlWrapper(
            Client client, 
            string systemName,  
            GetOrCreateUnitTestTypeData createData)
            : base(client, systemName)
        {
            if (createData == null)
            {
                throw new ArgumentNullException("createData");
            }
            CreateData = createData;
            ControlActivator = new ControlActivator<IUnitTestTypeControl>(CreateOnlineControl, CreateOfflineControl);
        }

        protected IUnitTestTypeControl CreateOnlineControl()
        {
            var data = new GetOrCreateUnitTestTypeData(CreateData.SystemName)
            {
                DisplayName = CreateData.DisplayName
            };
            var response = Client.ApiService.GetOrCreateUnitTestType(data);
            if (response.Success)
            {
                return new UnitTestTypeControlOnline(ClientInternal, SystemName, response.Data);
            }
            return null;
        }

        protected IUnitTestTypeControl CreateOfflineControl()
        {
            return new UnitTestTypeControlOffline(ClientInternal, SystemName);
        }

        public override UnitTestTypeInfo Info
        {
            get
            {
                var control = ControlActivator.GetControl();
                return control.Info;
            }
        }

        public override bool IsFake()
        {
            var control = ControlActivator.GetControl();
            return control.IsFake();
        }
    }
}
