namespace Zidium.Api.XmlConfig
{
    public class Config
    {
        public AccessElement Access { get; set; }

        public DefaultComponentElement DefaultComponent { get; set; }

        public LogsElement Logs { get; set; }

        public EventsElement Events { get; set; }

        public Config()
        {
            Access = new AccessElement();
            DefaultComponent = new DefaultComponentElement();
            Logs = new LogsElement();
            Events = new EventsElement();
        }
    }
}
