using Zidium.Api.Dto;

namespace Zidium.Api
{
    public class ComponentTypeControlOffline : ComponentTypeControlBase
    {
        public ComponentTypeControlOffline(Client client, string systemName) : base(client)
        {
            SystemName = systemName;
        }

        public override ComponentTypeDto Info
        {
            get { return null; }
        }

        public override bool IsFake()
        {
            return true;
        }
    }
}
