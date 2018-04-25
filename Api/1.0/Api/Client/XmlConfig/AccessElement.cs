using System;

namespace Zidium.Api.XmlConfig
{
    public class AccessElement
    {
        public string AccountName { get; set; }

        public string SecretKey { get; set; }

        public TimeSpan WaitOnError { get; set; }

        public bool Disable { get; set; }

        public string Url { get; set; }

        public AccessElement()
        {
            WaitOnError = TimeSpan.FromMinutes(1);
        }
    }
}
