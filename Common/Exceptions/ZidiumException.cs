using System;

namespace Zidium.Common
{
    /// <summary>
    /// Базовый класс всех исключений системы мониторинга
    /// </summary>
    public class ZidiumException : Exception
    {
        public ZidiumException(string message)
            : base(message)
        {

        }

        public ZidiumException(string message, Exception innerException)
            : base(message, innerException)
        {

        }
    }
}
