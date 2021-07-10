namespace Zidium.Api.XmlConfig
{
    public class EventsElement
    {
        public EventManagerElement EventManager { get; set; }

        public DefaultEventValuesElement DefaultValues { get; set; }

        public EventsElement()
        {
            EventManager = new EventManagerElement();
            DefaultValues = new DefaultEventValuesElement();
        }
    }
}
