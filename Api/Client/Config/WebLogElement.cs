using System;

namespace Zidium.Api.XmlConfig
{
    public class WebLogElement
    {
        public bool Disable { get; set; }

        public int Threads { get; set; }

        public int BatchBytes { get; set; }

        public TimeSpan SendPeriod { get; set; }

        public TimeSpan ReloadConfigsPeriod { get; set; }

        public int QueueBytes { get; set; }

        public WebLogElement()
        {
            Disable = false;
            Threads = 3;
            BatchBytes = 1024 * 100; // 100 Кбайт 
            SendPeriod = TimeSpan.FromSeconds(5);
            ReloadConfigsPeriod = TimeSpan.FromMinutes(5);
            QueueBytes = 1024 * 1024 * 100; // 100 Мбайт
        }
    }
}
