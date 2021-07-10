using System;
using Zidium.Api.Dto;

namespace Zidium.Api
{
    public class WebLogMessage
    {
        public DateTime CreateDate { get; set; }

        public int Attemps { get; set; }

        public DateTime? LastAttempTime { get; set; }

        public int? Order { get; set; }

        public IComponentControl ComponentControl { get; set; }

        public DateTime? Date { get; set; }

        public LogLevel? Level { get; set; }

        public string Message { get; set; }

        public string Context { get; set; }

        public ExtentionPropertyCollection Properties { get; set; }

        public int Size { get; set; }
    }
}
