using System;

namespace Zidium.Api.XmlConfig
{
    public class AccessElement
    {
        public string Url { get; set; }

        public string SecretKey { get; set; }

        public bool Disable { get; set; }

        public TimeSpan WaitOnError { get; set; }

        public TimeSpan Timeout { get; set; }

        public AccessElement()
        {
            WaitOnError = TimeSpan.FromMinutes(1);
            Timeout = TimeSpan.FromMinutes(1);
        }
    }
}
