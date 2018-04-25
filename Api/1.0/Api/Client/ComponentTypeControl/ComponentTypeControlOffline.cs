namespace Zidium.Api
{
    public class ComponentTypeControlOffline : ComponentTypeControlBase
    {
        public ComponentTypeControlOffline(Client client, string systemName) : base(client)
        {
            SystemName = systemName;
        }

        public override ComponentTypeInfo Info
        {
            get { return null; }
        }

        public override bool IsFake()
        {
            return true;
        }
    }
}
