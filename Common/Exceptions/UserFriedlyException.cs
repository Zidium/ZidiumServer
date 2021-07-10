using System;

namespace Zidium.Common
{
    /// <summary>
    /// Сообщение для пользователя, передаваемое как исключение
    /// </summary>
    public class UserFriendlyException : ZidiumException
    {
        public UserFriendlyException(string message)
            : base(message)
        {

        }

        public UserFriendlyException(string message, Exception innerException)
            : base(message, innerException)
        {

        }
    }
}
