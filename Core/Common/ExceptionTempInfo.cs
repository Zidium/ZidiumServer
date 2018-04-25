using System;

namespace Zidium.Core.Common
{
    public class ExceptionTempInfo
    {
        public DateTime Date { get; set; }

        public Exception Exception { get; set; }

        public ExceptionTempInfo(Exception exception)
        {
            Date = DateTime.Now;
            Exception = exception;
        }
    }
}
