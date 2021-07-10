using System;
using Zidium.Api.Dto;

namespace Zidium.Api
{
    public class LogMessage
    {
        public LogMessage()
        {
            Properties = new ExtentionPropertyCollection();    
        }
        
        public DateTime? Date { get; set; }
        
        public LogLevel? Level { get; set; }
        
        public string Message { get; set; }

        public string Context { get; set; }

        public ExtentionPropertyCollection Properties { get; set; }
    }
}
