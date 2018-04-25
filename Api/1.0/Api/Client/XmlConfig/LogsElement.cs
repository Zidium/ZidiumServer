namespace Zidium.Api.XmlConfig
{
    public class LogsElement
    {
        public WebLogElement WebLog { get; set; }

        public InternalLogElement InternalLog { get; set; }

        public AutoCreateEventsElement AutoCreateEvents { get; set; }

        public LogsElement()
        {
            WebLog = new WebLogElement();
            InternalLog = new InternalLogElement();
            AutoCreateEvents = new AutoCreateEventsElement();
        }
    }
}
