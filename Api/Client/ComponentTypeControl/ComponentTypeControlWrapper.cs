using System;
using Zidium.Api.Dto;

namespace Zidium.Api
{
    public class ComponentTypeControlWrapper : ComponentTypeControlBase
    {
        protected ControlActivator<IComponentTypeControl> ControlActivator { get; set; }

        protected GetOrCreateComponentTypeRequestDataDto GetOrCreateData { get; set; }

        public ComponentTypeControlWrapper(
            Client client, 
            string systemName,
            GetOrCreateComponentTypeRequestDataDto getOrCreateData) 
            : base(client)
        {
            if (getOrCreateData == null)
            {
                throw new ArgumentNullException("getOrCreateData");
            }
            SystemName = systemName;
            ControlActivator = new ControlActivator<IComponentTypeControl>(CreateOnline, CreateOffline);
            GetOrCreateData = getOrCreateData;
        }

        protected IComponentTypeControl CreateOnline()
        {
            var response = Client.ApiService.GetOrCreateComponentType(GetOrCreateData);
            if (response.Success)
            {
                return new ComponentTypeControlOnline(ClientInternal, response.GetDataAndCheck());
            }
            return null;
        }

        protected IComponentTypeControl CreateOffline()
        {
            return new ComponentTypeControlOffline(ClientInternal, SystemName);
        }



        public override ComponentTypeDto Info
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
