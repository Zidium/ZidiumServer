using System;

namespace Zidium.Common
{
    /// <summary>
    /// Исключение - объект не найден
    /// </summary>
    public class ObjectNotFoundException : UserFriendlyException
    {
        public ObjectNotFoundException(string message) : base(message)
        {
        }

        public ObjectNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}