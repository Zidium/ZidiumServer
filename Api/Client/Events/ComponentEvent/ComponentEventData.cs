using Zidium.Api.Dto;

namespace Zidium.Api
{
    public class ComponentEventData : SendEventBaseT<ComponentEventData>
    {
        public ComponentEventData(IComponentControl componentControl, string typeSystemName)
            :base (componentControl, typeSystemName)
        {
        }

        public override SendEventCategory EventCategory
        {
            get { return SendEventCategory.ComponentEvent; }
        }
    }
}
