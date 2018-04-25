using System;

namespace Zidium.Api.XmlConfig
{
    public class EventManagerElement
    {
        public bool Disabled { get; set; }

        public int QueueBytes { get; set; }

        public TimeSpan SendPeriod { get; set; }

        public int Threads { get; set; }

        public int MaxSend { get; set; }

        public int MaxJoin { get; set; }

        public EventManagerElement()
        {
            Disabled = false;
            QueueBytes = 1024 * 1024 * 100; // 100 Mb
            SendPeriod = TimeSpan.FromSeconds(5);
            Threads = 1;
            MaxSend = 1000;
            MaxJoin = 1000;
        }
    }
}
