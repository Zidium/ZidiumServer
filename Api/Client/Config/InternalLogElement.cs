using System.Text;
using Zidium.Api.Dto;

namespace Zidium.Api.XmlConfig
{
    public class InternalLogElement
    {
        public bool Disable { get; set; }

        public Encoding Encoding { get; set; }

        public string FilePath { get; set; }

        public bool DeleteOldFileOnStartup { get; set; }

        public LogLevel MinLevel { get; set; }

        public InternalLogElement()
        {
            Disable = true;
            FilePath = @"#appDir\Logs\#appName_ZidiumInternal_#date.txt";
            Encoding = Encoding.UTF8;
            DeleteOldFileOnStartup = false;
            MinLevel = LogLevel.Warning;
        }
    }
}
