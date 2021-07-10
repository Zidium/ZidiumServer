using System;
using Zidium.Api.Dto;

namespace Zidium.Api
{
    public class UnitTestTypeControlWrapper : UnitTestTypeControlBase
    {
        protected GetOrCreateUnitTestTypeRequestDataDto CreateData { get; set; }

        protected ControlActivator<IUnitTestTypeControl> ControlActivator { get; set; }

        public UnitTestTypeControlWrapper(
            Client client,
            string systemName,
            GetOrCreateUnitTestTypeRequestDataDto createData)
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
            var data = new GetOrCreateUnitTestTypeRequestDataDto()
            {
                SystemName = CreateData.SystemName,
                DisplayName = CreateData.DisplayName
            };
            var response = Client.ApiService.GetOrCreateUnitTestType(data);
            if (response.Success)
            {
                return new UnitTestTypeControlOnline(ClientInternal, SystemName, response.GetDataAndCheck());
            }
            return null;
        }

        protected IUnitTestTypeControl CreateOfflineControl()
        {
            return new UnitTestTypeControlOffline(ClientInternal, SystemName);
        }

        public override UnitTestTypeDto Info
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
