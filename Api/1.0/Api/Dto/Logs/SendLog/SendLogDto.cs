using System;
using System.Collections.Generic;

namespace Zidium.Api.Dto
{
    public class SendLogDto
    {
        public Guid? ComponentId { get; set; }

        public DateTime? Date { get; set; }

        public int? Order { get; set; }
        
        public LogLevel? Level { get; set; }
        
        public string Message { get; set; }

        public string Context { get; set; }

        public List<ExtentionPropertyDto> Properties { get; set; }
    }
}
