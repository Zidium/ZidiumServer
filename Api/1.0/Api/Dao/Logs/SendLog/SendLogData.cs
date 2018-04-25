using System;

namespace Zidium.Api
{
    public class SendLogData
    {
        public SendLogData()
        {
            Properties = new ExtentionPropertyCollection();    
        }

        public Guid? ComponentId { get; set; }

        public DateTime? Date { get; set; }

        public int? Order { get; set; }
        
        public LogLevel? Level { get; set; }
        
        public string Message { get; set; }

        public string Context { get; set; }

        public ExtentionPropertyCollection Properties { get; set; }
    }
}
